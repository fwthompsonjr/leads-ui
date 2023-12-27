using legallead.jdbc.attr;
using System.ComponentModel.DataAnnotations;

namespace legallead.json.db.tests.attr
{
    public class PermissionNameTests
    {
        [Theory]
        [InlineData("None", true)]
        [InlineData("Guest", true)]
        [InlineData("Silver", true)]
        [InlineData("Gold", true)]
        [InlineData("Platinum", true)]
        [InlineData("Admin", true)]
        [InlineData("none", true)]
        [InlineData("guest", true)]
        [InlineData("SilveR", true)]
        [InlineData("gold", true)]
        [InlineData("platinum", true)]
        [InlineData("admin", true)]
        [InlineData("Nane", false)]
        [InlineData("Guess", false)]
        [InlineData("Silva", false)]
        [InlineData("Golden", false)]
        [InlineData("Platipus", false)]
        [InlineData("Administrator", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void PermissionAttributeTest(string? name, bool expected)
        {
            var test = new NameTest { Name = name };
            _ = Check(test, out var actual);
            Assert.Equal(expected, actual);
        }

        private sealed class NameTest
        {
            [PermissionName]
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