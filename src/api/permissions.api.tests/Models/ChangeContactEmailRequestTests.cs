using legallead.permissions.api.Enumerations;
using legallead.permissions.api.Model;
using System.ComponentModel.DataAnnotations;

namespace permissions.api.tests.Models
{
    public class ChangeContactEmailRequestTests
    {
        private readonly static Faker<ChangeContactEmailRequest> faker =
            new Faker<ChangeContactEmailRequest>()
                .RuleFor(x => x.EmailType, y => y.PickRandom<EmailTypeNames>().ToString())
                .RuleFor(x => x.Email, y => y.Person.Email);

        [Fact]
        public void ContactCanCreateAsValid()
        {
            var item = faker.Generate();
            _ = Check(item, out var isvalid);
            Assert.True(isvalid);
        }

        [Theory]
        [InlineData("Personal", true)]
        [InlineData("personal", true)]
        [InlineData("Business", true)]
        [InlineData("business", true)]
        [InlineData("NotInList", false)]
        [InlineData("Other", true)]
        [InlineData("other", true)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void ContactTypeValidation(string? name, bool expected)
        {
            var test = faker.Generate();
            test.EmailType = name;
            _ = Check(test, out var actual);
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(40, true)]
        [InlineData(100, true)]
        [InlineData(200, true)]
        [InlineData(255, true)]
        [InlineData(256, false)]
        [InlineData(1000, false)]
        public void ContactLengthValidation(int length, bool expected)
        {
            var fieldData = new Faker().Random.AlphaNumeric(length);
            var test = faker.Generate();
            test.Email = fieldData;
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