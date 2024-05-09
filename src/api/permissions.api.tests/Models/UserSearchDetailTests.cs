using legallead.permissions.api.Model;

namespace permissions.api.tests.Models
{
    public class UserSearchDetailTests
    {
        private static readonly Faker<UserSearchDetail> faker =
            new Faker<UserSearchDetail>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Context, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LineNumber, y => y.Random.Int(1, 100))
            .RuleFor(x => x.Line, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new UserSearchDetail();
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
            var control = new UserSearchDetail();
            if (fieldId == 0) Assert.NotEqual(control.Context, test.Context);
            if (fieldId == 1) Assert.NotEqual(control.Id, test.Id);
            if (fieldId == 2) Assert.NotEqual(control.LineNumber, test.LineNumber);
            if (fieldId == 3) Assert.NotEqual(control.Line, test.Line);
            if (fieldId == 4) Assert.NotEqual(control.CreateDate, test.CreateDate);
        }
    }
}