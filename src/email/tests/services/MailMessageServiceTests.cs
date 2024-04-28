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
    public class MailMessageServiceTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var obj = InitializeProvider();
            var actual = obj?.GetService<MailMessageService>();
            Assert.NotNull(actual);
        }

        [Theory]
        [InlineData(TemplateNames.AccountRegistration, "", "abcd@email.com", true)]
        [InlineData(TemplateNames.AccountRegistration, "not-a-guid", "abcd@email.com", true)]
        [InlineData(TemplateNames.AccountRegistration, "fbd8493d-289c-40ba-84f7-daaeb8015c35", "", true)]
        public void SutCanMailMessageService(
            TemplateNames template,
            string userId,
            string userEmail,
            bool canSend)
        {
            var obj = InitializeProvider();
            var actual = obj.GetService<MailMessageService>();
            var list = obj.GetService<List<UserEmailSettingBo>>();
            if (list != null)
            {
                var fkr = new Faker();
                list.ForEach(a =>
                {
                    if (a.KeyName != null && a.KeyName.StartsWith("Email"))
                    {
                        a.KeyValue = fkr.Person.Email;
                    }
                });
            }
            Assert.NotNull(actual);
            actual = actual.With(template, userId, userEmail);
            Assert.Equal(canSend, actual.CanSend());
        }


        [Theory]
        [InlineData("", "abcd@email.com")]
        [InlineData("not-a-guid", "abcd@email.com")]
        [InlineData("fbd8493d-289c-40ba-84f7-daaeb8015c35", "")]
        [InlineData("fbd8493d-289c-40ba-84f7-daaeb8015c35", "", null)]
        [InlineData("fbd8493d-289c-40ba-84f7-daaeb8015c35", "", 0)]
        [InlineData("fbd8493d-289c-40ba-84f7-daaeb8015c35", "", 5, false)]
        [InlineData("fbd8493d-289c-40ba-84f7-daaeb8015c35", "not-email-address", 5, false)]
        [InlineData("fbd8493d-289c-40ba-84f7-daaeb8015c35", "", 5, true, false)]
        [InlineData("fbd8493d-289c-40ba-84f7-daaeb8015c35", "", 5, true, false, false)]
        [InlineData("fbd8493d-289c-40ba-84f7-daaeb8015c35", "", 5, true, true, true, true)]
        public void SutCanGetToMailAddress(
            string userId,
            string userEmail,
            int? settingsCount = 5,
            bool hasEmail = true,
            bool hasFirstName = true,
            bool hasLastName = true,
            bool firstNameIsNull = false,
            bool lastNameIsNull = false)
        {
            var exception = Record.Exception(() =>
            {
                const TemplateNames template = TemplateNames.AccountRegistration;
                var obj = InitializeProvider();
                var actual = obj.GetRequiredService<MailMessageService>();
                var collection = settingsCount switch
                {
                    null => null,
                    0 => faker.Generate(0),
                    _ => GetDefaultSettings()
                };
                if (!hasEmail && collection != null)
                {
                    collection.ForEach(c => c.Email = userEmail);
                }
                if (!hasFirstName && collection != null)
                {
                    collection.ForEach(c =>
                    {
                        if ((c.KeyName ?? "").Equals("First Name", StringComparison.OrdinalIgnoreCase))
                        {
                            c.KeyValue = "";

                        }
                    });
                }
                if (firstNameIsNull && collection != null)
                {
                    collection.RemoveAll(c =>
                    {
                        return ((c.KeyName ?? "").Equals("First Name", StringComparison.OrdinalIgnoreCase));
                    });
                }
                if (!hasLastName && collection != null)
                {
                    collection.ForEach(c =>
                    {
                        if ((c.KeyName ?? "").Equals("Last Name", StringComparison.OrdinalIgnoreCase))
                        {
                            c.KeyValue = "";

                        }
                    });
                }
                if (lastNameIsNull && collection != null)
                {
                    collection.RemoveAll(c =>
                    {
                        return ((c.KeyName ?? "").Equals("Last Name", StringComparison.OrdinalIgnoreCase));
                    });
                }
                var infra = obj.GetRequiredService<Mock<IUserSettingInfrastructure>>();
                infra.Setup(s => s.GetSettings(It.IsAny<UserSettingQuery>())).ReturnsAsync(collection);
                actual = actual.With(template, userId, userEmail);
                _ = actual.CanSend();
            });
            Assert.Null(exception);
        }


        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        [InlineData(2)]
        public void SutCanGetCcMailAddresses(
            int? emailCount = 3)
        {
            var exception = Record.Exception(() =>
            {
                const TemplateNames template = TemplateNames.AccountRegistration;
                const string userId = "fbd8493d-289c-40ba-84f7-daaeb8015c35";
                const string userEmail = "abcd@testing.com";
                string[] collection1 = ["Email 1", "Email 2"];
                string[] collection2 = ["Email 3"];
                var obj = InitializeProvider();
                var actual = obj.GetRequiredService<MailMessageService>();
                var collection = GetDefaultSettings();
                var fkr = new Faker();
                collection.ForEach(a =>
                {
                    if (a.KeyName != null && a.KeyName.StartsWith("Email"))
                    {
                        a.KeyValue = fkr.Person.Email;
                    }
                });
                if (emailCount == null) collection.RemoveAll(c => { return (c.KeyName ?? "").StartsWith("Email"); });
                if (emailCount.GetValueOrDefault() == 1)
                {
                    collection.RemoveAll(c => { return collection1.Contains(c.KeyName ?? ""); });
                }
                if (emailCount.GetValueOrDefault() == 2)
                {
                    collection.RemoveAll(c => { return collection2.Contains(c.KeyName ?? ""); });
                }
                var infra = obj.GetRequiredService<Mock<IUserSettingInfrastructure>>();
                infra.Setup(s => s.GetSettings(It.IsAny<UserSettingQuery>())).ReturnsAsync(collection);
                actual = actual.With(template, userId, userEmail);
                _ = actual.CanSend();
            });
            Assert.Null(exception);
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
                var smtpWrapperMock = new Mock<ISmtpClientWrapper>();
                var smtpMock = new Mock<ISmtpService>();
                var userDbMock = new Mock<IUserSettingInfrastructure>();
                var collection = GetDefaultSettings();
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
                services.AddTransient<IHtmlTransformService, HtmlTransformService>();
                services.AddKeyedTransient<IHtmlTransformDetailBase, AccountRegistrationTemplate>("AccountRegistration");
                services.AddTransient<MailMessageService>();
                var provider = services.BuildServiceProvider();
                ServiceInfrastructure.Provider = provider;
                return provider;
            }
        }
        private static List<UserEmailSettingBo> GetDefaultSettings()
        {
            var list = new List<UserEmailSettingBo>
            {
                new ()
            };
            commonKeys.ForEach(k =>
            {
                var item = faker.Generate();
                item.KeyName = k;
                if (k == "Person") item.KeyValue = new Faker().Person.FullName;
                list.Add(item);
            });
            var template = list[1];
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
                    _ => b.KeyValue
                };
            });
    }
}

