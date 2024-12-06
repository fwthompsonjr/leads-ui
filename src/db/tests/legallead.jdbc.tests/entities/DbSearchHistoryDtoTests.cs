using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class DbSearchHistoryDtoTests
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
        [InlineData("CountyId")]
        [InlineData("RecordCount")]
        [InlineData("SearchTypeId")]
        [InlineData("CaseTypeId")]
        [InlineData("DistrictCourtId")]
        [InlineData("DistrictSearchTypeId")]
        [InlineData("SearchDate")]
        [InlineData("CreateDate")]
        [InlineData("CompleteDate")]
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
        [InlineData("CountyId")]
        [InlineData("RecordCount")]
        [InlineData("SearchTypeId")]
        [InlineData("CaseTypeId")]
        [InlineData("DistrictCourtId")]
        [InlineData("DistrictSearchTypeId")]
        [InlineData("SearchDate")]
        [InlineData("CreateDate")]
        [InlineData("CompleteDate")]
        public void ModelCanReadWriteByIndex(string fieldName)
        {
            var demo = dfaker.Generate();
            var sut = dfaker.Generate();
            demo["id"] = null;
            sut[fieldName] = demo[fieldName];
            var actual = sut[fieldName];
            Assert.Equal(demo[fieldName], actual);
        }

        private static readonly Faker<DbSearchHistoryDto> dfaker
            = new Faker<DbSearchHistoryDto>()
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