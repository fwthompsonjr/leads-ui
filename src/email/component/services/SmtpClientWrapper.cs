using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
namespace legallead.email.services
{
    [ExcludeFromCodeCoverage(Justification = "Wrapper class that interacts with remote smtp resources.")]
    public class SmtpClientWrapper(IConfiguration? configuration = null) : ISmtpClientWrapper
    {
        private readonly bool isEmailEnabled = EvaluateConfig(configuration);

        public bool EmailEnabled => isEmailEnabled;

        public void Send(SmtpClient client, MailMessage message)
        {
            if (!isEmailEnabled) { return; }
            client.Send(message);
        }

        public const string ConfigurationName = "EnableEmail";

        public static bool EvaluateConfig(IConfiguration? configuration = null)
        {
            if (configuration == null) { return true; }
            var keyvalue = configuration[ConfigurationName];
            if (string.IsNullOrEmpty(keyvalue)) { return true; }
            if (!bool.TryParse(keyvalue, out var isEnabled)) { return true; }
            return isEnabled;
        }
    }
}
