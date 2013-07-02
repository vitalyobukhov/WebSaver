using System;

namespace Scr.Content
{
    sealed class CreateContentException : LoadContentException
    {
        public override string FriendlyMessage
        {
            get { return Localization.ContentCreateMessage; }
        }

        public CreateContentException()
        { }

        public CreateContentException(string message) :
            base(message)
        { }

        public CreateContentException(string message, Exception innerException) :
            base(message, innerException)
        { }
    }
}
