using legallead.json.db.attr;
using legallead.json.db.entity;
using System.ComponentModel.DataAnnotations;

namespace legallead.json.db.tests.attr
{

    [Collection("Sequential")]
    public class UsStateAttributeTests
    {
        private static bool isInitialized = false;
        private static readonly object locker = new();
        public UsStateAttributeTests()
        {
            if (!isInitialized)
            {
                lock (locker)
                {
                    UsState.Initialize();
                    isInitialized = true; 
                }
            }
        }

        [Fact]
        public void UsStateAttributeTest()
        {
            var tests = new Dictionary<string, bool>()
            {
                { "Tx", true },
                { "TX", true },
                { "texas", true },
                { "Texas", true },
                { "Nane", false },
                { "Colorado", false },
                { "", false },
            };
            foreach (var item in tests)
            {
                lock (locker)
                {
                    var test = new NameTest { Name = item.Key };
                    _ = Check(test, out var actual);
                    Assert.Equal(item.Value, actual); 
                }
            }
        }

        private sealed class NameTest
        {
            [UsState]
            public string? Name { get; set; } = string.Empty;
        }


        private static List<ValidationResult> Check<T>(T source, out bool isValid) where T : class
        {
            var context = new ValidationContext(source, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(source, context, validationResults, true);
            return validationResults;
        }

    }
}