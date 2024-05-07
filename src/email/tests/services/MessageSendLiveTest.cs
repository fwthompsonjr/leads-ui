using legallead.email.services;
using legallead.email.utility;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace legallead.email.tests.services
{
    public class MessageSendLiveTest
    {
        [Theory]
        [InlineData(TemplateNames.RegistrationCompleted)]
        [InlineData(TemplateNames.SearchPaymentCompleted)]
        [InlineData(TemplateNames.BeginSearchRequested)]
        [InlineData(TemplateNames.LockedAccountResponse)]
        [InlineData(TemplateNames.ProfileChanged)]
        public void MessageCanSendLiveEmail(TemplateNames template)
        {
            if (!Debugger.IsAttached) return;
            var target = TemplateList.Find(x => template == x.GetTemplateName());
            if (target == null || !target.IsTesting) return;
            var exception = Record.Exception(() =>
            {
                var parms = JsonConvert.DeserializeObject<SendMailParameters>(emailParameters);
                if (parms == null) return;
                var provider = ServiceInfrastructure.Provider;
                var messageHandler = provider?.GetRequiredService<MailMessageService>();
                var smtpServer = provider?.GetRequiredService<ISmtpService>();
                Assert.NotNull(messageHandler);
                Assert.NotNull(smtpServer);
                messageHandler.With(template, parms.UserId);
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

        private static List<TemplateTestStatusBo> TemplateList
        {
            get
            {
                if (templateList != null) return templateList;
                templateList = JsonConvert.DeserializeObject<List<TemplateTestStatusBo>>(templateJs) ?? [];
                return templateList;
            }
        }

        private static List<TemplateTestStatusBo>? templateList;
        private static readonly string emailParameters = Properties.Resources.email_parameters;
        private static readonly string templateJs = Properties.Resources.template_test_json;


    }
}
