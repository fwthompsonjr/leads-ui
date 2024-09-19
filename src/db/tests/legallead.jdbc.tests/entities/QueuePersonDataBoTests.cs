using Bogus;
using legallead.jdbc.entities;
using System.Text;

namespace legallead.jdbc.tests.entities
{
    public class QueuePersonDataBoTests
    {

        private static readonly Faker<QueuePersonDataBo> faker =
            new Faker<QueuePersonDataBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Data, y =>
            {
                var txt = y.Hacker.Phrase();
                return Encoding.UTF8.GetBytes(txt);
            });

        [Fact]
        public void QueuePersonDataBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new QueuePersonDataBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void QueuePersonDataBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void QueuePersonDataBoCanGetId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Id, dest.Id);
        }

        [Fact]
        public void QueuePersonDataBoCanGetName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Name, dest.Name);
        }

        [Fact]
        public void QueuePersonDataBoCanGetData()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            Assert.NotEqual(src.Data, dest.Data);
        }
    }
}