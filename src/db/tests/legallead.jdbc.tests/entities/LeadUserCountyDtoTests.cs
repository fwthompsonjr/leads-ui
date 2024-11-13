using Bogus;
using legallead.jdbc.entities;
using System.Text;

namespace legallead.jdbc.tests.entities
{
    public class LeadUserCountyDtoTests
    {

        private static readonly Faker<LeadUserCountyDto> faker =
            new Faker<LeadUserCountyDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LeadUserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Phrase, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Vector, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Token, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void LeadUserCountyDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LeadUserCountyDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserCountyDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("LeadUserId")]
        [InlineData("CountyName")]
        [InlineData("Phrase")]
        [InlineData("Vector")]
        [InlineData("Token")]
        [InlineData("CreateDate")]
        public void LeadUserCountyDtoHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = new LeadUserCountyDto();
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
        [InlineData("CountyName")]
        [InlineData("Phrase")]
        [InlineData("Vector")]
        [InlineData("Token")]
        [InlineData("CreateDate")]
        public void LeadUserCountyDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new LeadUserCountyDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}