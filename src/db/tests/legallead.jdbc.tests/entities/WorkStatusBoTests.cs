using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class WorkStatusBoTests
    {


        private static readonly Faker<WorkStatusBo> faker =
            new Faker<WorkStatusBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.MessageId, y => y.Random.Int(0, 6))
            .RuleFor(x => x.StatusId, y => y.Random.Int(0, 2));


        [Fact]
        public void WorkStatusBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var tmp = new WorkStatusBo();
                Assert.False(string.IsNullOrEmpty(tmp.Source));
            });
            Assert.Null(exception);
        }

        [Fact]
        public void WorkStatusBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void WorkStatusBoCanSetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Id = src.Id;
            Assert.Equal(src.Id, dest.Id);
        }

        [Fact]
        public void WorkStatusBoCanSetSource()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Source = src.Source;
            Assert.Equal(src.Source, dest.Source);
        }

        [Fact]
        public void WorkStatusBoCanSetMessageId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.MessageId = src.MessageId;
            Assert.Equal(src.MessageId, dest.MessageId);
        }

        [Fact]
        public void WorkStatusBoCanSetStatusId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.StatusId = src.StatusId;
            Assert.Equal(src.StatusId, dest.StatusId);
        }
    }
}