using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantumTunnel
{
    internal static class Flash
    {
        private static readonly string RawFlashDeviceName = "\\\\.\\Xvuc\\Flash";

        public static bool DumpFlashImage(string DumpToPath)
        {
            IntPtr FileHandle = IntPtr.Zero;
            FileHandle = KernelBase.CreateFile(RawFlashDeviceName, System.IO.FileAccess.Read, System.IO.FileShare.None, IntPtr.Zero, System.IO.FileMode.Open, System.IO.FileAttributes.Normal, IntPtr.Zero);
            if(FileHandle == IntPtr.Zero)
            {
                return false;
            }
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
                File.WriteAllBytes(DumpToPath, b);
            }
            return true;

        }
    }
}
