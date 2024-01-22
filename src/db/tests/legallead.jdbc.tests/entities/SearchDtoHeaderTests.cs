using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SearchDtoHeaderTests
    {

        private static readonly Faker<SearchDtoHeader> faker =
            new Faker<SearchDtoHeader>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StartDate, y => y.Date.Recent())
            .RuleFor(x => x.EndDate, y => y.Date.Recent())
            .RuleFor(x => x.EstimatedRowCount, y => y.Random.Int(5, 25055))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void SearchDtoHeaderCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SearchDtoHeader();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchDtoHeaderCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SearchDtoHeaderCanWriteAndRead()
        {
            var exception = Record.Exception(() =>
            {
                var a = faker.Generate();
                var b = faker.Generate();
                a.CreateDate = b.CreateDate;
                a.EndDate = b.EndDate;
                a.EstimatedRowCount = b.EstimatedRowCount;
                a.Id = b.Id;
                a.Name = b.Name;
                a.UserId = b.UserId;
            });
            Assert.Null(exception);
        }

    }
}