using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ExtendedEnum
{
    [Serializable]
    [JsonConverter(typeof(ExtendedEnumNewtonsoftConverter))]
    public abstract class ExtendedEnumeration : ISerializable, IComparable
    { 
        // Based on http://lostechies.com/jimmybogard/2008/08/12/enumeration-classes/ - See that page for some neat usage examples
        private readonly int _value;
        public int Value => _value;

        private void EnsureUniqueness()
        {
            var allOptions = GetEntries(GetType()).Cast<ExtendedEnumeration>().Concat(new[] { this });
            var dups = allOptions.GroupBy(entry => entry._value).Where(g => g.Count() > 1).ToArray();
            if (dups.Any())
            {
                throw new DuplicateValueException($"Enumeration {GetType()} contains duplicate values {string.Join(", ", dups.SelectMany(n => n))}");
            }
        }

        protected ExtendedEnumeration(int value)
        {
            _value = value;
            EnsureUniqueness();
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static IEnumerable<T> GetEntries<T>() where T : ExtendedEnumeration
        {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields.Select(info => info.GetValue(null)).OfType<T>();
        }

        public static IEnumerable GetEntries(Type type)
        {
            if (!typeof(ExtendedEnumeration).IsAssignableFrom(type)) throw new Exception("Supplied type is not an ExtendedEnumeration.");
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields.Select(info => info.GetValue(null)).Where(locatedValue => locatedValue != null);
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as ExtendedEnumeration;

            if (otherValue == null)
            {
                return false;
            }

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = _value.Equals(otherValue._value);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            return Value.CompareTo(obj);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.SetType(typeof(ExtendedEnumDeserializationProxy)); // Tell serializer to use the proxy class instead
            info.AddValue(ExtendedEnumDeserializationProxy.EnumTypeString, GetType().AssemblyQualifiedName);
            info.AddValue(ExtendedEnumDeserializationProxy.EnumValueString, _value);
        }

        protected ExtendedEnumeration(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException(
                "Deserialization of this type should occur via ExtendedEnumDeserializationProxy, not directly.");
        }

        public static T FromValue<T>(int value) where T : ExtendedEnumeration
        {
            var matchingItem = Parse<T, int>(value, nameof(Value), item => item._value.Equals(value));
            return matchingItem;
        }

        public static object FromValue(Type type, int value)
        {
            if (!typeof(ExtendedEnumeration).IsAssignableFrom(type)) throw new Exception("Supplied type is not an ExtendedEnumeration.");
            var allItems = GetEntries(type);
            return allItems.Cast<ExtendedEnumeration>().FirstOrDefault(item => item._value.Equals(value));
        }

        protected static TEnum Parse<TEnum, TValue>(TValue value, string description, Func<TEnum, bool> predicate) where TEnum : ExtendedEnumeration
        {
            var matchingItem = GetEntries<TEnum>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                var message = $"'{value}' is not a valid {description} in {typeof (TEnum)}";
                throw new MissingValueException(message);
            }

            return matchingItem;
        }
    }

    public class ExtendedEnumTypeConverter<TEnumType> : TypeConverter where TEnumType : ExtendedEnumeration
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof (string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
                int intValue;
                if (int.TryParse(stringValue, out intValue))
                {
                    return ExtendedEnumeration.FromValue<TEnumType>(intValue);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }

    public class ExtendedEnumNewtonsoftConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((ExtendedEnumeration)value).Value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = Convert.ToInt32(reader.Value);
            return ExtendedEnumeration.FromValue(objectType, (int) value);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (ExtendedEnumeration).IsAssignableFrom(objectType);
        }
    }
}
