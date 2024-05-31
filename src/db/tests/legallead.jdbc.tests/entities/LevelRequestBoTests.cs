using Bogus;
using legallead.jdbc.entities;

namespace legallead.jdbc.tests.entities
{
    public class LevelRequestBoTests
    {


        private static readonly Faker<LevelRequestBo> faker =
            new Faker<LevelRequestBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.CustomerId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.InvoiceUri, y => y.Hacker.Phrase())
            .RuleFor(x => x.LevelName, y => y.Hacker.Phrase())
            .RuleFor(x => x.SessionId, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void LevelRequestBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new LevelRequestBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LevelRequestBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LevelRequestBoCanSetUserId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserId = src.UserId;
            Assert.Equal(src.UserId, dest.UserId);
        }

        [Fact]
        public void LevelRequestBoCanSetCustomerId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CustomerId = src.CustomerId;
            Assert.Equal(src.CustomerId, dest.CustomerId);
        }

        [Fact]
        public void LevelRequestBoCanSetExternalId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ExternalId = src.ExternalId;
            Assert.Equal(src.ExternalId, dest.ExternalId);
        }

        [Fact]
        public void LevelRequestBoCanSetInvoiceUri()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.InvoiceUri = src.InvoiceUri;
            Assert.Equal(src.InvoiceUri, dest.InvoiceUri);
        }

        [Fact]
        public void LevelRequestBoCanSetLevelName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.LevelName = src.LevelName;
            Assert.Equal(src.LevelName, dest.LevelName);
        }

        [Fact]
        public void LevelRequestBoCanSetSessionId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.SessionId = src.SessionId;
            Assert.Equal(src.SessionId, dest.SessionId);
        }

        [Fact]
        public void LevelRequestBoCanSetIsPaymentSuccess()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.IsPaymentSuccess = src.IsPaymentSuccess;
            Assert.Equal(src.IsPaymentSuccess, dest.IsPaymentSuccess);
        }

        [Fact]
        public void LevelRequestBoCanSetCompletionDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CompletionDate = src.CompletionDate;
            Assert.Equal(src.CompletionDate, dest.CompletionDate);
        }

        [Fact]
        public void LevelRequestBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }
    }
}