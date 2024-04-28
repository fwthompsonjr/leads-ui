using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;

namespace legallead.email.services
{
    [ExcludeFromCodeCoverage(Justification = "Wrapper class that interacts with remote smtp resources.")]
    internal class SmtpClientWrapper : ISmtpClientWrapper
    {
        public void Send(SmtpClient client, MailMessage message)
        {
            client.Send(message);
        }
        public void SendAsync(SmtpClient client, MailMessage message)
        {
            client.SendAsync(message, Guid.NewGuid().ToString());
        }
    }
}
