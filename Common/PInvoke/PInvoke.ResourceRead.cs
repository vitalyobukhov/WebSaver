using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Common
{
    public static partial class PInvoke
    {
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms648042(v=vs.85).aspx
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindResource(IntPtr module, uint name, uint type);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms648048(v=vs.85).aspx
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint SizeofResource(IntPtr module, IntPtr findHandle);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms648046(v=vs.85).aspx
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LoadResource(IntPtr module, IntPtr findHandle);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms648047(v=vs.85).aspx
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LockResource(IntPtr loadHandle);


        // gets specified by type and name resource data within module
        public static byte[] LoadResourceBytes(IntPtr module, ushort type, ushort name)
        {
            var findHandle = FindResource(module, name, type);
            if (findHandle == IntPtr.Zero)
                throw new Win32Exception("Unable to find resource", 
                    new Win32Exception(Marshal.GetLastWin32Error()));

            var size = (int)SizeofResource(module, findHandle);
            if (size == 0)
                throw new Win32Exception("Unable to get size of resource", 
                    new Win32Exception(Marshal.GetLastWin32Error()));

            var loadHandle = LoadResource(module, findHandle);
            if (loadHandle == IntPtr.Zero)
                throw new Win32Exception("Unable to load resource", 
                    new Win32Exception(Marshal.GetLastWin32Error()));

            var lockHandle = LockResource(loadHandle);
            if (lockHandle == IntPtr.Zero)
                throw new Win32Exception("Unable to lock resource", 
                    new Win32Exception(Marshal.GetLastWin32Error()));

            try
            {
                var result = new byte[size];

                Marshal.Copy(lockHandle, result, 0, size);

                return result;
            }
            catch (Exception ex)
            { 
                throw new Win32Exception("Unable to copy resource", ex); 
            }
        }
    }
}
