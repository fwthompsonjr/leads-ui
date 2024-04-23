using legallead.email.models;
using Newtonsoft.Json;

namespace legallead.email.services
{
    internal class SettingsService : ISettingsService
    {
        private static readonly string SmtpContent =
            Properties.Resources.smtp_settings;

        private readonly MailSettings settings;

        public SettingsService()
        {
            var serialized = JsonConvert.DeserializeObject<MailSettings>(SmtpContent) ?? new();
            settings = serialized;
        }

        public virtual MailSettings GetSettings => settings;
    }
}
