﻿using legallead.permissions.api.Entities;

namespace permissions.api.tests.Entities
{
    public class QueueRecordStatusRequestTests
    {
        [Fact]
        public void QueueRecordStatusRequestCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new QueueRecordStatusRequest();
            });
            Assert.Null(error);
        }

        [Fact]
        public void QueueRecordStatusRequestCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                _ = faker.Generate(10);
            });
            Assert.Null(error);
        }

        [Fact]
        public void QueueRecordStatusRequestFieldsCanGetAndSet()
        {
            var error = Record.Exception(() =>
            {
                var collection = faker.Generate(2);
                var a = collection[0];
                var b = collection[1];
                Assert.NotEqual(a.UniqueId, b.UniqueId);
                Assert.NotEqual(a.MessageId, b.MessageId);
                Assert.NotEqual(a.StatusId, b.StatusId);
            });
            Assert.Null(error);
        }

        private static readonly Faker<QueueRecordStatusRequest> faker =
            new Faker<QueueRecordStatusRequest>()
            .RuleFor(x => x.UniqueId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.MessageId, y => y.Random.Int(0, 200000))
            .RuleFor(x => x.StatusId, y => y.Random.Int(0, 200000));
    }
}