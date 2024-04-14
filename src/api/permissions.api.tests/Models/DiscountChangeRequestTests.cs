using legallead.permissions.api.Models;

namespace permissions.api.tests.Models
{
    public class DiscountChangeRequestTests
    {
        private static readonly Faker<DiscountChangeRequest> faker =
            new Faker<DiscountChangeRequest>()
            .RuleFor(x => x.MonthlyBillingCode, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.AnnualBillingCode, y => y.Random.Guid().ToString("D"));


        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new DiscountChangeRequest();
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
        public void ModelCanGetField(int fieldId)
        {
            var test = faker.Generate();
            var control = new DiscountChangeRequest();
            if (fieldId == 0) Assert.NotEqual(control.MonthlyBillingCode, test.MonthlyBillingCode);
            if (fieldId == 1) Assert.NotEqual(control.AnnualBillingCode, test.AnnualBillingCode);
        }
    }
}