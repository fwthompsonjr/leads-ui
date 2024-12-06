using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbSearchHistoryResultBoTests
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
                Assert.False(string.IsNullOrEmpty(sut.SearchHistoryId));
                Assert.False(string.IsNullOrEmpty(sut.Name));
                Assert.False(string.IsNullOrEmpty(sut.Zip));
                Assert.False(string.IsNullOrEmpty(sut.Address1));
                Assert.False(string.IsNullOrEmpty(sut.Address2));
                Assert.False(string.IsNullOrEmpty(sut.Address3));
                Assert.False(string.IsNullOrEmpty(sut.CaseNumber));
                Assert.False(string.IsNullOrEmpty(sut.DateFiled));
                Assert.False(string.IsNullOrEmpty(sut.Court));
                Assert.False(string.IsNullOrEmpty(sut.CaseType));
                Assert.False(string.IsNullOrEmpty(sut.CaseStyle));
                Assert.False(string.IsNullOrEmpty(sut.Plaintiff));
                Assert.True(sut.CreateDate.HasValue);
            });
            Assert.Null(error);
        }

        private static readonly Faker<DbSearchHistoryResultBo> dfaker
            = new Faker<DbSearchHistoryResultBo>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.SearchHistoryId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Zip, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Address1, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Address2, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Address3, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.DateFiled, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Court, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CaseType, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CaseStyle, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.Plaintiff, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());
    }
}