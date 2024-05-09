using legallead.permissions.api.Models;

namespace permissions.api.tests.Models
{
    public class UserSearchQueryModelTests
    {
        private static readonly Faker<UserSearchQueryModel> faker =
            new Faker<UserSearchQueryModel>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.EstimatedRowCount, y => y.Random.Int(1, 100))
            .RuleFor(x => x.SearchProgress, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StateCode, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserSearchQueryModel();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        public void ModelCanGetField(int fieldId)
        {
            var test = faker.Generate();
            var indexed = test[fieldId];
            if (fieldId < 0 || fieldId > 6) Assert.True(string.IsNullOrEmpty(indexed));
            else Assert.False(string.IsNullOrEmpty(indexed));
        }
    }
}