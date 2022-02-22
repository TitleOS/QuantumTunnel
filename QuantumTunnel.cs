using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using CommandLine;

namespace QuantumTunnel
{
    internal static class Flash
    {
        private static readonly string RawFlashDeviceName = "\\\\.\\Xvuc\\Flash";
        private static readonly string FlashDeviceName = "\\\\.\\Xvuc\\FlashFs";

        /// <summary>
        /// Read raw flash image.
        /// </summary>
        /// <param name="outputFile">Filepath to save the flashdump.</param>
        /// <returns>Zero on success, Non-Zero on failure</returns>
        public static int ReadFlashImage(string outputFile)
        {
            return ReadInternal(RawFlashDeviceName, outputFile);
        }

        /// <summary>
        /// Read single file from flash filesystem.
        /// </summary>
        /// <param name="targetFile">Filename of file to read.</param>
        /// <param name="outputFile">Filepath to save the file.</param>
        /// <returns>Zero on success, Non-Zero on failure</returns>
        public static int ReadFlashFsFile(string targetFile, string outputFile)
        {
            string fullpath = FlashDeviceName + "\\" + targetFile;
            return ReadInternal(fullpath, outputFile);
        }

        /// <summary>
        /// Reads from opened DeviceHandle until EOF is signalled.
        /// </summary>
        /// <param name="devicePath">Full device path (e.g. \\.\Xvuc\FlashFs\filename.bin)</param>
        /// <param name="outputFile">Filepath to save the file.</param>
        /// <returns>Zero on success, Non-Zero on failure</returns>
        public static int ReadInternal(string devicePath, string outputFile)
        {
            IntPtr pHandle = KernelBase.CreateFile(devicePath, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite, IntPtr.Zero, System.IO.FileMode.Open, System.IO.FileAttributes.Normal, IntPtr.Zero);
            if (pHandle == IntPtr.Zero)
            {
                Console.WriteLine("Failed to get handle to {0}", devicePath);
                return -3;
            }

            bool success = false;
            uint numBytesRead = 0;
            ulong bytesReadTotal = 0;
            byte[] buf = new byte[1024 * 1024]; // 1kb

            using (FileStream fsOutputFile = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                do
                {
                    success = KernelBase.ReadFile(pHandle, buf, (uint)buf.Length, out numBytesRead, IntPtr.Zero);
                    if (!success && bytesReadTotal == 0 && numBytesRead == 0)
                    {
                        Console.WriteLine("Failed to ReadFile {0}, error: 0x{1:X}", devicePath, KernelBase.GetLastError());
                        KernelBase.CloseHandle(pHandle);
                        return -4;
                    }
                    fsOutputFile.Write(buf, 0, (int)numBytesRead);
                    bytesReadTotal += numBytesRead;
                }
                while (numBytesRead > 0);
            }
            Console.WriteLine("Read {0} bytes", bytesReadTotal);
            KernelBase.CloseHandle(pHandle);
            return 0;
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

    class CommandLineOptions
    {
        [Value(index: 0, Required = false, HelpText = "File to dump from FlashFs.")]
        public string TargetFile { get; set; }

        [Option(shortName: 'o', longName: "output", Required = false, HelpText = "Output filepath.")]
        public string OutputFile { get; set; }

        [Option(shortName: 'r', longName: "rawdump", Required = false, HelpText = "Toggle raw flashimage dumping.")]
        public bool DoRawdump { get; set; }
    }

    class QuantumTunnel
    {
        /// <param name="rawdump">Dump full raw flash</param>
        /// <param name="filename">Filename of file to dump from FlashFs</param>
        /// <param name="output">Output filepath</param>
        static int Main(string[] args)
        {
            Console.WriteLine("QuantumTunnel - C# FlashFS Reader");

            var exitCode = Parser.Default.ParseArguments<CommandLineOptions>(args)
                .MapResult(
                    (CommandLineOptions opts) => {
                        if (!String.IsNullOrEmpty(opts.TargetFile))
                        {
                            return Flash.ReadFlashFsFile(opts.TargetFile, opts.OutputFile ?? opts.TargetFile);
                        }
                        else if (opts.DoRawdump)
                        {
                            return Flash.ReadFlashImage(opts.OutputFile ?? "flash.bin");
                        }
                        else
                        {
                            Console.WriteLine("Error: Neither filename or rawdump flag provided!");
                            return -2;
                        }
                    },
                    _ => {
                        Console.WriteLine("Error parsing arguments! Use --help!");
                        return -1;
                    });

            return exitCode;
        }
    }

}
