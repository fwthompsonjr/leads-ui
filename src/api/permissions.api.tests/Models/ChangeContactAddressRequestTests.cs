using legallead.permissions.api.Enumerations;
using legallead.permissions.api.Model;
using System.ComponentModel.DataAnnotations;

namespace permissions.api.tests.Models
{
    public class ChangeContactAddressRequestTests
    {
        private readonly static Faker<ChangeContactAddressRequest> faker =
            new Faker<ChangeContactAddressRequest>()
                .RuleFor(x => x.AddressType, y => y.PickRandom<AddressTypeNames>().ToString())
                .RuleFor(x => x.Address, y =>
                {
                    var addr = y.Person.Address;
                    return $"{addr.Street} {addr.City} {addr.State} {addr.ZipCode}";
                });

        [Fact]
        public void ContactCanCreateAsValid()
        {
            var item = faker.Generate();
            _ = Check(item, out var isvalid);
            Assert.True(isvalid);
        }

        [Theory]
        [InlineData("Mailing", true)]
        [InlineData("mailing", true)]
        [InlineData("Billing", true)]
        [InlineData("billing", true)]
        [InlineData("NotInList", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        public void ContactTypeValidation(string? name, bool expected)
        {
            var test = faker.Generate();
            test.AddressType = name;
            _ = Check(test, out var actual);
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(40, true)]
        [InlineData(100, true)]
        [InlineData(200, true)]
        [InlineData(400, true)]
        [InlineData(500, true)]
        [InlineData(501, false)]
        [InlineData(1000, false)]
        public void ContactLengthValidation(int length, bool expected)
        {
            var fieldData = new Faker().Random.AlphaNumeric(length);
            var test = faker.Generate();
            test.Address = fieldData;
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