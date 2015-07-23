using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace ExtendedEnum.Tests
{
    public class NewtonsoftTests
    {
        [Test]
        public void SerializedValue_ShouldNotContainAllObjectProperties()
        {
            var serialised = JsonConvert.SerializeObject(Something.Two);
            serialised.ShouldNotContain("Two");
        }

        [Test]
        public void Deserialize_Works()
        {
            var serialised = JsonConvert.SerializeObject(Something.One);
            var deserialised = JsonConvert.DeserializeObject<Something>(serialised);
            deserialised.ShouldBeOfType<Something>();
            deserialised.Description.ShouldBe("One");
        }

        [Test]
        public void SerializeThenDeserialize_ShouldPreserveReferenceEquality()
        {
            var serialised = JsonConvert.SerializeObject(Something.One);
            var deserialised = JsonConvert.DeserializeObject<Something>(serialised);
            deserialised.ShouldBeSameAs(Something.One);
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
    }
}