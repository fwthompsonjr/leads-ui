using legallead.permissions.api.Attr;

namespace permissions.api.tests.Attr
{
    public class RequestTypeAttributeTests
    {
        private const int One = 1;
        private static readonly List<string> names = "Address,Email,Name,Phone".Split(',').ToList();
        private static readonly Faker<RequestTypeAttribute> faker
            = new Faker<RequestTypeAttribute>()
            .RuleFor(x => x.Name, y => y.PickRandom(names));

        [Fact]
        public void SutCanGenerate()
        {
            var exception = Record.Exception(() => faker.Generate());
            Assert.Null(exception);
        }

        [Theory] // Address,Email,Name,Phone
        [InlineData(true, "Address")]
        [InlineData(true, "address")]
        [InlineData(true, "Email")]
        [InlineData(true, "email")]
        [InlineData(true, "Name")]
        [InlineData(true, "name")]
        [InlineData(true, "Phone")]
        [InlineData(true, "phone")]
        [InlineData(true, "")]
        [InlineData(false, One)]
        [InlineData(false, "ABC")]
        [InlineData(false, null)]
        public void SutCanValidate(bool isValid, object? name)
        {
            var sut = faker.Generate();
            Assert.Equal(isValid, sut.IsValid(name));
        }
    }
}