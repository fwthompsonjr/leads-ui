using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class WorkingSearchBoTests
    {


        private static readonly Faker<WorkingSearchBo> faker =
            new Faker<WorkingSearchBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.MessageId, y => y.Random.Int(0, 6))
            .RuleFor(x => x.StatusId, y => y.Random.Int(0, 2))
            .RuleFor(x => x.MachineName, y => y.Hacker.Phrase())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.LastUpdateDt, y => y.Date.Recent())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent());


        [Fact]
        public void WorkingSearchBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new WorkingSearchBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void WorkingSearchBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void WorkingSearchBoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void WorkingSearchBoCanSetSearchId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.SearchId = src.SearchId;
            Assert.Equal(src.SearchId, dest.SearchId);
        }

        [Fact]
        public void WorkingSearchBoCanSetMessageId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.MessageId = src.MessageId;
            Assert.Equal(src.MessageId, dest.MessageId);
        }

        [Fact]
        public void WorkingSearchBoCanSetStatusId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.StatusId = src.StatusId;
            Assert.Equal(src.StatusId, dest.StatusId);
        }

        [Fact]
        public void WorkingSearchBoCanSetMachineName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.MachineName = src.MachineName;
            Assert.Equal(src.MachineName, dest.MachineName);
        }

        [Fact]
        public void WorkingSearchBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void WorkingSearchBoCanSetLastUpdateDt()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.LastUpdateDt = src.LastUpdateDt;
            Assert.Equal(src.LastUpdateDt, dest.LastUpdateDt);
        }

        [Fact]
        public void WorkingSearchBoCanSetCompletionDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CompletionDate = src.CompletionDate;
            Assert.Equal(src.CompletionDate, dest.CompletionDate);
        }
    }
}