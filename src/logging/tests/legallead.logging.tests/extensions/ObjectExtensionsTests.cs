using Bogus;
using legallead.logging.extensions;
using legallead.logging.tests.testobj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.logging.tests.extensions
{
    public class ObjectExtensionsTests
    {
        private static readonly Faker faker = new();

        [Fact]
        public void ObjCanSplit()
        {
            var excpetion = Record.Exception(() =>
            {
                var obj = faker.Lorem.Sentences(10);
                _ = obj.SplitByLength();
            });
            Assert.Null(excpetion);
        }

        [Theory]
        [InlineData(0, 50)]
        [InlineData(5, 15)]
        [InlineData(75, 25)]
        public void ObjCanTruncate(int length, int maxLength)
        {
            var test = length == 0 ? string.Empty : faker.Random.AlphaNumeric(length);
            var truncated = test.Truncate(maxLength);
            var actual = truncated.Length;
            Assert.True(actual <= maxLength);
        }

        [Theory]
        [InlineData(0, "Temp", false)]
        [InlineData(5, "Temp", false)]
        [InlineData(10, "Temp", false)]
        [InlineData(15, "Temp", false)]
        [InlineData(20, "Temp", false)]
        [InlineData(5, "A string of adequate length.", true)]
        [InlineData(10, "A string of adequate length.", true)]
        [InlineData(15, "A string of adequate length.", true)]
        [InlineData(20, "A string of adequate length.", false)]
        public void ObjCanValidate(int id, string name, bool expected)
        {
            var test = new ValidationDto { Index = id, Name = name };
            _ = test.ValidateMe(out var actual);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0, "Temp", false)]
        [InlineData(5, "Temp", false)]
        [InlineData(10, "Temp", false)]
        [InlineData(15, "Temp", false)]
        [InlineData(20, "Temp", false)]
        [InlineData(5, "A string of adequate length.", true)]
        [InlineData(10, "A string of adequate length.", true)]
        [InlineData(15, "A string of adequate length.", true)]
        [InlineData(20, "A string of adequate length.", false)]
        public void ObjCanReturnValidations(int id, string name, bool expected)
        {
            expected = !expected;
            var test = new ValidationDto { Index = id, Name = name };
            var response = test.ValidateMe(out var _);
            var isEmpty = response.Any();
            Assert.Equal(expected, isEmpty);
        }
    }
}