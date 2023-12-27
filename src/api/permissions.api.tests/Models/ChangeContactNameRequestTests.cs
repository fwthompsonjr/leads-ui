using legallead.permissions.api.Enumerations;
using legallead.permissions.api.Model;
using System.ComponentModel.DataAnnotations;

namespace permissions.api.tests.Models
{
    public class ChangeContactNameRequestTests
    {
        private static readonly Faker<ChangeContactNameRequest> faker =
            new Faker<ChangeContactNameRequest>()
                .RuleFor(x => x.NameType, y => y.PickRandom<NameTypeNames>().ToString())
                .RuleFor(x => x.Name, y => y.Person.FullName);

        [Fact]
        public void ContactCanCreateAsValid()
        {
            var item = faker.Generate();
            _ = Check(item, out var isvalid);
            Assert.True(isvalid);
        }

        [Theory]
        [InlineData("First", true)]
        [InlineData("first", true)]
        [InlineData("Last", true)]
        [InlineData("last", true)]
        [InlineData("Company", true)]
        [InlineData("company", true)]
        [InlineData("other", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void ContactTypeValidation(string? name, bool expected)
        {
            var test = faker.Generate();
            test.NameType = name;
            _ = Check(test, out var actual);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(40, true)]
        [InlineData(100, true)]
        [InlineData(175, true)]
        [InlineData(176, false)]
        [InlineData(1000, false)]
        public void ContactLengthValidation(int length, bool expected)
        {
            var fieldData = new Faker().Random.AlphaNumeric(length);
            var test = faker.Generate();
            test.Name = fieldData;
            _ = Check(test, out var actual);
            Assert.Equal(expected, actual);
        }

        private static List<System.ComponentModel.DataAnnotations.ValidationResult> Check<T>(T source, out bool isValid) where T : class
        {
            var context = new ValidationContext(source, serviceProvider: null, items: null);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            isValid = Validator.TryValidateObject(source, context, validationResults, true);
            return validationResults;
        }
    }
}