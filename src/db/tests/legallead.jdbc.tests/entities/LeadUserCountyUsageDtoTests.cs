using Bogus;
using legallead.jdbc.entities;
using System.Text;

namespace legallead.jdbc.tests.entities
{
    public class LeadUserCountyUsageDtoTests
    {

        private static readonly Faker<LeadUserCountyUsageDto> faker =
            new Faker<LeadUserCountyUsageDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LeadUserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LeadUserCountyId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.MonthlyUsage, y => y.Random.Int(0, 5000000))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void LeadUserCountyUsageDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LeadUserCountyUsageDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadUserCountyUsageDtoCanBeGenerated()
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
        [InlineData("LeadUserCountyId")]
        [InlineData("CountyName")]
        [InlineData("MonthlyUsage")]
        [InlineData("CreateDate")]
        public void LeadUserCountyUsageDtoHasExpectedFieldDefined(string name)
        {
            const string na = "notmapped";
            var sut = new LeadUserCountyUsageDto();
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
        [InlineData("LeadUserCountyId")]
        [InlineData("CountyName")]
        [InlineData("MonthlyUsage")]
        [InlineData("CreateDate")]
        public void LeadUserCountyUsageDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new LeadUserCountyUsageDto();
            var flds = sut.FieldList;
            demo["id"] = null;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}