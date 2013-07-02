using System;
using System.Runtime.InteropServices;

namespace Common
{
    // window related p/invoke definitions
    public static partial class PInvoke
    {
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633585(v=vs.85).aspx
        private enum WindowLongOffset : int
        {
            GWL_STYLE = -16
        }

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms632600(v=vs.85).aspx
        private enum WindowStyle : int
        {
            WS_CHILD = 0x40000000
        }

        // http://msdn.microsoft.com/en-us/library/windows/desktop/dd162897(v=vs.85).aspx
        [StructLayout(LayoutKind.Sequential)]
        public struct Rectangle
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;


            public int Width
            {
                get { return Right - Left; }
            }

            public int Height
            {
                get { return Bottom - Top; }
            }
        }


        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633584(v=vs.85).aspx
        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetWindowLong32(IntPtr window, int offset);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633585(v=vs.85).aspx
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetWindowLong64(IntPtr window, int offset);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633591(v=vs.85).aspx
        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowLong32(IntPtr window, int offset, IntPtr value);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms644898(v=vs.85).aspx
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowLong64(IntPtr window, int offset, IntPtr value);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633541(v=vs.85).aspx
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr window, IntPtr parent);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633503(v=vs.85).aspx
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetClientRect(IntPtr window, out Rectangle rectangle);


        // determines platform word length
        private static bool IsLong32
        {
            get { return IntPtr.Size == 4; }
        }

        // platform-independent GetWindowLong wrapper
        private static IntPtr GetWindowLong(IntPtr window, WindowLongOffset offset)
        {
            return IsLong32 ? 
                GetWindowLong32(window, (int)offset) :
                GetWindowLong64(window, (int)offset);
        }

        // platform-independent SetWindowLong wrapper
        private static IntPtr SetWindowLong(IntPtr window, WindowLongOffset offset, IntPtr value)
        {
            return IsLong32 ?
                SetWindowLong32(window, (int)offset, value) :
                SetWindowLong64(window, (int)offset, value);
        }

        // set window style as child one
        public static bool SetWindowChild(IntPtr window)
        {
            var oldValue = GetWindowLong(window, WindowLongOffset.GWL_STYLE);
            var newValue = new IntPtr(IsLong32 ?
                oldValue.ToInt32() | (int)WindowStyle.WS_CHILD :
                oldValue.ToInt64() | (long)WindowStyle.WS_CHILD);

            return SetWindowLong(window, WindowLongOffset.GWL_STYLE, newValue) != IntPtr.Zero;
        }
    }
}
