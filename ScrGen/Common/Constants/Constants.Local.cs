using System.Text;

namespace Common
{
    // local project constants
    public static partial class Constants
    {
        // screensaver module resource name
        public const ushort ModuleResourceName = 0;

        // screensaver module resource type
        public const ushort ModuleResourceType = 42;

        // pe main group icon resource name
        public const ushort MainGroupIconName = 32512;

        // screensaver output extension
        public const string OutputExtension = ".SCR";

        // caption file default extension
        public const string CaptionExtension = ".txt";

        // icon file default extension
        public const string IconExtension = ".ico";

        // default caption file name
        public const string DefaultCaptionFilename = "caption" + CaptionExtension;

        // default icon file name
        public const string DefaultIconFilename = "icon" + IconExtension;

        // caption file text encoding
        public static readonly Encoding CaptionEncoding = Encoding.UTF8;
    }
}
