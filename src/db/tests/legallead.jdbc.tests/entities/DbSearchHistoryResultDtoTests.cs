using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbSearchHistoryResultDtoTests
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
        [InlineData("SearchHistoryId")]
        [InlineData("Name")]
        [InlineData("Zip")]
        [InlineData("Address1")]
        [InlineData("Address2")]
        [InlineData("Address3")]
        [InlineData("CaseNumber")]
        [InlineData("DateFiled")]
        [InlineData("Court")]
        [InlineData("CaseType")]
        [InlineData("CaseStyle")]
        [InlineData("Plaintiff")]
        [InlineData("CreateDate")]
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
        [InlineData("SearchHistoryId")]
        [InlineData("Name")]
        [InlineData("Zip")]
        [InlineData("Address1")]
        [InlineData("Address2")]
        [InlineData("Address3")]
        [InlineData("CaseNumber")]
        [InlineData("DateFiled")]
        [InlineData("Court")]
        [InlineData("CaseType")]
        [InlineData("CaseStyle")]
        [InlineData("Plaintiff")]
        [InlineData("CreateDate")]
        public void ModelCanReadWriteByIndex(string fieldName)
        {
            var demo = dfaker.Generate();
            var sut = dfaker.Generate();
            demo["id"] = null;
            sut[fieldName] = demo[fieldName];
            var actual = sut[fieldName];
            Assert.Equal(demo[fieldName], actual);
        }

        private static readonly Faker<DbSearchHistoryResultDto> dfaker
            = new Faker<DbSearchHistoryResultDto>()
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