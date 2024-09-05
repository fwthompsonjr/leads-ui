using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class StatusSummaryBoTests
    {



        private static readonly Faker<StatusSummaryBo> faker =
            new Faker<StatusSummaryBo>()
            .RuleFor(x => x.SearchProgress, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Total, y => y.Random.Int(0, 125000));

        [Fact]
        public void StatusSummaryBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new StatusSummaryBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void StatusSummaryBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void StatusSummaryBoCanGetSearchProgress()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.SearchProgress, dest.SearchProgress);
        }

        [Fact]
        public void StatusSummaryBoCanGetTotal()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Total, dest.Total);
        }
    }
}