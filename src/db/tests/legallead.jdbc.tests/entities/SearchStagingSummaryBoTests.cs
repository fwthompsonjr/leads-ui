using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchStagingSummaryBoTests
    {


        private static readonly Faker<SearchStagingSummaryBo> faker =
            new Faker<SearchStagingSummaryBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.LineNbr, y => y.Random.Int(2, 2000))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent().ToString("f"))
            .RuleFor(x => x.StagingType, y => y.Random.Guid().ToString("D"));


        [Fact]
        public void SearchStagingSummaryBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchStagingSummaryBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchStagingSummaryBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchStagingSummaryBoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void SearchStagingSummaryBoCanSetLineNbr()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.LineNbr = src.LineNbr;
            Assert.Equal(src.LineNbr, dest.LineNbr);
        }

        [Fact]
        public void SearchStagingSummaryBoCanSetSearchId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.SearchId = src.SearchId;
            Assert.Equal(src.SearchId, dest.SearchId);
        }

        [Fact]
        public void SearchStagingSummaryBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void SearchStagingSummaryBoCanSetStagingType()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.StagingType = src.StagingType;
            Assert.Equal(src.StagingType, dest.StagingType);
        }
    }
}