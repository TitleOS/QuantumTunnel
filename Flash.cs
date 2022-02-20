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
            ;
            using (FileStream fsFlash = new FileStream(FileHandle, FileAccess.Read))
            using (FileStream fsOutputFile = new FileStream(DumpToPath, FileMode.Create, FileAccess.Write))
            {
                int count = 0;
                byte[] buf = new byte[1024 * 1024]; // 1MB
                do
                {
                    count = fsFlash.Read(buf, 0, buf.Length);
                    fsOutputFile.Write(buf, 0, count);
                    total -=  count;
                } while (fsFlash.CanRead && count > 0);
            }
            return true;
        }
    }
}
