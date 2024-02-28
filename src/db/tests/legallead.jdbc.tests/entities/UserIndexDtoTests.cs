
using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class UserIndexDtoTests
    {

        private static readonly Faker<UserIndexDto> faker =
            new Faker<UserIndexDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"));

        [Fact]
        public void UserIndexDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserIndexDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserIndexDtoCanBeGenerated()
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
        public void UserIndexDtoCanWriteAndRead(int fieldId)
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