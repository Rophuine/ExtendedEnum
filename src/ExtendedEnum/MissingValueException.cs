using System;

namespace ExtendedEnum
{
    public class MissingValueException : Exception
    {
        public MissingValueException(string message, Exception innerException) : base(message, innerException)
        { }
        public MissingValueException(string message) : base(message)
        { }
        public MissingValueException()
        { }
    }
}