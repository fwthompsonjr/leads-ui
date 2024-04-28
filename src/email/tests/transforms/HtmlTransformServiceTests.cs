using Bogus;
using legallead.email.implementations;
using legallead.email.interfaces;
using legallead.email.models;
using legallead.email.services;
using legallead.email.transforms;
using legallead.email.utility;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace legallead.email.tests.transforms
{
    public class HtmlTransformServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var obj = InitializeProvider();
            var actual = obj?.GetService<IHtmlTransformService>();
            Assert.NotNull(actual);
        }

        [Fact]
        public void ServiceContainsBaseHtml()
        {
            var obj = InitializeProvider();
            var actual = obj?.GetService<IHtmlTransformService>();
            var html = actual?.BaseHtml;
            Assert.False(string.IsNullOrWhiteSpace(html));
        }

        [Fact]
        public void ServiceContainsKeyNames()
        {
            var obj = InitializeProvider();
            var actual = obj?.GetService<IHtmlTransformService>();
            var names = actual?.KeyNames;
            Assert.NotNull(names);
            Assert.Equal(4, names.Count);
        }

        [Fact]
        public void ServiceContainsSubstitutions()
        {
            var obj = InitializeProvider();
            var actual = obj?.GetService<IHtmlTransformService>();
            var names = actual?.Substitutions;
            Assert.NotNull(names);
            Assert.Equal(4, names.Count);
        }

        [Theory]
        [InlineData("")]
        [InlineData("AccountRegistration")]
        [InlineData("accountRegistration")]
        [InlineData("AccountRegistration", false)]
        [InlineData("AccountRegistration", false, false)]
        [InlineData("AccountRegistration", true, true, null)]
        [InlineData("AccountRegistration", true, true, 0)]
        public async Task ServiceCanGetHtmlTemplate(string templateName, bool hasEmail = true, bool hasUserId = true, int? settingsCount = 4)
        {
            var obj = InitializeProvider();
            var query = queryfaker.Generate();
            if (!hasEmail) query.Email = string.Empty;
            if (!hasUserId) query.Id = string.Empty;
            var dbmock = obj.GetRequiredService<Mock<IUserSettingInfrastructure>>();
            var service = obj.GetRequiredService<IHtmlTransformService>();
            var settings = settingsCount switch
            {
                null => null,
                0 => faker.Generate(0),
                _ => GetDefaultSettings()
            };
            dbmock.Setup(m => m.GetSettings(It.IsAny<UserSettingQuery>())).ReturnsAsync(settings);

            var response = await service.GetHtmlTemplate(query, templateName);
            if (!hasEmail && !hasUserId) Assert.Null(response);
            else Assert.False(string.IsNullOrEmpty(response));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [InlineData(true, false)]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        [InlineData(true, true, true, false)]
        public async Task ServiceCanExtractUserName(
            bool hasUserName = true,
            bool hasFirstName = true,
            bool hasLastName = true,
            bool hasAttributes = true)
        {
            const string templateName = "AccountRegistration";
            var obj = InitializeProvider();
            var query = queryfaker.Generate();
            var dbmock = obj.GetRequiredService<Mock<IUserSettingInfrastructure>>();
            var service = obj.GetRequiredService<IHtmlTransformService>();
            var settings = hasAttributes ? GetDefaultSettings() : [];
            if (!hasUserName) settings.ForEach(s => { s.UserName = null; });
            if (!hasFirstName) settings.RemoveAll(s => { return s.KeyName == "First Name"; });
            if (!hasLastName) settings.RemoveAll(s => { return s.KeyName == "Last Name"; });
            dbmock.Setup(m => m.GetSettings(It.IsAny<UserSettingQuery>())).ReturnsAsync(settings);
            var response = await service.GetHtmlTemplate(query, templateName);
            Assert.False(string.IsNullOrEmpty(response));
        }

        private static readonly object locker = new();
        private static ServiceProvider InitializeProvider()
        {
            lock (locker)
            {
                var services = new ServiceCollection();
                var connectionMock = new Mock<IConnectionStringService>();
                var cryptoMock = new Mock<ICryptographyService>();
                var dbMock = new Mock<IDataCommandService>();
                var dbConnectionMock = new Mock<IDataConnectionService>();
                var settingsMock = new Mock<ISettingsService>();
                var smtpWrapperMock = new Mock<ISmtpClientWrapper>();
                var smtpMock = new Mock<ISmtpService>();
                var userDbMock = new Mock<IUserSettingInfrastructure>();
                // add mocks
                services.AddSingleton(connectionMock);
                services.AddSingleton(cryptoMock);
                services.AddSingleton(dbMock);
                services.AddSingleton(dbConnectionMock);
                services.AddSingleton(settingsMock);
                services.AddSingleton(smtpWrapperMock);
                services.AddSingleton(smtpMock);
                services.AddSingleton(userDbMock);
                // add implementations
                services.AddSingleton(connectionMock.Object);
                services.AddSingleton(cryptoMock.Object);
                services.AddSingleton(dbMock.Object);
                services.AddSingleton(dbConnectionMock.Object);
                services.AddSingleton(settingsMock.Object);
                services.AddSingleton(smtpWrapperMock.Object);
                services.AddSingleton(smtpMock.Object);
                services.AddSingleton(userDbMock.Object);
                services.AddTransient<IHtmlTransformService, HtmlTransformService>();
                services.AddKeyedTransient<IHtmlTransformDetailBase, AccountRegistrationTemplate>("AccountRegistration");
                var provider = services.BuildServiceProvider();
                ServiceInfrastructure.Provider = provider;
                return provider;
            }
        }
        private static List<UserEmailSettingBo> GetDefaultSettings()
        {
            var list = new List<UserEmailSettingBo>
            {
                new()
            };
            commonKeys.ForEach(k =>
            {
                var item = faker.Generate();
                item.KeyName = k;
                if (k == "Person") item.KeyValue = new Faker().Person.FullName;
                list.Add(item);
            });
            var template = list[0];
            list.ForEach(a =>
            {
                a.Id = template.Id;
                a.Email = template.Email;
                a.UserName = template.UserName;
            });
            return list;
        }


        private static readonly List<string> commonKeys =
        [
            "Email 1",
            "Email 2",
            "Email 3",
            "First Name",
            "Last Name"
        ];

        private static readonly Faker<UserSettingQuery> queryfaker =
            new Faker<UserSettingQuery>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email);

        private static readonly Faker<UserEmailSettingBo> faker =
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
                    _ => a.Person.Email
                };
            });
    }
}
