using Common;

namespace ResUpd
{
    // base class for operation args
    abstract class OperationArgs : StartupArgs
    {
        protected override uint MinOptionsCount
        {
            get { return 4; }
        }

        // source pe path
        public string SourcePath { get; protected set; }

        // output pe path
        public string OutputPath { get; protected set; }

        // pe resource type
        public ushort ResourceType { get; protected set; }

        // pe resource name
        public ushort ResourceName { get; protected set; }


        protected  OperationArgs() :
            base(true, true)
        { }


        // common parsing logic
        protected override bool InnerParse(string[] args)
        {
            ushort outType = 0,
                outName = 0;

            var result = ushort.TryParse(args[2], out outType) &&
                ushort.TryParse(args[3], out outName);

            if (result)
            {
                SourcePath = args[0];
                OutputPath = args[1];
                ResourceType = outType;
                ResourceName = outName;
            }

            return result;
        }
    }

    // extract resource
    sealed class ExtractArgs : OperationArgs
    {
        protected override char Key
        {
            get { return 'e'; }
        }
    }

    // add or update resource
    sealed class InjectArgs : OperationArgs
    {
        protected override char Key
        {
            get { return 'i'; }
        }

        protected override uint MinOptionsCount
        {
            get { return base.MinOptionsCount + 1; }
        }

        // embedding resource path
        public string ResourcePath { get; private set; }


        protected override bool InnerParse(string[] args)
        {
            var result = base.InnerParse(args);

            if (result)
                ResourcePath = args[4];

            return result;
        }
    }

    // delete resource
    sealed class DeleteArgs : OperationArgs
    {
        protected override char Key
        {
            get { return 'd'; }
        }
    }
}
