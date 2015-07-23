using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Shouldly;

namespace ExtendedEnum.Tests
{
    public class SerializationTests
    {
        [Test]
        public void SerializedValue_ShouldNotContainAllObjectProperties()
        {
            var stream = SerializeToStream(Something.One);
            string serialized;
            using (var sr = new StreamReader(stream)) serialized = sr.ReadToEnd();
            serialized.ShouldNotContain("One");
        }

        [Test]
        public void Deserialize_Works()
        {
            var stream = SerializeToStream(Something.One);
            var deserialized = DeserializeFromStream(stream);
            deserialized.ShouldBeOfType<Something>();
            ((Something) deserialized).Description.ShouldBe("One");
        }

        [Test]
        public void SerializeThenDeserialize_ShouldPreserveReferenceEquality()
        {
            var stream = SerializeToStream(Something.Two);
            var deserialized = DeserializeFromStream(stream);

            deserialized.ShouldBeSameAs(Something.Two);
        }

        [Serializable]
        private class Something : ExtendedEnumeration
        {
            public static readonly Something One = new Something(1, "One");
            public static readonly Something Two = new Something(2, "Two");

            public readonly string Description;

            private Something(int value, string description) : base(value)
            {
                Description = description;
            }
            protected Something(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                throw new NotImplementedException();             
            }
        }

        public static MemoryStream SerializeToStream(object o)
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);
            return stream;
        }

        public static object DeserializeFromStream(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object o = formatter.Deserialize(stream);
            return o;
        }
    }
}