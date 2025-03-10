using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LeadCountyPricingBoTests
    {

        private static readonly Faker<LeadCountyPricingBo> faker =
            new Faker<LeadCountyPricingBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CountyId, y => y.Random.Int())
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.PerRecord, y => y.Random.Decimal())
            .RuleFor(x => x.CompleteDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void LeadCountyPricingBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LeadCountyPricingBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadCountyPricingBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LeadCountyPricingBoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void LeadCountyPricingBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void LeadCountyPricingBoCanSetCountyId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CountyId = src.CountyId;
            Assert.Equal(src.CountyId, dest.CountyId);
        }

        [Fact]
        public void LeadCountyPricingBoCanSetCountyName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CountyName = src.CountyName;
            Assert.Equal(src.CountyName, dest.CountyName);
        }
        [Fact]
        public void LeadCountyPricingBoCanSetIsActive()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.IsActive = src.IsActive;
            Assert.Equal(src.IsActive, dest.IsActive);
        }

        [Fact]
        public void LeadCountyPricingBoCanSetPerRecord()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.PerRecord = src.PerRecord;
            Assert.Equal(src.PerRecord, dest.PerRecord);
        }

        [Fact]
        public void LeadCountyPricingBoCanSetCompleteDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CompleteDate = src.CompleteDate;
            Assert.Equal(src.CompleteDate, dest.CompleteDate);
        }
    }
}