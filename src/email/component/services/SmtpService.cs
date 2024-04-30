using legallead.email.models;
using System.Net;
using System.Net.Mail;

namespace legallead.email.services
{
    internal class SmtpService(
        ISettingsService settings,
        ISmtpClientWrapper smtpwrapper,
        IMailLoggingService logger) : ISmtpService
    {
        private readonly MailSettings _settings = settings.GetSettings;
        private readonly ISmtpClientWrapper wrapper = smtpwrapper;
        private readonly IMailLoggingService loggingSvc = logger;

        public bool Send(MailMessage? message, string userId = "")
        {
            ArgumentNullException.ThrowIfNull(message);
            using var client = GetClient();
            client.Credentials = GetCredentials();// Enable SSL encryption
            client.EnableSsl = true;// Try to send the message. Show status in console.
            var id = loggingSvc.Log(userId, message).GetAwaiter().GetResult();
            try
            {
                Console.WriteLine("Attempting to send email...");
                wrapper.Send(client, message);
                Console.WriteLine("Email sent!");
                if (!string.IsNullOrEmpty(id)) { loggingSvc.Success(id); }
                return true;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(id)) { loggingSvc.Error(id, ex.ToString()); }
                return false;
            }
        }

        public virtual SmtpClient GetClient()
        {
            return new SmtpClient(_settings.Settings.Endpoint, _settings.Settings.Port);
        }

        public virtual ICredentialsByHost GetCredentials()
        {
            return new NetworkCredential(_settings.Uid, _settings.Secret);
        }
    }
}
