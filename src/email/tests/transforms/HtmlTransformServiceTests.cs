using Bogus;
using legallead.email.interfaces;
using legallead.email.models;
using legallead.email.transforms;
using legallead.email.utility;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace legallead.email.tests.transforms
{
    public class HtmlTransformServiceTests : IDisposable
    {
        public HtmlTransformServiceTests()
        {
            _ = InitializeProvider();
        }
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
                0 => MockMessageInfrastructure.UserEmailFaker.Generate(0),
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
            return MockMessageInfrastructure.GetServiceProvider(true);
        }

        private static List<UserEmailSettingBo> GetDefaultSettings()
        {
            return MockMessageInfrastructure.GetDefaultSettings(true);
        }

        private static readonly Faker<UserSettingQuery> queryfaker =
            new Faker<UserSettingQuery>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Email, y => y.Person.Email);

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
