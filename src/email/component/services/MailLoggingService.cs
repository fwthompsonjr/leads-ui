using legallead.email.interfaces;
using legallead.email.utility;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Text;

namespace legallead.email.services
{
    internal class MailLoggingService(IUserSettingInfrastructure userSettingInfrastructure) : IMailLoggingService
    {
        private readonly IUserSettingInfrastructure userDb = userSettingInfrastructure;

        public async Task<string> Log(string userId, MailMessage message)
        {
            if (string.IsNullOrEmpty(userId)) return string.Empty;
            if (!Guid.TryParse(userId, out var _)) return string.Empty;
            var to = JsonConvert.SerializeObject(message.To);
            var cc = JsonConvert.SerializeObject(message.CC);
            var subject = message.Subject;
            var body = message.Body;
            var bytes = Encoding.UTF8.GetBytes(body);
            var content64 = Convert.ToBase64String(bytes);
            var obj = new
            {
                From = JsonConvert.SerializeObject(message.From),
                To = to,
                Cc = cc,
                Subject = subject,
                Body = content64
            };
            var serial = UnEscape(JsonConvert.SerializeObject(obj));
            var response = await userDb.Log(userId, serial);
            return response?.Id ?? string.Empty;

        }
        public void Success(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (!Guid.TryParse(id, out var _)) return;
            userDb.LogSuccess(id);
        }
        public void Error(string id, string message)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (string.IsNullOrEmpty(message)) return;
            if (!Guid.TryParse(id, out var _)) return;
            var messages = JsonConvert.SerializeObject(message.SplitByLength(400));
            userDb.LogError(id, messages);
        }

        public static string UnEscape(string json)
        {
            const char slash = '\\';
            const char quote = '"';
            var openBrace = string.Concat(quote, "{");
            var closeBrace = string.Concat("}", quote);
            var openBracket = string.Concat(quote, "[");
            var closeBracket = string.Concat("]", quote);
            var find = string.Concat(slash, quote);
            var serial = Uri.UnescapeDataString(json);
            serial = serial.Replace(find, quote.ToString());
            serial = serial.Replace(openBrace, "{");
            serial = serial.Replace(closeBrace, "}");
            serial = serial.Replace(openBracket, "[");
            serial = serial.Replace(closeBracket, "]");
            return serial;

        }
    }
}
