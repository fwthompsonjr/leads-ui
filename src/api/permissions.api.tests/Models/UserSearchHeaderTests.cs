using legallead.permissions.api.Model;

namespace permissions.api.tests.Models
{
    public class UserSearchHeaderTests
    {
        private static readonly Faker<UserSearchHeader> faker =
            new Faker<UserSearchHeader>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.EstimatedRowCount, y => y.Random.Int(1, 100))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserSearchHeader();
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
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ModelCanGetField(int fieldId)
        {
            var test = faker.Generate();
            var control = new UserSearchHeader();
            if (fieldId == 0) Assert.NotEqual(control.Id, test.Id);
            if (fieldId == 1) Assert.NotEqual(control.StartDate, test.StartDate);
            if (fieldId == 2) Assert.NotEqual(control.EndDate, test.EndDate);
            if (fieldId == 3) Assert.NotEqual(control.EstimatedRowCount, test.EstimatedRowCount);
            if (fieldId == 4) Assert.NotEqual(control.CreateDate, test.CreateDate);
        }
    }
}