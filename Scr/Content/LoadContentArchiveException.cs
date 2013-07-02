using System;

namespace Scr.Content
{
    sealed class LoadContentArchiveException : LoadContentException
    {
        public enum Details
        {
            Unknown,
            Bytes,
            Stream,
            Archive
        }


        public Details Reason { get; private set; }

        public override string FriendlyMessage
        {
            get
            {
                switch (Reason)
                {
                    case Details.Bytes:
                        return Localization.LoadContentArchiveBytesMessage;

                    case Details.Stream:
                    case Details.Archive:
                        return Localization.LoadContentArchiveOperationsMessage;

                    default:
                        return Localization.LoadContentArchiveUnknownMessage;
                }
            }
        }


        public LoadContentArchiveException(Details reason)
        { 
            Reason = reason; 
        }

        public LoadContentArchiveException(Details reason, string message) :
            base(message)
        { 
            Reason = reason; 
        }

        public LoadContentArchiveException(Details reason, string message, Exception innerException) :
            base(message, innerException)
        { 
            Reason = reason; 
        }
    }
}
