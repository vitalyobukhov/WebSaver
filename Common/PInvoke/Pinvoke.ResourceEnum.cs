using System;
using System.Runtime.InteropServices;
using System.Linq;
using System.ComponentModel;

namespace Common
{
    public static partial class PInvoke
    {
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms648034(v=vs.85).aspx
        public delegate bool EnumResourceNamesProcedure(IntPtr module, ushort type, 
            ushort name, IntPtr parameter);


        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms681386(v=vs.85).aspx
        private enum SystemErrorCode
        {
            ERROR_RESOURCE_TYPE_NOT_FOUND = 1813
        }


        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms648037(v=vs.85).aspx
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumResourceNames(IntPtr module, uint type, 
            EnumResourceNamesProcedure procedure, IntPtr parameter);


        // parameter for EnumResourceNames callback with results
        [StructLayout(LayoutKind.Sequential)]
        private struct GetResourceNamesParameter
        {
            public const int MaxResultCount = ushort.MaxValue + 1;


            // found resource names count
            public int ResultLength;

            // found resource names
            public readonly ushort[] Result;


            public GetResourceNamesParameter(int resultCount)
            {
                ResultLength = 0;
                Result = new ushort[resultCount];
            }
        }


        // EnumResourceNames callback implementation
        private static bool GetResourceNamesCallback(IntPtr module, ushort type, 
            ushort name, IntPtr parameterPtr)
        {
            var parameter = (GetResourceNamesParameter)Marshal.
                PtrToStructure(parameterPtr, typeof(GetResourceNamesParameter));
            parameter.Result[parameter.ResultLength++] = name;
            Marshal.StructureToPtr(parameter, parameterPtr, true);

            return true;
        }

        // finds resource names within provided module and using provided resource type
        public static ushort[] GetResourceNames(IntPtr module, uint type)
        {
            var parameter = new GetResourceNamesParameter(GetResourceNamesParameter.MaxResultCount);
            var parameterPtr = IntPtr.Zero;

            try
            {
                parameterPtr = Marshal.AllocHGlobal(Marshal.SizeOf(parameter));
                Marshal.StructureToPtr(parameter, parameterPtr, false);

                if (!EnumResourceNames(module, type, GetResourceNamesCallback, parameterPtr))
                {
                    var error = Marshal.GetLastWin32Error();
                    if (error != (int)SystemErrorCode.ERROR_RESOURCE_TYPE_NOT_FOUND)
                        throw new Win32Exception(error);
                }

                parameter = (GetResourceNamesParameter)Marshal.
                    PtrToStructure(parameterPtr, typeof(GetResourceNamesParameter));

                return parameter.Result.Take(parameter.ResultLength).ToArray();
            }
            catch (Exception ex)
            { 
                throw new Win32Exception("Unable to get resource names", ex); 
            }
            finally
            {
                try
                {
                    if (parameterPtr != IntPtr.Zero)
                        Marshal.DestroyStructure(parameterPtr, typeof(GetResourceNamesParameter));
                }
                catch { }
            }
        }
    }
}
