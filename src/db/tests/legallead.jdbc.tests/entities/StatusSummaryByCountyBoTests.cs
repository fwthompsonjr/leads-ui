using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class StatusSummaryByCountyBoTests
    {



        private static readonly Faker<StatusSummaryByCountyBo> faker =
            new Faker<StatusSummaryByCountyBo>()
            .RuleFor(x => x.Region, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Count, y => y.Random.Int(0, 125000))
            .RuleFor(x => x.Oldest, y => y.Date.Recent())
            .RuleFor(x => x.Newest, y => y.Date.Recent());

        [Fact]
        public void StatusSummaryByCountyBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new StatusSummaryByCountyBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void StatusSummaryByCountyBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void StatusSummaryByCountyBoCanGetRegion()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Region, dest.Region);
        }

        [Fact]
        public void StatusSummaryByCountyBoCanGetCount()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Count, dest.Count);
        }

        [Fact]
        public void StatusSummaryByCountyBoCanGetOldest()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Oldest, dest.Oldest);
        }

        [Fact]
        public void StatusSummaryByCountyBoCanGetNewest()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Newest, dest.Newest);
        }
    }
}