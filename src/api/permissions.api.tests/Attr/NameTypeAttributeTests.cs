using legallead.permissions.api.Attr;

namespace permissions.api.tests.Attr
{
    public class NameTypeAttributeTests
    {
        private const int One = 1;
        private static readonly List<string> names = "First,Last,Company".Split(',').ToList();
        private static readonly Faker<NameTypeAttribute> faker
            = new Faker<NameTypeAttribute>()
            .RuleFor(x => x.Name, y => y.PickRandom(names));

        [Fact]
        public void SutCanGenerate()
        {
            var exception = Record.Exception(() => faker.Generate());
            Assert.Null(exception);
        }

        [Theory] // First,Last,Company
        [InlineData(true, "First")]
        [InlineData(true, "first")]
        [InlineData(true, "Last")]
        [InlineData(true, "last")]
        [InlineData(true, "Company")]
        [InlineData(true, "company")]
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