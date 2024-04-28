using Bogus;
using legallead.email.interfaces;
using legallead.email.services;
using Moq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;

namespace legallead.email.tests.services
{
    public class MailLoggingServiceTests
    {
        private static readonly MailLoggingService service = new(new Mock<IUserSettingInfrastructure>().Object);

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
        [Fact]
        public void ServiceCanUnEscape()
        {
            var message = GetMessage();
            var to = JsonConvert.SerializeObject(message.To);
            var cc = JsonConvert.SerializeObject(message.CC);
            var subject = message.Subject;
            var body = message.Body;
            var bytes = Encoding.UTF8.GetBytes(body);
            var content64 = Convert.ToBase64String(bytes);
            var obj = new
            {
                From = JsonConvert.SerializeObject(message.From),
                To = to,
                Cc = cc,
                Subject = subject,
                Body = content64
            };
            var serial = MailLoggingService.UnEscape(JsonConvert.SerializeObject(obj));
            Assert.NotEmpty(serial);
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
            
            var message = new MailMessage("admin@somewhere.org", "person@places.com")
            {
                Subject = "Test",
                Body = Properties.Resources.parser_test_html
            };
            return message;
        }
    }
}
