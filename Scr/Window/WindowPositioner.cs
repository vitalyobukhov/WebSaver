using System;
using Common;
using System.Windows.Forms;
using System.Drawing;

namespace Scr.Window
{
    // window related p/invoke wrapper
    static class WindowPositioner
    {
        // docks window into parent one
        public static void DockAsChild(Control control, IntPtr parent)
        {
            // set window parent
            if (PInvoke.SetParent(control.Handle, parent) == IntPtr.Zero)
                throw new DockAsChildException(Localization.DockAsChildMessage, 
                    new InvalidOperationException(Localization.DockAsChildSetParentMessage,
                        PInvoke.GetLastWin32Exception()));

            // set window as child
            if (!PInvoke.SetWindowChild(control.Handle))
                throw new DockAsChildException(Localization.DockAsChildMessage,
                    new InvalidOperationException(Localization.DockAsChildSetWindowChildMessage,
                        PInvoke.GetLastWin32Exception()));

            // set window dimensions
            PInvoke.Rectangle parentRectangle;
            if (!PInvoke.GetClientRect(parent, out parentRectangle))
                throw new DockAsChildException(Localization.DockAsChildMessage,
                    new InvalidOperationException(Localization.DockAsChildGetClientRectMessage,
                        PInvoke.GetLastWin32Exception()));

            try
            {
                control.Size = new Size(parentRectangle.Width, parentRectangle.Height);
                control.Location = new Point(0, 0);
            }
            catch (Exception ex)
            {
                throw new DockAsChildException(Localization.DockAsChildMessage,
                    new InvalidOperationException(Localization.DockAsChildSizeLocationMessage, ex));
            }
        }
    }
}
