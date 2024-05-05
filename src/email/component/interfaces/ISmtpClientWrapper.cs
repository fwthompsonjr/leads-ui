using System.Net.Mail;

namespace legallead.email.services
{
    public interface ISmtpClientWrapper
    {
        bool EmailEnabled { get; }

        void Send(SmtpClient client, MailMessage message);
    }
}