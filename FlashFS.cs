using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace QuantumTunnel
{
    public static class FlashFS
    {
        private static readonly string FlashDeviceName = "\\\\.\\Xvuc\\FlashFs";
        
        public static bool ReadFile(string name)
        {
            string fullpath = FlashDeviceName + "\\" + name;
            IntPtr pHandle = KernelBase.CreateFile(fullpath, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite, IntPtr.Zero, System.IO.FileMode.Open, System.IO.FileAttributes.Normal, IntPtr.Zero);
            if(pHandle == IntPtr.Zero)
            {
                Console.WriteLine("Failed to get handle to {0}", fullpath);
                return false;
            }

            uint numBytesRead = 0;
            ulong bytesReadTotal = 0;
            byte[] buf = new byte[1024*1024]; // 1kb

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
                while(numBytesRead > 0);
            }
            Console.WriteLine("Read {0} bytes", bytesReadTotal);
            KernelBase.CloseHandle(pHandle);
            return true;
        }
    }
}
