using Bogus;
using legallead.desktop.entities;

namespace legallead.desktop.tests.entities
{
    public class UserBoTests
    {
        private static readonly Faker<ApiContext> contextfaker =
            new Faker<ApiContext>()
            .RuleFor(x => x.Id, y => y.Random.Int(1, 500).ToString())
            .RuleFor(x => x.Name, y => y.Company.CompanyName());

        private readonly Faker<UserBo> faker =
            new Faker<UserBo>()
            .RuleFor(x => x.IsAuthenicated, y => y.Random.Bool())
            .RuleFor(x => x.Applications, y =>
            {
                var isnull = y.Random.Bool();
                if (isnull) return null;
                var count = y.Random.Int(0, 6);
                return contextfaker.Generate(count).ToArray();
            })
            .RuleFor(x => x.UserName, y => y.Company.CompanyName());

        [Fact]
        public void UserBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void UserBoCanUpdateIsAuthenicated()
        {
            var items = faker.Generate(2);
            items[0].IsAuthenicated = items[1].IsAuthenicated;
            Assert.Equal(items[1].IsAuthenicated, items[0].IsAuthenicated);
        }

        [Fact]
        public void UserBoCanUpdateUserName()
        {
            var items = faker.Generate(2);
            items[0].UserName = items[1].UserName;
            Assert.Equal(items[1].UserName, items[0].UserName);
        }

        [Fact]
        public void UserBoCanGetIsInitialized()
        {
            var items = faker.Generate(10);
            items.ForEach(i =>
            {
                var expected = i.Applications != null;
                Assert.Equal(expected, i.IsInitialized);
            });
        }
    }
}