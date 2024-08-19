using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;

namespace permissions.api.tests.Entities
{
    public class QueueInitializeRequestTests
    {
        [Fact]
        public void QueueInitializeRequestCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new QueueInitializeRequest();
            });
            Assert.Null(error);
        }

        [Fact]
        public void QueueInitializeRequestCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                _ = faker.Generate(10);
            });
            Assert.Null(error);
        }

        [Fact]
        public void QueueInitializeRequestFieldsCanGetAneSet()
        {
            var error = Record.Exception(() =>
            {
                var collection = faker.Generate(2);
                var a = collection[0];
                var b = collection[1];
                Assert.NotEqual(a.MachineName, b.MachineName);
                Assert.NotEqual(a.Message, b.Message);
                Assert.NotEqual(a.StatusId, b.StatusId);
                Assert.NotEqual(a.Items, b.Items);
                _ = a.IsValid();
                _ = a.Serialize();
            });
            Assert.Null(error);
        }

        private static readonly Faker<QueueInitializeRequestItem> faker1 =
            new Faker<QueueInitializeRequestItem>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        private static readonly Faker<QueueInitializeRequest> faker =
            new Faker<QueueInitializeRequest>()
            .RuleFor(x => x.MachineName, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Message, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StatusId, y => y.Random.Int(0, 500000))
            .RuleFor(x => x.Items, y =>
            {
                var n = y.Random.Int(1, 10);
                return faker1.Generate(n);
            });
    }
}