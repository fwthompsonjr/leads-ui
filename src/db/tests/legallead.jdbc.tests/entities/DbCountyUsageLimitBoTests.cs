using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbCountyUsageLimitBoTests
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
                _ = sut.IsActive;
                Assert.False(string.IsNullOrEmpty(sut.Id));
                Assert.False(string.IsNullOrEmpty(sut.LeadUserId));
                Assert.NotEqual(0, sut.CountyId);
                Assert.NotEqual(0, sut.MaxRecords);
                Assert.True(sut.CreateDate.HasValue);
                Assert.True(sut.CompleteDate.HasValue);
            });
            Assert.Null(error);
        }

        private static readonly Faker<DbCountyUsageLimitBo> dfaker
            = new Faker<DbCountyUsageLimitBo>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.MaxRecords, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent());
    }
}