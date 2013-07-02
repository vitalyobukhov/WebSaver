using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Common
{
    public static partial class PInvoke
    {
        // wrapper for unmanaged resource with related pointer and dispose method
        public sealed class DisposableHandle : IDisposable
        {
            private bool disposed;

            // cancel ptr dispose
            private bool suppressDispose;

            // unmanaged resource ptr
            private readonly IntPtr handle;

            // dispose method for resource ptr
            private readonly Func<IntPtr, bool> dispose;

            // supress dispose exceptions
            private readonly bool silentDispose;


            public IntPtr Handle
            {
                get
                {
                    if (disposed)
                        throw new ObjectDisposedException("Resource");

                    return handle;
                }
            }


            public DisposableHandle(IntPtr handle, Func<IntPtr, bool> dispose, 
                bool silentDispose = true)
            {
                if (handle == IntPtr.Zero)
                    throw new ArgumentNullException("handle");

                if (dispose == null)
                    throw new ArgumentNullException("dispose");

                disposed = false;
                suppressDispose = false;
                this.handle = handle;
                this.dispose = dispose;
                this.silentDispose = silentDispose;
            }

            ~DisposableHandle()
            {
                Dispose(false);
            }


            private void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (!suppressDispose)
                    {
                        if (disposing)
                        { }

                        if (!dispose(handle) && !silentDispose)
                            throw new Win32Exception("Unable to dispose resource",
                                new Win32Exception(Marshal.GetLastWin32Error()));
                    }

                    disposed = true;
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void SuppressDispose()
            {
                suppressDispose = true;
            }
        }


        // last error accessor
        public static Exception GetLastWin32Exception()
        {
            return new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
