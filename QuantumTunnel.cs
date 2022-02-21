using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace QuantumTunnel
{
    class QuantumTunnel
    {
        static void Main(string[] args)
        {
            Console.WriteLine("QuantumTunnel - C# FlashFS Reader");
            if(args.Length == 0)
            {
                Console.WriteLine("Provide name of file on flash. Example: certkeys.bin OR -rawdump to obtain a raw image of the flash, named flash.bin.");
                Environment.Exit(1);
            }
            if(args[0].ToLower() == "-rawdump")
            {
                Console.WriteLine("Dumping raw flash image, this will take a while...");
                if (Flash.DumpFlashImage("flash.bin"))
                {
                    Console.WriteLine("Dumped flash to flash.bin");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine($"An error occured, do you have privileges? Error: {new Win32Exception(Marshal.GetLastWin32Error()).Message}");
                    Environment.Exit(1);
                }
            }
            string filename = args[0];
            FlashFS.ReadFile(filename);
            Console.WriteLine($"File read to {Environment.CurrentDirectory}\\{filename}");
        }
    }

    internal static class FlashFS
    {
        private static readonly string FlashDeviceName = "\\\\.\\Xvuc\\FlashFs";

        public static bool ReadFile(string name)
        {
            string fullpath = FlashDeviceName + "\\" + name;
            IntPtr pHandle = KernelBase.CreateFile(fullpath, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite, IntPtr.Zero, System.IO.FileMode.Open, System.IO.FileAttributes.Normal, IntPtr.Zero);
            if (pHandle == IntPtr.Zero)
            {
                Console.WriteLine("Failed to get handle to {0}", fullpath);
                return false;
            }

            uint numBytesRead = 0;
            ulong bytesReadTotal = 0;
            byte[] buf = new byte[1024 * 1024]; // 1kb

            using (FileStream fsOutputFile = new FileStream(name, FileMode.Create, FileAccess.Write))
            {
                do
                {
                    if (!KernelBase.ReadFile(pHandle, buf, (uint)buf.Length, out numBytesRead, IntPtr.Zero))
                    {
                        Console.WriteLine("Failed to ReadFile {0}, error: 0x{1:X}", fullpath, KernelBase.GetLastError());
                        KernelBase.CloseHandle(pHandle);
                        return false;
                    }
                    fsOutputFile.Write(buf, 0, (int)numBytesRead);
                    bytesReadTotal += numBytesRead;
                }
                while (numBytesRead > 0);
            }
            Console.WriteLine("Read {0} bytes", bytesReadTotal);
            KernelBase.CloseHandle(pHandle);
            return true;
        }
    }

    internal static class Flash
    {
        private static readonly string RawFlashDeviceName = "\\\\.\\Xvuc\\Flash";

        public static bool DumpFlashImage(string DumpToPath)
        {
            IntPtr pHandle = KernelBase.CreateFile(RawFlashDeviceName, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite, IntPtr.Zero, System.IO.FileMode.Open, System.IO.FileAttributes.Normal, IntPtr.Zero);
            if (pHandle == IntPtr.Zero)
            {
                Console.WriteLine("Failed to get handle to {0}", RawFlashDeviceName);
                return false;
            }

            bool success = false;
            uint numBytesRead = 0;
            ulong bytesReadTotal = 0;
            byte[] buf = new byte[1024 * 1024]; // 1kb

            using (FileStream fsOutputFile = new FileStream(DumpToPath, FileMode.Create, FileAccess.Write))
            {
                do
                {
                    success = KernelBase.ReadFile(pHandle, buf, (uint)buf.Length, out numBytesRead, IntPtr.Zero);
                    if (!success && bytesReadTotal == 0) // Only fail if nothing was read yet
                    {
                        Console.WriteLine("Failed to ReadFile {0}, error: 0x{1:X}", RawFlashDeviceName, KernelBase.GetLastError());
                        Console.WriteLine(numBytesRead);
                        KernelBase.CloseHandle(pHandle);
                        return false;
                    }

                    fsOutputFile.Write(buf, 0, (int)numBytesRead);
                    bytesReadTotal += numBytesRead;
                }
                while (numBytesRead > 0);
            }
            Console.WriteLine("Read {0} bytes", bytesReadTotal);
            KernelBase.CloseHandle(pHandle);
            return true;
        }
    }

    internal static class KernelBase
    {
        [DllImport("kernelbase.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFile(
        [MarshalAs(UnmanagedType.LPWStr)] string filename,
        [MarshalAs(UnmanagedType.U4)] FileAccess access,
        [MarshalAs(UnmanagedType.U4)] FileShare share,
        IntPtr securityAttributes,
        [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
        IntPtr templateFile);

        [DllImport("kernelbase.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernelbase.dll", SetLastError = true)]
        public static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer,
            uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernelbase.dll")]
        public static extern uint GetLastError();
    }

}
