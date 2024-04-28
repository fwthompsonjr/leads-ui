using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Mail;

namespace legallead.email.services
{
    internal class MailLoggingService : IMailLoggingService
    {
        public async Task<string> Log(string userId, MailMessage message)
        {
            if (string.IsNullOrEmpty(userId)) return string.Empty;
            if (!Guid.TryParse(userId, out var _)) return string.Empty;
            var to = JsonConvert.SerializeObject(message.To);
            var cc = JsonConvert.SerializeObject(message.CC);
            var subject = message.Subject;
            var body = message.Body;
            var obj = new
            {
                From = JsonConvert.SerializeObject(message.From),
                To = to,
                Cc = cc,
                Subject = subject,
                Body = body
            };
            var serial = JsonConvert.SerializeObject(obj);
            var guid = await Task.Run(() =>
            {
                Debug.WriteLine(serial);
                return Guid.NewGuid().ToString();
            });
            return guid;

        }
        public void Success(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (!Guid.TryParse(id, out var _)) return;
            Debug.WriteLine("Message {0} is sent.", id);
        }
        public void Error(string id, string message)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (string.IsNullOrEmpty(message)) return;
            if (!Guid.TryParse(id, out var _)) return;
            Debug.WriteLine("Message {0} has error - {1}.", id, message);
        }
    }
}
