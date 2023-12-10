using legallead.json.db.attr;
using legallead.json.db.entity;
using System.ComponentModel.DataAnnotations;

namespace legallead.json.db.tests.attr
{
    [Collection("Sequential")]
    public class UsCountyAttributeTests
    {
        private static bool isInitialized = false;
        private static readonly object locker = new();
        public UsCountyAttributeTests()
        {
            if (!isInitialized)
            {
                lock (locker)
                {
                    UsState.Initialize();
                    UsStateCounty.Initialize();
                    isInitialized = true; 
                }
            }
        }

        [Theory]
        [InlineData("denton", true)]
        [InlineData("collin", true)]
        [InlineData("tarrant", true)]
        [InlineData("Denton", true)]
        [InlineData("Nane", false)]
        [InlineData("Colorado", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void UsCountyAttributeTest(string? name, bool expected)
        {
            var test = new NameTest { Name = name };
            _ = Check(test, out var actual);
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData("collin", "Tx", true)]
        [InlineData("denton", "Tx", true)]
        [InlineData("tarrant", "Texas", true)]
        [InlineData("Denton", "Colorado", false)]
        [InlineData("Nane", "Texas", false)]
        [InlineData("Colorado", "Texas", false)]
        public void StateToCountyTest(string? name, string? stateName, bool expected)
        {
            var test = new NameStateTest { Name = name, State = stateName };
            _ = Check(test, out var actual);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("denton", true)]
        [InlineData("collin", true)]
        [InlineData("tarrant", true)]
        [InlineData("Denton", true)]
        [InlineData("Nane", false)]
        [InlineData("Colorado", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void UsCountyCanFind(string? name, bool expected)
        {
            var test = UsStateCountyList.Find(name);
            var actual = test != null;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("denton", true)]
        [InlineData("collin", true)]
        [InlineData("tarrant", true)]
        [InlineData("Denton", true)]
        [InlineData("Nane", false)]
        [InlineData("Colorado", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void UsCountyCanFindAll(string? name, bool expected)
        {
            var test = UsStateCountyList.FindAll(name);
            var actual = test != null;
            Assert.Equal(expected, actual);
        }

        private static List<ValidationResult> Check<T>(T source, out bool isValid) where T : class
        {
            var context = new ValidationContext(source, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(source, context, validationResults, true);
            return validationResults;
        }


        private sealed class NameTest
        {
            [UsCounty]
            public string? Name { get; set; } = string.Empty;
        }

        private sealed class NameStateTest
        {
            [UsCounty]
            [StateCheck("State")]
            public string? Name { get; set; } = string.Empty;

            [UsState]
            public string? State { get; set; } = string.Empty;
        }
    }
}