using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class SubscriptionDetailBoTests
    {
        private static readonly Faker<SubscriptionDetailBo> faker =
            new Faker<SubscriptionDetailBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CustomerId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Email, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.SubscriptionType, y => y.Hacker.Phrase())
            .RuleFor(x => x.SubscriptionDetail, y => y.Hacker.Phrase())
            .RuleFor(x => x.PermissionLevel, y => y.Hacker.Phrase())
            .RuleFor(x => x.CountySubscriptions, y => y.Hacker.Phrase())
            .RuleFor(x => x.StateSubscriptions, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsSubscriptionVerified, y => y.Random.Bool())
            .RuleFor(x => x.VerificationDate, y => y.Date.Recent())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void SubscriptionDetailBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new SubscriptionDetailBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SubscriptionDetailBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetUserId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserId = src.UserId;
            Assert.Equal(src.UserId, dest.UserId);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetCustomerId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CustomerId = src.CustomerId;
            Assert.Equal(src.CustomerId, dest.CustomerId);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetEmail()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.Email = src.Email;
            Assert.Equal(src.Email, dest.Email);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetExternalId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ExternalId = src.ExternalId;
            Assert.Equal(src.ExternalId, dest.ExternalId);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetSubscriptionType()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.SubscriptionType = src.SubscriptionType;
            Assert.Equal(src.SubscriptionType, dest.SubscriptionType);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetSubscriptionDetail()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.SubscriptionDetail = src.SubscriptionDetail;
            Assert.Equal(src.SubscriptionDetail, dest.SubscriptionDetail);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetPermissionLevel()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.PermissionLevel = src.PermissionLevel;
            Assert.Equal(src.PermissionLevel, dest.PermissionLevel);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetCountySubscriptions()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CountySubscriptions = src.CountySubscriptions;
            Assert.Equal(src.CountySubscriptions, dest.CountySubscriptions);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetStateSubscriptions()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.StateSubscriptions = src.StateSubscriptions;
            Assert.Equal(src.StateSubscriptions, dest.StateSubscriptions);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetIsSubscriptionVerified()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.IsSubscriptionVerified = src.IsSubscriptionVerified;
            Assert.Equal(src.IsSubscriptionVerified, dest.IsSubscriptionVerified);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetVerificationDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.VerificationDate = src.VerificationDate;
            Assert.Equal(src.VerificationDate, dest.VerificationDate);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetCompletionDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CompletionDate = src.CompletionDate;
            Assert.Equal(src.CompletionDate, dest.CompletionDate);
        }

        [Fact]
        public void SubscriptionDetailBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }
    }
}