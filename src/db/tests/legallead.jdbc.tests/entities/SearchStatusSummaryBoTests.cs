using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchStatusSummaryBoTests
    {


        private static readonly Faker<SearchStatusSummaryBo> faker =
            new Faker<SearchStatusSummaryBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(2, 2000))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent().ToString("f"))
            .RuleFor(x => x.Line, y => y.Random.Guid().ToString("D"));


        [Fact]
        public void SearchStatusSummaryBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchStatusSummaryBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchStatusSummaryBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchStatusSummaryBoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void SearchStatusSummaryBoCanSetLineNbr()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.LineNbr = src.LineNbr;
            Assert.Equal(src.LineNbr, dest.LineNbr);
        }

        [Fact]
        public void SearchStatusSummaryBoCanSetSearchId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.SearchId = src.SearchId;
            Assert.Equal(src.SearchId, dest.SearchId);
        }

        [Fact]
        public void SearchStatusSummaryBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void SearchStatusSummaryBoCanSetLine()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Line = src.Line;
            Assert.Equal(src.Line, dest.Line);
        }
    }
}