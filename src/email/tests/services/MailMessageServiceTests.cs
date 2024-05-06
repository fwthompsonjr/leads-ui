using legallead.email.interfaces;
using legallead.email.models;
using legallead.email.services;
using legallead.email.utility;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.email.tests.transforms
{
    public class MailMessageServiceTests : IDisposable
    {
        public MailMessageServiceTests()
        {
            _ = InitializeProvider();
        }
        [Fact]
        public void ServiceCanBeCreated()
        {
            var obj = InitializeProvider();
            var actual = obj?.GetService<MailMessageService>();
            Assert.NotNull(actual);
        }

        [Theory]
        [InlineData(TemplateNames.RegistrationCompleted, "", "abcd@email.com", true)]
        [InlineData(TemplateNames.RegistrationCompleted, "not-a-guid", "abcd@email.com", true)]
        [InlineData(TemplateNames.RegistrationCompleted, "fbd8493d-289c-40ba-84f7-daaeb8015c35", "", true)]
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
                const TemplateNames template = TemplateNames.RegistrationCompleted;
                var obj = InitializeProvider();
                var actual = obj.GetRequiredService<MailMessageService>();
                var collection = settingsCount switch
                {
                    null => null,
                    0 => MockMessageInfrastructure.UserEmailFaker.Generate(0),
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
                const TemplateNames template = TemplateNames.RegistrationCompleted;
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
            return MockMessageInfrastructure.GetServiceProvider(true);
        }
        private static List<UserEmailSettingBo> GetDefaultSettings()
        {
            return MockMessageInfrastructure.GetDefaultSettings(true);
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    lock (locker)
                    {
                        ServiceInfrastructure.Provider = null;
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

