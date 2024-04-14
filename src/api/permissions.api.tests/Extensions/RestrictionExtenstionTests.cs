using legallead.jdbc.entities;
using legallead.permissions.api.Extensions;

namespace permissions.api.tests.Extensions
{
    public class RestrictionExtenstionTests
    {

        private static readonly Faker<SearchRestrictionDto> restrictionFaker
            = new Faker<SearchRestrictionDto>()
            .RuleFor(x => x.IsLocked, y => y.Random.Bool())
            .RuleFor(x => x.Reason, y => y.Random.Guid().ToString())
            .RuleFor(x => x.MaxPerMonth, y => y.Random.Int(100, 500))
            .RuleFor(x => x.MaxPerYear, y => y.Random.Int(2500, 5000))
            .RuleFor(x => x.ThisMonth, y => y.Random.Int(0, 100))
            .RuleFor(x => x.ThisYear, y => y.Random.Int(100, 1500));

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void RestrictionToModelCheck(bool monthlyExceeded, bool annualExceed)
        {
            var item = restrictionFaker.Generate();
            if (monthlyExceeded) { item.ThisMonth = item.MaxPerMonth.GetValueOrDefault() + item.ThisMonth.GetValueOrDefault(); }
            if (annualExceed) { item.ThisYear = item.MaxPerYear.GetValueOrDefault() + item.ThisYear.GetValueOrDefault(); }
            var model = item.ToModel();
            Assert.NotNull(model);
        }
    }
}
