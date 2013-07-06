using System;
using System.IO;
using System.Linq;
using Common;

namespace ScrGen
{
    // base args interface with output path
    interface IUpdateArgs
    {
        string OutputPath { get; }
    }

    // args with web content directory path
    interface IUpdateContentArgs : IUpdateArgs
    {
        string ContentPath { get; }
    }

    // args with caption file path
    interface IUpdateCaptionArgs : IUpdateArgs
    {
        string CaptionPath { get; }
    }

    // args with icon file path 
    interface IUpdateIconArgs : IUpdateArgs
    {
        string IconPath { get; }
    }

    // base args class with content and output path
    abstract class UpdateArgs : StartupArgs
    {
        protected override char Key
        {
            get { throw new InvalidOperationException("Key is not used"); }
        }

        protected override uint MinOptionsCount
        {
            get { return 2; }
        }

        // web content directory path
        public string ContentPath { get; private set; }

        // output screensaver path
        public string OutputPath { get; protected set; }


        protected UpdateArgs() :
            base(false, true)
        { }


        // children classes should parse output screensaver path
        protected bool TryParseOutputPath(string arg)
        {
            try
            {
                OutputPath = arg.Substring(0, arg.Length - Path.GetExtension(arg).Length) + Constants.OutputExtension;
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override bool InnerParse(string[] args)
        {
 	        ContentPath = args[0];

            return true;
        }
    }


    // args with content path only
    class InjectContentArgs : UpdateArgs,
        IUpdateContentArgs
    {
        protected override uint MinOptionsCount
        {
            get { return base.MinOptionsCount; }
        }


        protected override bool InnerParse(string[] args)
        {
            return base.InnerParse(args) && TryParseOutputPath(args[1]);
        }
    }

    // args with content and caption paths
    sealed class InjectContentCaptionArgs : UpdateArgs,
        IUpdateContentArgs,
        IUpdateCaptionArgs
    {
        protected override uint MinOptionsCount
        {
            get { return base.MinOptionsCount + 1; }
        }

        // caption file path
        public string CaptionPath { get; private set; }


        protected override bool InnerParse(string[] args)
        {
            var result = base.InnerParse(args) && TryParseOutputPath(args[2]);

            if (result)
            {
                CaptionPath = args[1];
            }

            return result;
        }
    }

    // args with content and icon paths
    sealed class InjectContentIconArgs : UpdateArgs,
        IUpdateContentArgs,
        IUpdateIconArgs
    {
        protected override uint MinOptionsCount
        {
            get { return base.MinOptionsCount + 1; }
        }

        // icon file path
        public string IconPath { get; private set; }


        protected override bool InnerParse(string[] args)
        {
            var result = base.InnerParse(args) && TryParseOutputPath(args[2]);

            if (result)
            {
                IconPath = args[1];
            }

            return result;
        }
    }

    // args with content, caption and icon paths
    sealed class InjectContentCaptionIconArgs : UpdateArgs,
        IUpdateContentArgs,
        IUpdateCaptionArgs,
        IUpdateIconArgs
    {
        protected override uint MinOptionsCount
        {
            get { return base.MinOptionsCount + 2; }
        }

        // caption file path
        public string CaptionPath { get; private set; }

        // icon file path
        public string IconPath { get; private set; }


        protected override bool InnerParse(string[] args)
        {
            var result = base.InnerParse(args) && TryParseOutputPath(args[3]);

            if (result)
            {
                CaptionPath = args[1];
                IconPath = args[2];
            }

            return result;
        }
    }

    // args with content and default caption, icon paths
    sealed class InjectContentCaptionIconLatentArgs : InjectContentArgs,
        IUpdateCaptionArgs,
        IUpdateIconArgs
    {
        public string CaptionPath { get; private set; }
        public string IconPath { get; private set; }


        // try to get caption and icon pths using parent directory path
        private static bool TryGetSourcePaths(string parentPath,
            out string captionPath, out string iconPath)
        {
            captionPath = Path.Combine(parentPath, Constants.DefaultCaptionFilename);
            iconPath = Path.Combine(parentPath, Constants.DefaultIconFilename);

            var result = File.Exists(captionPath) && File.Exists(iconPath);

            if (!result)
            {
                captionPath = null;
                iconPath = null;
            }

            return result;
        }

        protected override bool InnerParse(string[] args)
        {
            var result = base.InnerParse(args);

            if (result)
            {
                string captionPath, iconPath, parentPath = null;
                try
                {
                    parentPath = Directory.GetParent(ContentPath).FullName;
                }
                catch { }

                // try parent directory first, then content one
                result = (parentPath != null && 
                        TryGetSourcePaths(parentPath, out captionPath, out iconPath)) ||
                    TryGetSourcePaths(ContentPath, out captionPath, out iconPath);

                if (result)
                {
                    CaptionPath = captionPath;
                    IconPath = iconPath;
                }
            }

            return result;
        }
    }


    // application args custom resolver
    static class UpdateArgsResolver
    {
        private static bool ReviseInjectContentArgs(string[] arguments)
        {
            return true;
        }

        private static bool ReviseInjectContentCaptionIconLatentArgs(string[] arguments)
        {
            return true;
        }

        private static bool ReviseInjectContentCaptionArgs(string[] arguments)
        {
            return arguments[1].EndsWith(Constants.CaptionExtension);
        }

        private static bool ReviseInjectContentIconArgs(string[] arguments)
        {
            return arguments[1].EndsWith(Constants.IconExtension);
        }

        private static bool ReviseInjectContentCaptionIconArgs(string[] arguments)
        {
            return true;
        }

        // resolve args from string using internal mapping logic
        public static UpdateArgs Resolve(string[] arguments)
        {
            var resolvers = new []
            {
                new Tuple<UpdateArgs, Func<string[], bool>>
                    (new InjectContentCaptionIconArgs(), ReviseInjectContentCaptionIconArgs),
                new Tuple<UpdateArgs, Func<string[], bool>>
                    (new InjectContentIconArgs(), ReviseInjectContentIconArgs),
                new Tuple<UpdateArgs, Func<string[], bool>>
                    (new InjectContentCaptionArgs(), ReviseInjectContentCaptionArgs),
                new Tuple<UpdateArgs, Func<string[], bool>>
                    (new InjectContentCaptionIconLatentArgs(), ReviseInjectContentCaptionIconLatentArgs),
                new Tuple<UpdateArgs, Func<string[], bool>>
                    (new InjectContentArgs(), ReviseInjectContentArgs)
            };

            // resolved args should be parsed and revised via related logic
            return 
                (from resolver 
                 in resolvers 
                 let args = resolver.Item1 
                 let revise = resolver.Item2 
                 where args.Parse(arguments) && revise(arguments) 
                 select args).FirstOrDefault();
        }
    }
}
