using legallead.models;
using legallead.permissions.api;

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

        [Fact]
        public void ModelCanBeValidated()
        {
            var model = faker.Generate();
            var validation = model.Validate(out var isvalid);
            Assert.False(isvalid);
            Assert.NotNull(validation);
        }
    }
}