using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Common;

namespace Scr.Input
{
    partial class InputTracker
    {
        // aux user input info container
        class InputInfo : ICloneable
        {
            // p/invoke object
            private PInvoke.LastInputInfo info;

            // actual value
            private uint ticks;


            public DateTime DateTime
            {
                get { return new DateTime(ticks); }
            }


            public InputInfo()
            {
                info = PInvoke.LastInputInfo.Create();

                ticks = 0;
            }

            // initializes and actualizes new instance
            public static InputInfo Create()
            {
                var result = new InputInfo();

                result.Update();

                return result;
            }


            // compares instances values
            public override bool Equals(object obj)
            {
                return obj is InputInfo && ((InputInfo)obj).ticks == ticks;
            }

            public override int GetHashCode()
            {
                return (int)ticks;
            }

            public object Clone()
            {
                return new InputInfo { ticks = ticks };
            }

            // actualize instance value
            public void Update()
            {
                if (!PInvoke.GetLastInputInfo(ref info))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                ticks = info.Ticks;
            }
        }
    }
}
