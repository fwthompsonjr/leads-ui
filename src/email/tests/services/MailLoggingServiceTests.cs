using Bogus;
using legallead.email.services;
using System.Net.Mail;

namespace legallead.email.tests.services
{
    public class MailLoggingServiceTests
    {
        private static readonly MailLoggingService service = new();

        [Theory]
        [InlineData("")]
        [InlineData("not-a-guid")]
        [InlineData("278344f4-d2cc-47d2-b1c5-b6ce27aa1e84")]
        public async Task ServiceCanLog(string userId)
        {
            var exception = await Record.ExceptionAsync(async () =>
            {
                _ = await service.Log(userId, GetMessage());
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-a-guid")]
        [InlineData("278344f4-d2cc-47d2-b1c5-b6ce27aa1e84")]
        public void ServiceCanLogSuccess(string userId)
        {
            var exception = Record.Exception(() =>
            {
                service.Success(userId);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-a-guid")]
        [InlineData("278344f4-d2cc-47d2-b1c5-b6ce27aa1e84")]
        public void ServiceCanLogError(string userId)
        {
            var exception = Record.Exception(() =>
            {
                var message = new Faker().System.Exception().Message;
                service.Error(userId, message);
            });
            Assert.Null(exception);
        }

        private static MailMessage GetMessage()
        {
            var message = new MailMessage("admin@somewhere.org", "person@places.com");
            message.Subject = "Test";
            message.Body = "Test";
            return message;
        }
    }
}
