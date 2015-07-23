using System;
using System.Runtime.Serialization;

namespace ExtendedEnum
{
    [Serializable]
    public class ExtendedEnumDeserializationProxy : IObjectReference, ISerializable
    {
        // This is a plumbing class for handling the reference-deserialization magic of ExtendedEnumeration
        private readonly Type _enumType;
        private readonly int _enumValue;
        public static string EnumTypeString = nameof(_enumType);
        public static string EnumValueString = nameof(_enumValue);
        protected ExtendedEnumDeserializationProxy(SerializationInfo info, StreamingContext context)
        {
            _enumValue = info.GetInt32(EnumValueString);
            _enumType = Type.GetType(info.GetString(EnumTypeString));
        }
        public object GetRealObject(StreamingContext context)
        {
            return ExtendedEnumeration.FromValue(_enumType, _enumValue);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotSupportedException("This class shouldn't be serialized directly - it is plumbing for the ExtendedEnumeration class.");
        }
    }
}