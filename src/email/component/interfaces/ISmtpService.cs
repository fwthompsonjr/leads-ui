using System.Net.Mail;

namespace legallead.email.services
{
    public interface ISmtpService
    {
        bool Send(MailMessage? message);
        Task<bool> SendAsync(MailMessage? message);
    }
}