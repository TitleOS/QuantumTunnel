using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace QuantumTunnel
{
    internal static class Flash
    {
        private static readonly string RawFlashDeviceName = "\\\\.\\Xvuc\\Flash";

        public static bool DumpFlashImage(string DumpToPath)
        {
            IntPtr pHandle = KernelBase.CreateFile(RawFlashDeviceName, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite, IntPtr.Zero, System.IO.FileMode.Open, System.IO.FileAttributes.Normal, IntPtr.Zero);
            if(pHandle == IntPtr.Zero)
            {
                Console.WriteLine("Failed to get handle to {0}", RawFlashDeviceName);
                return false;
            }

            bool success = false;
            uint numBytesRead = 0;
            ulong bytesReadTotal = 0;
            byte[] buf = new byte[1024*1024]; // 1kb

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
                while(numBytesRead > 0);
            }
            Console.WriteLine("Read {0} bytes", bytesReadTotal);
            KernelBase.CloseHandle(pHandle);
            return true;
        }
    }
}
