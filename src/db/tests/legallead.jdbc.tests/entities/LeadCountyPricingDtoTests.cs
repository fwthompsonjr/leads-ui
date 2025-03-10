using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LeadCountyPricingDtoTests
    {

        private static readonly Faker<LeadCountyPricingDto> faker =
            new Faker<LeadCountyPricingDto>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyId, y => y.Random.Int())
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.PerRecord, y => y.Random.Decimal())
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void LeadCountyPricingDtoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LeadCountyPricingDto();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadCountyPricingDtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadCountyPricingDtoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void LeadCountyPricingDtoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void LeadCountyPricingDtoIsBaseDto()
        {
            var sut = new LeadCountyPricingDto();
            Assert.NotNull(sut);
            Assert.IsAssignableFrom<BaseDto>(sut);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("CountyId")]
        [InlineData("CountyName")]
        [InlineData("IsActive")]
        [InlineData("PerRecord")]
        [InlineData("CompleteDate")]
        [InlineData("CreateDate")]
        public void LeadCountyPricingDtoHasExpectedFieldDefined(string name)
        {
            var sut = new LeadCountyPricingDto();
            var fields = sut.FieldList;
            Assert.NotNull(fields);
            Assert.NotEmpty(fields);
            Assert.Contains(name, fields);
        }

        [Theory]
        [InlineData("Id")]
        [InlineData("CountyId")]
        [InlineData("CountyName")]
        [InlineData("IsActive")]
        [InlineData("PerRecord")]
        [InlineData("CompleteDate")]
        [InlineData("CreateDate")]
        public void LeadCountyPricingDtoCanReadWriteByIndex(string fieldName)
        {
            var demo = faker.Generate();
            var sut = new LeadCountyPricingDto();
            var flds = sut.FieldList;
            var position = flds.IndexOf(fieldName);
            sut[position] = demo[position];
            var actual = sut[position];
            Assert.Equal(demo[position], actual);
        }
    }
}