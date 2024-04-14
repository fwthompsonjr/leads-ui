using legallead.permissions.api.Attr;

namespace permissions.api.tests.Attr
{
    public class LevelRequestAttributeTests
    {
        private const int One = 1;
        private static readonly List<string> names = "Admin,Platinum,Gold,Silver,Guest".Split(',').ToList();
        private static readonly Faker<LevelRequestAttribute> faker
            = new Faker<LevelRequestAttribute>()
            .RuleFor(x => x.Level, y => y.PickRandom(names));

        [Fact]
        public void SutCanGenerate()
        {
            var exception = Record.Exception(() => faker.Generate());
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true, "Admin")]
        [InlineData(true, "Platinum")]
        [InlineData(true, "Gold")]
        [InlineData(true, "Silver")]
        [InlineData(true, "Guest")]
        [InlineData(true, "admin")]
        [InlineData(true, "platinum")]
        [InlineData(true, "gold")]
        [InlineData(true, "silver")]
        [InlineData(true, "guest")]
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