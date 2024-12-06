using Bogus;
using legallead.jdbc.models;

namespace legallead.jdbc.tests.entities
{
    public class DbHistoryRequestTests
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
                Assert.NotEqual(0, sut.CountyId);
                Assert.NotEqual(DateTime.MinValue, sut.SearchDate);
                Assert.NotEqual(0, sut.SearchTypeId);
                Assert.NotEqual(0, sut.CaseTypeId);
                Assert.NotEqual(0, sut.DistrictCourtId);
                Assert.NotEqual(0, sut.DistrictSearchTypeId);
            });
            Assert.Null(error);
        }

        private static readonly Faker<DbHistoryRequest> dfaker
            = new Faker<DbHistoryRequest>()
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.SearchDate, y => y.Date.Recent())
            .RuleFor(x => x.SearchTypeId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CaseTypeId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.DistrictCourtId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.DistrictSearchTypeId, y => y.Random.Int(1, 555555));
    }
}