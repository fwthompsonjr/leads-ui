using legallead.email.models;

namespace legallead.email.services
{
    public interface ISettingsService
    {
        MailSettings GetSettings { get; }
    }
}