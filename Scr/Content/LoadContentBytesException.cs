using System;

namespace Scr.Content
{
    sealed class LoadContentBytesException : LoadContentException
    {
        public enum Details
        {
            Unknown,
            FindResource,
            SizeofResource,
            LoadResource,
            LockResource,
            CopyBytes
        }


        public Details Reason { get; private set; }

        public override string FriendlyMessage
        {
            get
            {
                switch (Reason)
                {
                    case Details.FindResource:
                        return Localization.LoadContentBytesFindResourceMessage;

                    case Details.SizeofResource:
                    case Details.LoadResource:
                    case Details.LockResource:
                        return Localization.LoadContentBytesOperationsMessage;

                    case Details.CopyBytes:
                        return Localization.LoadContentBytesCopyBytesMessage;

                    default:
                        return Localization.LoadContentBytesUnknownMessage;
                }
            }
        }


        public LoadContentBytesException(Details reason)
        { 
            Reason = reason; 
        }

        public LoadContentBytesException(Details reason, string message) :
            base(message)
        { 
            Reason = reason; 
        }

        public LoadContentBytesException(Details reason, string message, Exception innerException) :
            base(message, innerException)
        { 
            Reason = reason; 
        }
    }
}
