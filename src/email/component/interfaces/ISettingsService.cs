using legallead.email.models;

namespace legallead.email.services
{
    internal interface ISettingsService
    {
        MailSettings GetSettings { get; }
    }
}