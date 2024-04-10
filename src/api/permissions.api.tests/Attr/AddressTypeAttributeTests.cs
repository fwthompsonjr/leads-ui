using legallead.permissions.api.Attr;

namespace permissions.api.tests.Attr
{
    public class AddressTypeAttributeTests
    {
        private const int One = 1;
        private static readonly List<string> addressNames = "Mailing,Billing".Split(',').ToList();
        private static readonly Faker<AddressTypeAttribute> faker
            = new Faker<AddressTypeAttribute>()
            .RuleFor(x => x.Name, y =>
            {
                var n = y.Random.Double();
                var name = y.PickRandom(addressNames);
                if (n > 0.15f) name = y.Random.AlphaNumeric(10);
                return name;
            });

        [Fact]
        public void SutCanGenerate()
        {
            var exception = Record.Exception(() => faker.Generate());
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true, "Mailing")]
        [InlineData(true, "mailing")]
        [InlineData(true, "Billing")]
        [InlineData(true, "billing")]
        [InlineData(false, "Other")]
        [InlineData(false, null)]
        [InlineData(false, One)]
        public void SutCanValidate(bool isValid, object? name)
        {
            var sut = faker.Generate();
            Assert.Equal(isValid, sut.IsValid(name));
        }
    }
}
