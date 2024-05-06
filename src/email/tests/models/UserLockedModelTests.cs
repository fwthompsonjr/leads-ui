using legallead.email.models;

namespace legallead.email.tests.entities
{
    public class UserLockedModelTests
    {
        private static readonly Faker<UserLockedModel> faker =
            new Faker<UserLockedModel>()
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Message, y => y.Hacker.Phrase());

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
            var sut = new UserLockedModel();
            var test = faker.Generate();
            Assert.NotEqual(sut.Email, test.Email);
            Assert.NotEqual(sut.Message, test.Message);
        }
    }
}