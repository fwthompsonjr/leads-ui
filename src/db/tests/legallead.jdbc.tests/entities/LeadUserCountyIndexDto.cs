using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LeadUserCountyIndexDtoTests
    {

        private static readonly Faker<LeadUserCountyIndexDto> faker =
            new Faker<LeadUserCountyIndexDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LeadUserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyList, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void LeadUserCountyIndexDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LeadUserCountyIndexDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserCountyIndexDtoCanBeGenerated()
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
        [InlineData("CountyList")]
        [InlineData("CreateDate")]
        public void LeadUserCountyIndexDtoHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = new LeadUserCountyIndexDto();
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
        [InlineData("CountyList")]
        [InlineData("CreateDate")]
        public void LeadUserCountyIndexDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new LeadUserCountyIndexDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}