using legallead.permissions.api.Enumerations;
using legallead.permissions.api.Model;
using System.ComponentModel.DataAnnotations;

namespace permissions.api.tests.Models
{
    public class GetContactRequestTests
    {
        private static readonly Faker<GetContactRequest> faker =
            new Faker<GetContactRequest>()
                .RuleFor(x => x.RequestType, y =>
                {
                    var requestName = y.PickRandom<RequestTypeNames>().ToString();
                    return requestName.Equals("None") ? string.Empty : requestName;
                });

        [Fact]
        public void ContactCanCreateAsValid()
        {
            var item = faker.Generate();
            _ = Check(item, out var isvalid);
            Assert.True(isvalid);
        }

        [Theory]
        [InlineData("Address", true)]
        [InlineData("address", true)]
        [InlineData("Email", true)]
        [InlineData("email", true)]
        [InlineData("Name", true)]
        [InlineData("name", true)]
        [InlineData("Phone", true)]
        [InlineData("phone", true)]
        [InlineData("", true)]
        [InlineData("NotInList", false)]
        public void RequestTypeValidation(string? name, bool expected)
        {
            var test = faker.Generate();
            test.RequestType = name;
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