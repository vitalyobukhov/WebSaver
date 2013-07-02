using System;

namespace Scr.Content
{
    sealed class ExtractContentArchiveException : LoadContentException
    {
        public enum Details
        {
            Unknown,
            Archive,
            Path,
            Directory,
            Extract
        }


        public Details Reason { get; private set; }

        public override string FriendlyMessage
        {
            get
            {
                switch (Reason)
                {
                    case Details.Archive:
                        return Localization.ExtractContentArchiveArchiveMessage;

                    case Details.Path:
                    case Details.Directory:
                    case Details.Extract:
                        return Localization.ExtractContentArchiveOperationsMessage;

                    default:
                        return Localization.ExtractContentArchiveUnknownMessage;
                }
            }
        }


        public ExtractContentArchiveException(Details reason)
        { 
            Reason = reason; 
        }

        public ExtractContentArchiveException(Details reason, string message) :
            base(message)
        { 
            Reason = reason; 
        }

        public ExtractContentArchiveException(Details reason, string message, Exception innerException) :
            base(message, innerException)
        { 
            Reason = reason; 
        }
    }
}
