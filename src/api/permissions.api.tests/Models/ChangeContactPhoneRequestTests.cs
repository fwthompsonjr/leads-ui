using legallead.permissions.api.Enum;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Http.Features;
using System.ComponentModel.DataAnnotations;

namespace permissions.api.tests.Models
{
    public class ChangeContactPhoneRequestTests
    {
        private readonly static string[] fakerNumbers = new string[]
        {
            "(799) 846-8465",
            "(355) 976-9927",
            "(688) 671-3533",
            "(731) 708-8757",
            "(587) 863-9416",
            "(795) 499-0471",
            "(714) 682-7575",
            "(939) 792-5046",
            "(842) 691-3765",
            "(907) 785-9086",
            "(226) 349-3936",
            "(871) 758-6644",
            "608-943-3822",
            "572-620-8386",
            "575-439-9534",
            "401-869-3467",
            "803-353-1652",
            "364-371-8470",
            "229-591-5606",
            "469-496-2948",
        };
        private readonly static Faker<ChangeContactPhoneRequest> faker =
            new Faker<ChangeContactPhoneRequest>()
                .RuleFor(x => x.PhoneType, y => y.PickRandom<PhoneTypeNames>().ToString())
                .RuleFor(x => x.Phone, y => y.PickRandom(fakerNumbers));

        [Fact]
        public void ContactCanCreateAsValid()
        {
            var items = faker.Generate(fakerNumbers.Length);
            var results = new List<bool>();
            items.ForEach(x => {
                _ = Check(x, out var isvalid);
                results.Add(isvalid);
            });
            Assert.DoesNotContain(false, results);
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
            test.PhoneType = name;
            _ = Check(test, out var actual);
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(5, false)]
        [InlineData(7, false)]
        [InlineData(10, false)]
        [InlineData(25, false)]
        public void ContactLengthValidation(int length, bool expected)
        {
            var fieldData = new Faker().Random.AlphaNumeric(length);
            var test = faker.Generate();
            test.Phone = fieldData;
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