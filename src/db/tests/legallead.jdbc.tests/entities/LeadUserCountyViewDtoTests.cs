using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LeadUserCountyViewDtoTests
    {

        private static readonly Faker<LeadUserCountyViewDto> faker =
            new Faker<LeadUserCountyViewDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.RwId, y => y.Random.Int())
            .RuleFor(x => x.CountyId, y => y.Random.Int())
            .RuleFor(x => x.LeadUserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Phrase, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Vector, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Token, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.MonthlyUsage, y => y.Random.Int())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void LeadUserCountyViewDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LeadUserCountyViewDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserCountyViewDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserCountyViewDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void LeadUserCountyViewDtoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void LeadUserCountyViewDtoIsBaseDto()
        {
            var sut = new LeadUserCountyViewDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("RwId")]
        [InlineData("CountyId")]
        [InlineData("LeadUserId")]
        [InlineData("CountyName")]
        [InlineData("Phrase")]
        [InlineData("Vector")]
        [InlineData("Token")]
        [InlineData("MonthlyUsage")]
        [InlineData("CreateDate")]
        public void LeadUserCountyViewDtoHasExpectedFieldDefined(string name)
        {
            var sut = new LeadUserCountyViewDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("RwId")]
        [InlineData("CountyId")]
        [InlineData("LeadUserId")]
        [InlineData("CountyName")]
        [InlineData("Phrase")]
        [InlineData("Vector")]
        [InlineData("Token")]
        [InlineData("MonthlyUsage")]
        [InlineData("CreateDate")]
        public void LeadUserCountyViewDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new LeadUserCountyViewDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}