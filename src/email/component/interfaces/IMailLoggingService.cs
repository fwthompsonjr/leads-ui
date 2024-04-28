using System.Net.Mail;

namespace legallead.email.services
{
    public interface IMailLoggingService
    {
        void Error(string id, string message);
        Task<string> Log(string userId, MailMessage message);
        void Success(string id);
    }
}