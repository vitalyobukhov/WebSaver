using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace Screensaver
{
    static class WindowFinder
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr window, StringBuilder @class, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr window, StringBuilder @class, int count);

        private delegate bool EnumWindowsCallback(IntPtr window, IntPtr parameter);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowsCallback callback, IntPtr parameter);

        [StructLayout(LayoutKind.Sequential)]
        private struct FindData
        {
            public readonly string Class;
            public readonly string Text;


            public FindData(string @class = null, string text = null)
            {
                Class = @class ?? string.Empty;
                Text = text ?? string.Empty;
            }


            public bool Equals(string @class = null, string @text = null)
            {
                return (string.IsNullOrEmpty(Class) && string.IsNullOrEmpty(Text)) ||
                    (Class == @class && string.IsNullOrEmpty(Text)) ||
                    (string.IsNullOrEmpty(Class) && Text == @text) ||
                    (Class == @class && Text == @text);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FindOneParameter
        {
            public readonly FindData Data;
            public IntPtr Result;


            public FindOneParameter(string @class = null, string text = null)
            {
                Data = new FindData(@class, @text);
                Result = IntPtr.Zero;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FindAllParameter
        {
            public const int DefaultMaxResultCount = ushort.MaxValue;

            public readonly FindData Data;
            public IntPtr[] Result;


            public FindAllParameter(string @class = null, string text = null, int maxResultCount = DefaultMaxResultCount)
            {
                if (maxResultCount < 0)
                    throw new ArgumentOutOfRangeException("maxResultCount");

                Data = new FindData(@class, @text);
                Result = new IntPtr[maxResultCount];
            }
        }


        private static readonly EnumWindowsCallback findOneCallback = FindOneCallback;
        private static readonly EnumWindowsCallback findAllCallback = FindAllCallback;


        private static bool FindOneCallback(IntPtr window, IntPtr parameterPtr)
        {
            var @class = new StringBuilder(ushort.MaxValue);
            GetClassName(window, @class, @class.Capacity);

            var @text = new StringBuilder(ushort.MaxValue);
            GetWindowText(window, @text, @text.Capacity);

            var parameter = (FindOneParameter) Marshal.PtrToStructure(parameterPtr, typeof(FindOneParameter));
            if (parameter.Data.Equals(@class.ToString(), @text.ToString()))
            {
                parameter.Result = window;
                Marshal.StructureToPtr(parameter, parameterPtr, true);
                return false;
            }

            return true;
        }

        private static bool FindAllCallback(IntPtr window, IntPtr parameterPtr)
        {
            var @class = new StringBuilder(ushort.MaxValue);
            GetClassName(window, @class, @class.Capacity);

            var @text = new StringBuilder(ushort.MaxValue);
            GetWindowText(window, @text, @text.Capacity);

            var parameter = (FindAllParameter)Marshal.PtrToStructure(parameterPtr, typeof(FindAllParameter));
            if (parameter.Data.Equals(@class.ToString(), @text.ToString()))
            {
                var index = parameter.Result.
                    Select((p, i) => new KeyValuePair<int?, IntPtr>(i, p)).
                    FirstOrDefault(p => p.Value == IntPtr.Zero).Key;
                if (index.HasValue)
                {
                    parameter.Result.SetValue(window, index.Value);
                    Marshal.StructureToPtr(parameter, parameterPtr, true);
                }
            }

            return true;
        }

        public static IntPtr FindOne(IntPtr parentWindow, string @class = null, string text = null)
        {
            var parameter = new FindOneParameter(@class, @text);
            var parameterPtr = IntPtr.Zero;

            try
            {
                parameterPtr = Marshal.AllocHGlobal(Marshal.SizeOf(parameter));
                Marshal.StructureToPtr(parameter, parameterPtr, false);

                EnumChildWindows(parentWindow, findOneCallback, parameterPtr);

                parameter = (FindOneParameter) Marshal.PtrToStructure(parameterPtr, typeof (FindOneParameter));
            }
            catch { }
            finally
            {
                try
                {
                    if (parameterPtr != IntPtr.Zero)
                        Marshal.DestroyStructure(parameterPtr, typeof(FindOneParameter));
                }
                catch { }
            }

            return parameter.Result;
        }

        public static IEnumerable<IntPtr> FindAll(IntPtr parentWindow, string @class = null, string text = null, 
            int maxResultCount = FindAllParameter.DefaultMaxResultCount)
        {
            var parameter = new FindAllParameter(@class, @text, maxResultCount);
            var parameterPtr = IntPtr.Zero;

            try
            {
                parameterPtr = Marshal.AllocHGlobal(Marshal.SizeOf(parameter));
                Marshal.StructureToPtr(parameter, parameterPtr, false);

                EnumChildWindows(parentWindow, findAllCallback, parameterPtr);

                parameter = (FindAllParameter)Marshal.PtrToStructure(parameterPtr, typeof(FindAllParameter));
            }
            catch { }
            finally
            {
                try
                {
                    if (parameterPtr != IntPtr.Zero)
                        Marshal.DestroyStructure(parameterPtr, typeof(FindAllParameter));
                }
                catch { }
            }

            return parameter.Result.Where(r => r != IntPtr.Zero).ToArray();
        }
    }
}
