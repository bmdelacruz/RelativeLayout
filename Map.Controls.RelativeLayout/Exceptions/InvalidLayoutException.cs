using System;

namespace Map.Controls.Exceptions
{
    public class InvalidLayoutException : Exception
    {
        public InvalidLayoutException(string message) : base(message) { }
    }
}
