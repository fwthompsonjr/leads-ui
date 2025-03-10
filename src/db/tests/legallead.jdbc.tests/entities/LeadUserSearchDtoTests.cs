using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LeadUserSearchDtoTests
    {

        private static readonly Faker<LeadUserSearchDto> faker =
            new Faker<LeadUserSearchDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LeadUserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyId, y => y.Random.Int())
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.DateRange, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.RecordCount, y => y.Random.Int())
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void LeadUserSearchDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LeadUserSearchDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserSearchDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserSearchDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void LeadUserSearchDtoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void LeadUserSearchDtoIsBaseDto()
        {
            var sut = new LeadUserSearchDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
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
        [InlineData("CompleteDate")]
        [InlineData("CreateDate")]
        public void LeadUserSearchDtoHasExpectedFieldDefined(string name)
        {
            var sut = new LeadUserSearchDto();
            var fields = sut.FieldList;
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
        [InlineData("CompleteDate")]
        [InlineData("CreateDate")]
        public void LeadUserSearchDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new LeadUserSearchDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}