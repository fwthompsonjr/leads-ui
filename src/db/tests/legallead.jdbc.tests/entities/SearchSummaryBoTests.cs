using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchSummaryBoTests
    {


        private static readonly Faker<SearchSummaryBo> faker =
            new Faker<SearchSummaryBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.EndDate, y => y.Date.Recent().ToString("f"))
            .RuleFor(x => x.StartDate, y => y.Date.Recent().ToString("f"))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent().ToString("f"))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(2, 2000));


        [Fact]
        public void SearchSummaryBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchSummaryBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchSummaryBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchSummaryBoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void SearchSummaryBoCanSetEndDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.EndDate = src.EndDate;
            Assert.Equal(src.EndDate, dest.EndDate);
        }

        [Fact]
        public void SearchSummaryBoCanSetStartDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.StartDate = src.StartDate;
            Assert.Equal(src.StartDate, dest.StartDate);
        }

        [Fact]
        public void SearchSummaryBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void SearchSummaryBoCanSetExpectedRows()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ExpectedRows = src.ExpectedRows;
            Assert.Equal(src.ExpectedRows, dest.ExpectedRows);
        }
    }
}