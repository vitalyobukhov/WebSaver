using System.Runtime.InteropServices;

namespace Common
{
    public static partial class PInvoke
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct LastInputInfo
        {
            public uint Size;
            public uint Ticks;


            public static LastInputInfo Create()
            {
                var result = new LastInputInfo();
                result.Size = (uint)Marshal.SizeOf(result);
                return result;
            }
        }

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms646302(v=vs.85).aspx
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetLastInputInfo(ref LastInputInfo info);
    }
}
