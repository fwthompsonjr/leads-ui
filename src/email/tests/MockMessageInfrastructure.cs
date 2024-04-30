using Bogus;
using legallead.email.actions;
using legallead.email.implementations;
using legallead.email.interfaces;
using legallead.email.models;
using legallead.email.services;
using legallead.email.transforms;
using legallead.email.utility;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace legallead.email.tests
{
    internal static class MockMessageInfrastructure
    {
        public static List<UserEmailSettingBo> GetDefaultSettings(bool includeEmptyEntry = false)
        {
            var list = includeEmptyEntry ?
                new List<UserEmailSettingBo> { new() } :
                [];
            var helper = new Faker();
            commonKeys.ForEach(k =>
            {
                var item = UserEmailFaker.Generate();
                item.KeyName = k;
                if (k == "Person") item.KeyValue = helper.Person.FullName;
                if (k == "Email 1") item.KeyValue = helper.Person.Email;
                if (k == "Email 2") item.KeyValue = helper.Person.Email;
                if (k == "Email 3") item.KeyValue = helper.Person.Email;
                if (k == "First Name") item.KeyValue = helper.Person.FirstName;
                if (k == "Last Name") item.KeyValue = helper.Person.LastName;
                list.Add(item);
            });
            var template = list.Find(x => !string.IsNullOrEmpty(x.Email));
            if (template == null) return list;
            list.ForEach(a =>
            {
                a.Id = template.Id;
                a.Email = template.Email;
                a.UserName = template.UserName;
            });
            return list;
        }

        public static ServiceProvider GetServiceProvider(bool includeEmptyEntry = false)
        {
            lock (locker)
            {
                var services = new ServiceCollection();
                var connectionMock = new Mock<IConnectionStringService>();
                var cryptoMock = new Mock<ICryptographyService>();
                var dbMock = new Mock<IDataCommandService>();
                var dbConnectionMock = new Mock<IDataConnectionService>();
                var smtpWrapperMock = new Mock<ISmtpClientWrapper>();
                var smtpMock = new Mock<ISmtpService>();
                var userDbMock = new Mock<IUserSettingInfrastructure>();
                var collection = GetDefaultSettings(includeEmptyEntry);
                userDbMock.Setup(s => s.GetSettings(It.IsAny<UserSettingQuery>())).ReturnsAsync(collection);
                // add mocks
                services.AddSingleton(collection);
                services.AddSingleton(connectionMock);
                services.AddSingleton(cryptoMock);
                services.AddSingleton(dbMock);
                services.AddSingleton(dbConnectionMock);
                services.AddSingleton(smtpWrapperMock);
                services.AddSingleton(smtpMock);
                services.AddSingleton(userDbMock);
                // add implementations
                services.AddSingleton(connectionMock.Object);
                services.AddSingleton(cryptoMock.Object);
                services.AddSingleton(dbMock.Object);
                services.AddSingleton(dbConnectionMock.Object);
                services.AddSingleton(smtpWrapperMock.Object);
                services.AddSingleton(smtpMock.Object);
                services.AddSingleton(userDbMock.Object);
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddSingleton<IHtmlBeautifyService, HtmlBeautifyService>();
                services.AddTransient<IHtmlTransformService, HtmlTransformService>();
                services.AddKeyedTransient<IHtmlTransformDetailBase, RegistrationCompletedTemplate>(TemplateNames.RegistrationCompleted.ToString());
                services.AddKeyedTransient<IHtmlTransformDetailBase, SearchPaymentCompletedTemplate>(TemplateNames.SearchPaymentCompleted.ToString());
                services.AddTransient<RegistrationCompleted>();
                services.AddTransient<SearchPaymentCompletedTemplate>();
                services.AddTransient(x =>
                {
                    var settings = x.GetRequiredService<ISettingsService>();
                    var infra = userDbMock.Object;
                    var transform = x.GetRequiredService<IHtmlTransformService>();
                    var beauty = x.GetRequiredService<IHtmlBeautifyService>();
                    return new MailMessageService(settings, infra, transform, beauty);
                });
                services.AddMvcCore(options =>
                {
                    options.Filters.AddService<RegistrationCompleted>();
                });
                var provider = services.BuildServiceProvider();
                ServiceInfrastructure.Provider = provider;
                return provider;
            }

        }


        internal static readonly Faker<UserEmailSettingBo> UserEmailFaker =
            new Faker<UserEmailSettingBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.KeyName, y => y.PickRandom(commonKeys))
            .FinishWith((a, b) =>
            {
                b.KeyValue = b.KeyName switch
                {
                    "First Name" => a.Person.FirstName,
                    "Last Name" => a.Person.LastName,
                    _ => b.KeyValue
                };
            });

        private static readonly List<string> commonKeys =
        [
            "Email 1",
            "Email 2",
            "Email 3",
            "First Name",
            "Last Name"
        ];

        private static readonly object locker = new();
    }
}
