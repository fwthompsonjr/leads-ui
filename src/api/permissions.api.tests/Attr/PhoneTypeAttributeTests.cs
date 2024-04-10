using legallead.permissions.api.Attr;

namespace permissions.api.tests.Attr
{
    public class PhoneTypeAttributeTests
    {
        private const int One = 1;
        private static readonly List<string> names = "Personal,Business,Other".Split(',').ToList();
        private static readonly Faker<PhoneTypeAttribute> faker
            = new Faker<PhoneTypeAttribute>()
            .RuleFor(x => x.Name, y => y.PickRandom(names));

        [Fact]
        public void SutCanGenerate()
        {
            var exception = Record.Exception(() => faker.Generate());
            Assert.Null(exception);
        }

        [Theory] // Personal,Business,Other
        [InlineData(true, "Personal")]
        [InlineData(true, "personal")]
        [InlineData(true, "Business")]
        [InlineData(true, "business")]
        [InlineData(true, "Other")]
        [InlineData(true, "other")]
        [InlineData(false, "ABC")]
        [InlineData(false, null)]
        [InlineData(false, One)]
        public void SutCanValidate(bool isValid, object? name)
        {
            var sut = faker.Generate();
            Assert.Equal(isValid, sut.IsValid(name));
        }
    }
}
