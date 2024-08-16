using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class QueueWorkingBoTests
    {


        private static readonly Faker<QueueWorkingBo> faker =
            new Faker<QueueWorkingBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Message, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.StatusId, y => y.Random.Int(-1, 2000))
            .RuleFor(x => x.MachineName, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(300))
            .RuleFor(x => x.LastUpdateDt, y => y.Date.Recent(15))
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent(15));

        [Fact]
        public void QueueWorkingBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new QueueWorkingBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void QueueWorkingBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void QueueWorkingBoCanGetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Id, dest.Id);
        }

        [Fact]
        public void QueueWorkingBoCanGetSearchId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.SearchId, dest.SearchId);
        }

        [Fact]
        public void QueueWorkingBoCanGetStatusId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.StatusId, dest.StatusId);
        }

        [Fact]
        public void QueueWorkingBoCanGetMessage()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Message, dest.Message);
        }

        [Fact]
        public void QueueWorkingBoCanGetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.CreateDate, dest.CreateDate);
        }

        [Fact]
        public void QueueWorkingBoCanGetLastUpdateDt()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.LastUpdateDt, dest.LastUpdateDt);
        }

        [Fact]
        public void QueueWorkingBoCanGetCompletionDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.CompletionDate, dest.CompletionDate);
        }
    }
}