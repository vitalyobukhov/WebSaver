using System;

namespace Scr.Window
{
    sealed class DockAsChildException : Exception
    {
        public DockAsChildException()
        { }

        public DockAsChildException(string message) :
            base(message)
        { }

        public DockAsChildException(string message, Exception innerException) :
            base(message, innerException)
        { }
    }
}
