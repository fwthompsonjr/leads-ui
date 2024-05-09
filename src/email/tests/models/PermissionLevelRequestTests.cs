using legallead.email.models;

namespace legallead.email.tests.models
{
    public class PermissionLevelRequestTests
    {
        private static readonly List<string> LevelTypes = [.. "Platinum,Gold,Silver,Guest".Split(',')];
        private static readonly Faker<PermissionLevelRequest> faker
            = new Faker<PermissionLevelRequest>()
            .RuleFor(x => x.Level, y => y.PickRandom(LevelTypes));


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
            var sut = new PermissionLevelRequest();
            var test = faker.Generate();
            Assert.NotEqual(sut.Level, test.Level);
        }
    }
}
