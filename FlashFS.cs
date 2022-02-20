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
            IntPtr pHandle = KernelBase.CreateFile(fullpath, System.IO.FileAccess.Read, System.IO.FileShare.None, IntPtr.Zero, System.IO.FileMode.Open, System.IO.FileAttributes.Normal, IntPtr.Zero);
            if(pHandle == IntPtr.Zero)
            {
                return false;
            }
            // Wrap the raw ptr into self-disposing handle
            SafeFileHandle hFlashFile = new SafeFileHandle(pHandle, true);

            using (FileStream fsFlashFile = new FileStream(hFlashFile, FileAccess.Read))
            using (FileStream fsOutputFile = new FileStream(name, FileMode.Create, FileAccess.Write))
            {
                int count = 0;
                byte[] buf = new byte[1024]; // 1KB
                do
                {
                    count = fsFlashFile.Read(buf, 0, 1024);
                    fsOutputFile.Write(buf, 0, count);
                } while (fsFlashFile.CanRead && count > 0);
            }
            return true;
        }
    }
}
