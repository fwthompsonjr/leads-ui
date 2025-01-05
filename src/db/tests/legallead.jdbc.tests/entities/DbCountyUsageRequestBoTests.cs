using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbCountyUsageRequestBoTests
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
                _ = sut.CountyName;
                _ = sut.StartDate;
                _ = sut.EndDate;
                _ = sut.DateRange;
                _ = sut.RecordCount;
                Assert.False(string.IsNullOrEmpty(sut.Id));
                Assert.False(string.IsNullOrEmpty(sut.LeadUserId));
                Assert.NotEqual(0, sut.CountyId);
                Assert.True(sut.CreateDate.HasValue);
                Assert.True(sut.CompleteDate.HasValue);
            });
            Assert.Null(error);
        }

        private static readonly Faker<DbCountyUsageRequestBo> dfaker
            = new Faker<DbCountyUsageRequestBo>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CountyName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.DateRange, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent())
            .RuleFor(x => x.ShortFileName, y => y.System.FileName())
            .RuleFor(x => x.FileCompletedDate, y => y.Date.Recent());
    }
}