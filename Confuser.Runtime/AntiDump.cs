using System;
using System.Runtime.InteropServices;

namespace Confuser.Runtime
{
    internal static class AntiDump
    {
        [DllImport("kernel32.dll")]
        private static extern unsafe bool VirtualProtect(byte* lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

        private static unsafe void Initialize()
        {
            #region Stuffs

            uint AnY;

            var module = typeof(AntiDump).Module;
            var bas = (byte*)Marshal.GetHINSTANCE(module);

            var ptr = bas + 0x3c;
            byte* ptr2;
            ptr = ptr2 = bas + *(uint*)ptr;
            ptr += 0x6;

            var sectNum = *(ushort*)ptr;
            ptr += 14;

            var optSize = *(ushort*)ptr;
            ptr = ptr2 = ptr + 0x4 + optSize;

            byte* @new = stackalloc byte[11];

            #endregion  

            // Prevents dumping performed by famous tools as MegaDumper
            VirtualProtect(ptr - 16, 8, 0x40, out AnY);
            *(uint*)(ptr - 12) = 0;
            var mdDir = bas + *(uint*)(ptr - 16);
            *(uint*)(ptr - 16) = 0;

            // Erase MetaData (DataDir) - This is the most important part of the code!
            VirtualProtect(mdDir, 0x48, 0x40, out AnY);
            var mdHdr = bas + *(uint*)(mdDir + 8);
            *(uint*)mdDir = 0;
            *((uint*)mdDir + 1) = 0;
            *((uint*)mdDir + 2) = 0;
            *((uint*)mdDir + 3) = 0;

            // Erase value for MetaData.RVA (BSJB)
            VirtualProtect(mdHdr, 4, 0x40, out AnY);
            *(uint*)mdHdr = 0;

            // Erase sections name
            for (int i = 0; i < sectNum; i++)
            {
                VirtualProtect(ptr, 8, 0x40, out AnY);
                Marshal.Copy(new byte[8], 0, (IntPtr)ptr, 8);
                ptr += 0x28;
            }
        }
    }
}