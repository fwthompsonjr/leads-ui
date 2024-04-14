using legallead.permissions.api.Model;

namespace permissions.api.tests.Models
{
    public class PermissionGroupModelTests
    {
        private static readonly Faker<PermissionGroupModel> faker =
            new Faker<PermissionGroupModel>()
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.GroupId, y => y.Random.Int(1, 16))
            .RuleFor(x => x.OrderId, y => y.Random.Int(1, 100))
            .RuleFor(x => x.PerRequest, y => y.Random.Int(1, 100))
            .RuleFor(x => x.PerMonth, y => y.Random.Int(1, 100))
            .RuleFor(x => x.PerYear, y => y.Random.Int(1, 100))
            .RuleFor(x => x.IsActive, y => y.Random.Bool());


        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PermissionGroupModel();
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
        [InlineData(5)]
        [InlineData(6)]
        public void ModelCanGetField(int fieldId)
        {
            var test = faker.Generate();
            var control = new PermissionGroupModel();
            if (fieldId == 0) Assert.NotEqual(control.Name, test.Name);
            if (fieldId == 1) Assert.NotEqual(control.GroupId, test.GroupId);
            if (fieldId == 2) Assert.NotEqual(control.OrderId, test.OrderId);
            if (fieldId == 3) Assert.NotEqual(control.PerRequest, test.PerRequest);
            if (fieldId == 4) Assert.NotEqual(control.PerMonth, test.PerMonth);
            if (fieldId == 5) Assert.NotEqual(control.PerYear, test.PerYear);
            if (fieldId == 6) Assert.NotEqual(control.IsActive, test.IsActive);
        }
    }
}