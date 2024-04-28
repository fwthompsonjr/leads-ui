using legallead.email.models;
using System.Net;
using System.Net.Mail;

namespace legallead.email.services
{
    internal class SmtpService(ISettingsService settings, ISmtpClientWrapper smtpwrapper) : ISmtpService
    {
        private readonly MailSettings _settings = settings.GetSettings;
        private readonly ISmtpClientWrapper wrapper = smtpwrapper;

        public bool Send(MailMessage? message)
        {
            ArgumentNullException.ThrowIfNull(message);
            using var client = GetClient();
            client.Credentials = GetCredentials();// Enable SSL encryption
            client.EnableSsl = true;// Try to send the message. Show status in console.
            try
            {
                Console.WriteLine("Attempting to send email...");
                wrapper.Send(client, message);
                Console.WriteLine("Email sent!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The email was not sent.");
                Console.WriteLine("Error message: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> SendAsync(MailMessage? message)
        {
            ArgumentNullException.ThrowIfNull(message);
            return await Task.Run(() =>
            {
                using var client = GetClient();
                client.Credentials = GetCredentials();// Enable SSL encryption
                client.EnableSsl = true;// Try to send the message. Show status in console.
                try
                {
                    Console.WriteLine("Attempting to send email...");
                    wrapper.SendAsync(client, message);
                    Console.WriteLine("Email sent!");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);
                    return false;
                }
            });
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
