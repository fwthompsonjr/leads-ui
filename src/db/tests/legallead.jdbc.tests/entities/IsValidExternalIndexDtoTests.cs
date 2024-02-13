using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class IsValidExternalIndexDtoTests
    {

        private static readonly Faker<IsValidExternalIndexDto> faker =
            new Faker<IsValidExternalIndexDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.IsFound, y => y.Random.Bool());

        [Fact]
        public void IsValidExternalIndexDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new IsValidExternalIndexDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void IsValidExternalIndexDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void IsValidExternalIndexDtoCanWriteAndRead(int fieldId)
        {
            var exception = Record.Exception(() =>
            {
                var a = faker.Generate();
                var b = faker.Generate();
                a[fieldId] = b[fieldId];
                Assert.Equal(a[fieldId], b[fieldId]);
            });
            Assert.Null(exception);
        }

    }
}