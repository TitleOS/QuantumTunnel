using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuantumTunnel
{
    public static class FlashFS
    {
        private static readonly string FlashDeviceName = "\\\\.\\Xvuc\\FlashFs";

        public static void ReadFile(string name)
        {
            string fullpath = FlashDeviceName + "\\" + name;
            IntPtr FileHandle = KernelBase.CreateFile(fullpath, System.IO.FileAccess.Read, System.IO.FileShare.None, IntPtr.Zero, System.IO.FileMode.Open, System.IO.FileAttributes.Normal, IntPtr.Zero);
            FileStream FlashStream = new FileStream(FileHandle, FileAccess.Read);
            byte[] b = null;
            using (MemoryStream ms = new MemoryStream())
            {
                int count = 0;
                do
                {
                    byte[] buf = new byte[1024];
                    count = FlashStream.Read(buf, 0, 1024);
                    ms.Write(buf, 0, count);
                } while (FlashStream.CanRead && count > 0);
                b = ms.ToArray();
                File.WriteAllBytes(name, b);
            }
        }

    }
}
