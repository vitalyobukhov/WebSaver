using Common;

namespace Scr
{
    // run screensaver 
    sealed class RunArgs : StartupArgs
    {
        protected override char Key
        {
            get { return 's'; }
        }

        protected override uint MinOptionsCount
        {
            get { return 0; }
        }

        protected override bool InnerParse(string[] args)
        {
            return true;
        }
    }

    // configure screensaver
    sealed class ConfigureArgs : StartupArgs
    {
        protected override char Key
        {
            get { return 'c'; }
        }

        protected override uint MinOptionsCount
        {
            get { return 0; }
        }

        protected override bool InnerParse(string[] args)
        {
            return true;
        }
    }

    // preview screensaver
    sealed class PreviewArgs : StartupArgs
    {
        protected override char Key
        {
            get { return 'p'; }
        }

        protected override uint MinOptionsCount
        {
            get { return 1; }
        }

        // first arg is parent window handle to embed screensaver window as child one
        public int ParentWindowHandle { get; private set; }


        protected override bool InnerParse(string[] args)
        {
            int outHandle;

            var result = int.TryParse(args[0], out outHandle);

            if (result)
                ParentWindowHandle = outHandle;

            return result;
        }
    }
}
