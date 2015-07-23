using System;

namespace ExtendedEnum
{
    public class DuplicateValueException : Exception
    {
        public DuplicateValueException(string message, Exception innerException) : base(message, innerException)
        { }
        public DuplicateValueException(string message) : base(message)
        { }
        public DuplicateValueException()
        { }
    }
}