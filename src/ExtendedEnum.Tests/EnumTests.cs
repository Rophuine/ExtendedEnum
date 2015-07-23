using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace ExtendedEnum.Tests
{
    public class EnumTests
    {
        [Test]
        public void GetEntries_Works()
        {
            var entries = ExtendedEnumeration.GetEntries<Something>().ToList();
            entries.Count().ShouldBe(2);
            entries.Single(e => e.Value == 1).Description.ShouldBe("One");
            entries.Single(e => e.Value == 2).Description.ShouldBe("Two");
        }

        [Test]
        public void GetByValue_Works()
        {
            ((Something) ExtendedEnumeration.FromValue(typeof (Something), 1)).Description.ShouldBe("One");
            ExtendedEnumeration.FromValue<Something>(2).Description.ShouldBe("Two");
        }

        [Test]
        [ExpectedException(typeof(MissingValueException))]
        public void GetByValue_Throws_WhenValueDoesNotExist()
        {
            ExtendedEnumeration.FromValue<Something>(3);
        }

        private class Something : ExtendedEnumeration
        {
            public static Something One = new Something(1, "One");
            public static Something Two = new Something(2, "Two");

            public readonly string Description;

            private Something(int value, string description) : base(value)
            {
                Description = description;
            }
        }
    }
}