using System;
using System.Linq;

namespace Common
{
    // base class of application startup arguments parser
    public abstract class StartupArgs
    {
        // key arg can start with these symbols
        private static readonly char[] keyPrefixes = { '/', '\\' };

        // should parser validate key arg
        private readonly bool useKey;

        // should parser validate args count
        private readonly bool useMinOptionsCount;


        // specifies valid key symbol if useKey == true
        protected abstract char Key { get; }

        // specifies min additional args count except key if useMinOptionsCount == true
        protected abstract uint MinOptionsCount { get; }


        // initializes parser using given parse options
        protected StartupArgs(bool useKey = true, bool useMinOptionsCount = true)
        {
            this.useKey = useKey;
            this.useMinOptionsCount = useMinOptionsCount;
        }


        // additional parsing logic for child classes
        protected abstract bool InnerParse(string[] args);

        // common parsing logic for arguments
        public bool Parse(string[] args)
        {
            if (args == null)
                return false;

            // validate first arg as key symbol
            if (useKey)
            {
                if (args.Length == 0 || args[0] == null)
                    return false;

                var firstArg = args[0].ToLower().TrimStart(keyPrefixes);
                if (firstArg[0] != Key)
                    return false;
            }

            // validate min rest args count
            var restArgs = args.Skip(useKey ? 1 : 0).Take((int)MinOptionsCount).ToArray();
            if (useMinOptionsCount)
            {
                if (restArgs.Length < MinOptionsCount || restArgs.Any(a => a == null))
                    return false;
            }

            // call child class parsing logic
            return InnerParse(restArgs);
        }
    }


    // wrapper with parsing sequence invocation logic for defined application startup arguments
    public static class StartupArgsHandler
    {
        public static StartupArgs Parse(string[] arguments, StartupArgs[] args)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            if (args == null)
                throw new ArgumentNullException("args");

            // find first successfully parsed arg
            return args.FirstOrDefault(a => a.Parse(arguments));
        }

        public static StartupArgs Parse(string[] arguments, StartupArgs[] args, 
            StartupArgs @default)
        {
            if (@default == null)
                throw new ArgumentNullException("default");

            // return default arg if no successfully parsed arg was found
            return Parse(arguments, args) ?? @default;
        }

        public static StartupArgs Parse(string[] arguments, StartupArgs[] args, 
            Func<string[], StartupArgs> resolveDefault)
        {
            if (resolveDefault == null)
                throw new ArgumentNullException("resolveDefault");

            // return resolver result if no successfully parsed arg was found
            return Parse(arguments, args) ?? resolveDefault(arguments);
        }
    }
}
