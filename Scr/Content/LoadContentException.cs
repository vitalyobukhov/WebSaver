using System;

namespace Scr.Content
{
    abstract class LoadContentException : Exception
    {
        public abstract string FriendlyMessage { get; }


        protected LoadContentException()
        { }

        protected LoadContentException(string message) :
            base(message)
        { }

        protected LoadContentException(string message, Exception innerException) :
            base(message, innerException)
        { }
    }
}
