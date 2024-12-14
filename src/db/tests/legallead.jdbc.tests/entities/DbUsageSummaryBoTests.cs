using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbUsageSummaryBoTests
    {
        [Fact]
        public void ModelCanCreate()
        {
            var error = Record.Exception(() =>
            {
                _ = dfaker.Generate();
            });
            Assert.Null(error);
        }
        [Fact]
        public void ModelHasExpectedFields()
        {
            var error = Record.Exception(() =>
            {
                var sut = dfaker.Generate();
                Assert.False(string.IsNullOrEmpty(sut.Id));
                Assert.False(string.IsNullOrEmpty(sut.UserName));
                Assert.False(string.IsNullOrEmpty(sut.LeadUserId));
                Assert.False(string.IsNullOrEmpty(sut.CountyName));
                Assert.NotEqual(0, sut.SearchYear);
                Assert.NotEqual(0, sut.SearchMonth);
                Assert.NotEqual(0, sut.CountyId);
                Assert.NotEqual(0, sut.MTD);
                Assert.NotEqual(0, sut.MonthlyLimit);
                Assert.True(sut.LastSearchDate.HasValue);
            });
            Assert.Null(error);
        }

        private static readonly Faker<DbUsageSummaryBo> dfaker
            = new Faker<DbUsageSummaryBo>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.UserName, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.SearchYear, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.SearchMonth, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.LastSearchDate, y => y.Date.Recent())
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CountyName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.MTD, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.MonthlyLimit, y => y.Random.Int(1, 555555));
    }
}