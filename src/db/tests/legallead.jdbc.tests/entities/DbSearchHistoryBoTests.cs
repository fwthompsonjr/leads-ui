using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbSearchHistoryBoTests
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
                Assert.NotEqual(0, sut.CountyId);
                Assert.NotEqual(0, sut.RecordCount);
                Assert.NotEqual(DateTime.MinValue, sut.SearchDate);
                Assert.NotEqual(0, sut.SearchTypeId);
                Assert.NotEqual(0, sut.CaseTypeId);
                Assert.NotEqual(0, sut.DistrictCourtId);
                Assert.NotEqual(0, sut.DistrictSearchTypeId);
                Assert.True(sut.CreateDate.HasValue);
                Assert.True(sut.CompleteDate.HasValue);
            });
            Assert.Null(error);
        }

        private static readonly Faker<DbSearchHistoryBo> dfaker
            = new Faker<DbSearchHistoryBo>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.RecordCount, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.SearchTypeId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CaseTypeId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.DistrictCourtId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.DistrictSearchTypeId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.SearchDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent());
    }
}