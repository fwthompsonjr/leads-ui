using legallead.models;

namespace permissions.api.tests.Models
{
    public class UserLevelRequestTests
    {
        private static readonly Faker<UserLevelRequest> faker = new Faker<UserLevelRequest>()
            .RuleFor(x => x.Level, y => y.Random.AlphaNumeric(15));

        [Fact]
        public void ModelCanBeCreated()
        {
            var model = faker.Generate();
            Assert.False(string.IsNullOrEmpty(model.Level));
        }
    }
}