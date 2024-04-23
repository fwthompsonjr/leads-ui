
using Bogus;
using legallead.email.models;

namespace legallead.email.tests.models
{
    public class MailSettingsTests
    {
        private static readonly Faker<MailSettings.SmtpSettings> smtpFaker
            = new Faker<MailSettings.SmtpSettings>()
            .RuleFor(x => x.Endpoint, y => y.Internet.Url())
            .RuleFor(x => x.Port, y => y.Random.Int(20, 5000))
            .RuleFor(x => x.From, y =>
            {
                var email = y.Person.Email;
                var displayName = y.Person.FullName;
                return new() { Email = email, DisplayName = displayName };
            });

        private static readonly Faker<MailSettings> faker =
            new Faker<MailSettings>()
            .RuleFor(x => x.Account, y => y.Random.AlphaNumeric(30))
            .RuleFor(x => x.Uid, y => y.Random.AlphaNumeric(30))
            .RuleFor(x => x.Secret, y => y.Random.AlphaNumeric(30))
            .RuleFor(x => x.MailType, y => y.Lorem.Word())
            .RuleFor(x => x.Settings, y => smtpFaker.Generate());

        [Fact]
        public void SettingCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }
    }
}
