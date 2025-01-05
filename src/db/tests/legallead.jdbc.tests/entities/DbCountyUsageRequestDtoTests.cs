using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbCountyUsageRequestDtoTests
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
        [InlineData("LeadUserId")]
        [InlineData("CountyId")]
        [InlineData("CountyName")]
        [InlineData("StartDate")]
        [InlineData("EndDate")]
        [InlineData("DateRange")]
        [InlineData("RecordCount")]
        [InlineData("CreateDate")]
        [InlineData("CompleteDate")]
        [InlineData("ShortFileName")]
        [InlineData("FileCompletedDate")]
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
        [InlineData("LeadUserId")]
        [InlineData("CountyId")]
        [InlineData("CountyName")]
        [InlineData("StartDate")]
        [InlineData("EndDate")]
        [InlineData("DateRange")]
        [InlineData("RecordCount")]
        [InlineData("CreateDate")]
        [InlineData("CompleteDate")]
        [InlineData("ShortFileName")]
        [InlineData("FileCompletedDate")]
        public void ModelCanReadWriteByIndex(string fieldName)
        {
            var demo = dfaker.Generate();
            var sut = dfaker.Generate();
            demo["id"] = null;
            sut[fieldName] = demo[fieldName];
            var actual = sut[fieldName];
            Assert.Equal(demo[fieldName], actual);
        }

        private static readonly Faker<DbCountyUsageRequestDto> dfaker
            = new Faker<DbCountyUsageRequestDto>()
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
            .RuleFor(x => x.FileCompletedDate, y => y.Date.Future());
    }
}
