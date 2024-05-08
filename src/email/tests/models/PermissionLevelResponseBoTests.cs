using legallead.email.models;

namespace legallead.email.tests.models
{
    public class PermissionLevelResponseBoTests
    {
        private static readonly Faker<PermissionLevelResponseBo> faker =
            new Faker<PermissionLevelResponseBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.InvoiceUri, y => y.Hacker.Phrase())
            .RuleFor(x => x.LevelName, y => y.Hacker.Phrase())
            .RuleFor(x => x.SessionId, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());


        [Fact]
        public void PermissionLevelResponseBoCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new PermissionLevelResponseBo();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PermissionLevelResponseBoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void PermissionLevelResponseBoCanSetUserId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.UserId = src.UserId;
            Assert.Equal(src.UserId, dest.UserId);
        }

        [Fact]
        public void PermissionLevelResponseBoCanSetExternalId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.ExternalId = src.ExternalId;
            Assert.Equal(src.ExternalId, dest.ExternalId);
        }

        [Fact]
        public void PermissionLevelResponseBoCanSetInvoiceUri()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.InvoiceUri = src.InvoiceUri;
            Assert.Equal(src.InvoiceUri, dest.InvoiceUri);
        }

        [Fact]
        public void PermissionLevelResponseBoCanSetLevelName()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.LevelName = src.LevelName;
            Assert.Equal(src.LevelName, dest.LevelName);
        }

        [Fact]
        public void PermissionLevelResponseBoCanSetSessionId()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.SessionId = src.SessionId;
            Assert.Equal(src.SessionId, dest.SessionId);
        }

        [Fact]
        public void PermissionLevelResponseBoCanSetIsPaymentSuccess()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.IsPaymentSuccess = src.IsPaymentSuccess;
            Assert.Equal(src.IsPaymentSuccess, dest.IsPaymentSuccess);
        }

        [Fact]
        public void PermissionLevelResponseBoCanSetCompletionDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CompletionDate = src.CompletionDate;
            Assert.Equal(src.CompletionDate, dest.CompletionDate);
        }

        [Fact]
        public void PermissionLevelResponseBoCanSetCreateDate()
        {
            var data = faker.Generate(2);
            var src = data[0];
            var dest = data[1];
            dest.CreateDate = src.CreateDate;
            Assert.Equal(src.CreateDate, dest.CreateDate);
        }
    }
}
