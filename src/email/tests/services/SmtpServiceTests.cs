using Bogus;
using legallead.email.services;
using Moq;
using System.Net;
using System.Net.Mail;

namespace legallead.email.tests.services
{
    public class SmtpServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var settings = new SettingsService();
                var wrapper = new Mock<ISmtpClientWrapper>();
                var logger = new Mock<IMailLoggingService>();
                _ = new SmtpService(settings, wrapper.Object, logger.Object);

            });
            Assert.Null(exception);
        }

        [Fact]
        public void ServiceCanGetClient()
        {
            var exception = Record.Exception(() =>
            {
                var wrapper = new Mock<ISmtpClientWrapper>();
                var logger = new Mock<IMailLoggingService>();
                var service = new SmtpService(new SettingsService(), wrapper.Object, logger.Object);
                _ = service.GetClient();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ServiceCanGetCredentials()
        {
            var exception = Record.Exception(() =>
            {
                var wrapper = new Mock<ISmtpClientWrapper>();
                var logger = new Mock<IMailLoggingService>();
                var service = new SmtpService(new SettingsService(), wrapper.Object, logger.Object);
                _ = service.GetCredentials();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [InlineData(false, false)]
        [InlineData(false, true, false)]
        public void ServiceCanSend(bool hasException, bool hasMessage = true, bool hasUserId = true)
        {
            var exception = Record.Exception(() =>
            {
                var err = new Faker().System.Exception();
                var message = hasMessage ? new MailMessage() : null;
                var wrapper = new Mock<ISmtpClientWrapper>();
                var logger = new Mock<IMailLoggingService>();
                var service = new MockSmtpService(new SettingsService(), wrapper, logger);
                var guid = Guid.NewGuid().ToString();
                logger.Setup(x => x.Log(It.IsAny<string>(), It.IsAny<MailMessage>())).ReturnsAsync(guid);
                if (hasException)
                {
                    wrapper.Setup(m => m.Send(It.IsAny<SmtpClient>(), It.IsAny<MailMessage>())).Throws(err);
                }
                else
                {
                    wrapper.Setup(m => m.Send(It.IsAny<SmtpClient>(), It.IsAny<MailMessage>()));
                }

                var actual = hasUserId ? service.Send(message, Guid.NewGuid().ToString()) : service.Send(message);
                Assert.NotEqual(hasException, actual);
            });
            if (hasMessage) Assert.Null(exception);
        }

        private sealed class MockSmtpService(
            ISettingsService settings,
            Mock<ISmtpClientWrapper> wrapper,
            Mock<IMailLoggingService> logger) : SmtpService(settings, wrapper.Object, logger.Object)
        {

            private static readonly Mock<SmtpClient> _client = new();
            private static readonly ICredentialsByHost _credential = new NetworkCredential("user", "123abc");


            public override SmtpClient GetClient()
            {
                return _client.Object;
            }

            public override ICredentialsByHost GetCredentials()
            {
                return _credential;
            }

        }

    }
}
