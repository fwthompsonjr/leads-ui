using legallead.jdbc.entities;
using legallead.permissions.api.Utility;

namespace permissions.api.tests.Utility
{
    public class SearchRestrictionFilterTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(500, 525)]
        [InlineData(100, 100)]
        [InlineData(1, 2000)]
        public void FilterCanHandle(int thisMonth, int thisYear)
        {
            var preview = previewfaker.Generate(50);
            var restrictions = restrictionFaker.Generate();
            restrictions.ThisMonth = thisMonth;
            restrictions.ThisYear = thisYear;
            var items = SearchRestrictionFilter.FilterByRestriction(preview, restrictions);
            Assert.NotNull(items);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(2000)]
        public void FilterCanSanitize(int count)
        {
            var preview = previewfaker.Generate(count);
            var items = SearchRestrictionFilter.Sanitize(preview);
            Assert.NotNull(items);
        }

        private static readonly Faker<SearchPreviewBo> previewfaker =
            new Faker<SearchPreviewBo>()
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Zip, y => y.Person.Address.ZipCode)
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(15))
            .RuleFor(x => x.Court, y => y.Commerce.ProductName())
            .RuleFor(x => x.CaseType, y => y.Commerce.ProductName())
            .RuleFor(x => x.Name, y => y.Person.FullName)
            .RuleFor(x => x.FirstName, y => y.Person.FirstName)
            .RuleFor(x => x.LastName, y => y.Person.LastName);

        private static readonly Faker<SearchRestrictionDto> restrictionFaker
            = new Faker<SearchRestrictionDto>()
            .RuleFor(x => x.IsLocked, y => y.Random.Bool())
            .RuleFor(x => x.Reason, y => y.Random.Guid().ToString())
            .RuleFor(x => x.MaxPerMonth, y => y.Random.Int(55, 100))
            .RuleFor(x => x.MaxPerYear, y => y.Random.Int(150, 250))
            .RuleFor(x => x.ThisMonth, y => y.Random.Int(1, 20))
            .RuleFor(x => x.ThisYear, y => y.Random.Int(10, 100));
    }
}
