using legallead.email.models;

namespace legallead.email.tests.entities
{
    public class UserAccountByUserNameBoTests
    {
        private static readonly Faker<UserAccountByUserNameBo> faker =
            new Faker<UserAccountByUserNameBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.UserName, y => y.Person.UserName);

        [Fact]
        public void DtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void DtoHasExpectedFieldDefined()
        {
            var sut = new UserAccountByUserNameBo();
            var test = faker.Generate();
            Assert.NotEqual(sut.Id, test.Id);
            Assert.NotEqual(sut.Email, test.Email);
            Assert.NotEqual(sut.UserName, test.UserName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData("1")]
        [InlineData("2")]
        [InlineData("3")]
        [InlineData("4")]
        public void DtoCanValidate(string? fakeUserName)
        {
            var test = faker.Generate();
            bool isFake = string.IsNullOrWhiteSpace(fakeUserName);
            if (isFake) test.UserName = fakeUserName;
            var actual = test.IsValid;
            Assert.NotEqual(isFake, actual);
        }

        [Fact]
        public void DtoCanGetParameters()
        {
            var test = faker.Generate();
            var parms = test.GetParameters();
            Assert.NotNull(parms);
        }
    }
}