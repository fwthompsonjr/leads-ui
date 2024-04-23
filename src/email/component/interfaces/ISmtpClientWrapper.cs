using System.Net.Mail;

namespace legallead.email.services
{
    public interface ISmtpClientWrapper
    {
        void Send(SmtpClient client, MailMessage message);
        void SendAsync(SmtpClient client, MailMessage message);
    }
}