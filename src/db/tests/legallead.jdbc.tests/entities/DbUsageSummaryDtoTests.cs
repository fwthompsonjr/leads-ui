using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbUsageSummaryDtoTests
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

        [Theory]
        [InlineData("Id")]
        [InlineData("UserName")]
        [InlineData("LeadUserId")]
        [InlineData("SearchYear")]
        [InlineData("SearchMonth")]
        [InlineData("LastSearchDate")]
        [InlineData("CountyId")]
        [InlineData("CountyName")]
        [InlineData("MTD")]
        [InlineData("MonthlyLimit")]
        public void ModelHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = dfaker.Generate();
            var fields = sut.FieldList;
            sut[na] = na;
            _ = sut[na];
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("UserName")]
        [InlineData("LeadUserId")]
        [InlineData("SearchYear")]
        [InlineData("SearchMonth")]
        [InlineData("LastSearchDate")]
        [InlineData("CountyId")]
        [InlineData("CountyName")]
        [InlineData("MTD")]
        [InlineData("MonthlyLimit")]
        public void ModelCanReadWriteByIndex(string fieldName)
        {
            var demo = dfaker.Generate();
            var sut = dfaker.Generate();
            demo["id"] = null;
            sut[fieldName] = demo[fieldName];
            var actual = sut[fieldName];
            Assert.Equal(demo[fieldName], actual);
        }
        private static readonly Faker<DbUsageSummaryDto> dfaker
            = new Faker<DbUsageSummaryDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.UserName, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.SearchYear, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.SearchMonth, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.LastSearchDate, y => y.Date.Recent())
            .RuleFor(x => x.LeadUserId, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 555555))
            .RuleFor(x => x.CountyName, y => y.Random.AlphaNumeric(25))
            .RuleFor(x => x.MTD, y => y.Random.Int())
            .RuleFor(x => x.MonthlyLimit, y => y.Random.Int(1, 555555));
    }
}