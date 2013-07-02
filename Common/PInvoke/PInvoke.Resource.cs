namespace Common
{
    public static partial class PInvoke
    {
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms648009(v=vs.85).aspx
        public enum ResourceType : ushort
        {
            ICON = 3,
            STRING = 6,
            GROUP_ICON = ICON + 11
        }
    }
}
