using legallead.email.services;
using legallead.email.utility;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Diagnostics;

namespace legallead.email.tests.services
{
    public class MessageSendLiveTest
    {
        [Fact]
        public void MessageCanSendLiveEmail()
        {
            if (!Debugger.IsAttached) return;
            var exception = Record.Exception(() =>
            {
                var parms = JsonConvert.DeserializeObject<SendMailParameters>(emailParameters);
                if (parms == null) return;
                var provider = ServiceInfrastructure.Provider;
                var messageHandler = provider?.GetRequiredService<MailMessageService>();
                var smtpServer = provider?.GetRequiredService<ISmtpService>();
                Assert.NotNull(messageHandler);
                Assert.NotNull(smtpServer);
                messageHandler.With(TemplateNames.AccountRegistration, parms.UserId);
                Assert.True(messageHandler.CanSend());
                // alter message To, CC
                var message = messageHandler.Message;
                if (message == null) return;
                message.To.Clear();
                message.CC.Clear();
                message.To.Add(parms.Email);
                smtpServer.Send(messageHandler.Message);
                Console.WriteLine("message queued.");
            });
            Assert.Null(exception);
        }

        private static readonly string emailParameters = Properties.Resources.email_parameters;
    }
}
