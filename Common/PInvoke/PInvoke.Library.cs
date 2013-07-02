using System;
using System.Runtime.InteropServices;

namespace Common
{
    public static partial class PInvoke
    {
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms684179(v=vs.85).aspx
        [Flags]
        private enum LoadLibraryExFlags : uint
        {
            DONT_RESOLVE_DLL_REFERENCES = 0x01,
            LOAD_LIBRARY_AS_DATAFILE = 0x02
        }


        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms684179(v=vs.85).aspx
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(string filename, IntPtr reserved, uint flags);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms683152(v=vs.85).aspx
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr module);

        // loads library as datafile for following resource manipulations
        public static IntPtr LoadDatafile(string filename)
        {
            return LoadLibraryEx(filename, IntPtr.Zero, 
                (uint)(LoadLibraryExFlags.LOAD_LIBRARY_AS_DATAFILE | 
                    LoadLibraryExFlags.DONT_RESOLVE_DLL_REFERENCES));
        }
    }
}
