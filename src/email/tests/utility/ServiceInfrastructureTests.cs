using legallead.email.interfaces;
using legallead.email.services;
using legallead.email.transforms;
using legallead.email.utility;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.email.tests.utility
{
    using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
    public class ServiceInfrastructureTests
    {
        [Fact]
        public void SutExposesServiceProvider()
        {
            var obj = ServiceInfrastructure.Provider;
            Assert.NotNull(obj);
        }

        [Theory]
        [InlineData(typeof(IConnectionStringService))]
        [InlineData(typeof(ICryptographyService))]
        [InlineData(typeof(IDataCommandService))]
        [InlineData(typeof(IDataConnectionService))]
        [InlineData(typeof(ISettingsService))]
        [InlineData(typeof(ISmtpClientWrapper))]
        [InlineData(typeof(ISmtpService))]
        [InlineData(typeof(IUserSettingInfrastructure))]
        [InlineData(typeof(IHtmlTransformService))]
        [InlineData(typeof(MailMessageService))]
        public void SutCanGetType(Type serviceType)
        {

            var obj = ServiceInfrastructure.Provider;
            var actual = obj?.GetService(serviceType);
            Assert.NotNull(actual);
        }

        [Fact]
        public void SutCanGetTemplates()
        {
            var names = Enum.GetNames<TemplateNames>().ToList();
            var expected = names.Count - 1; // exclude none
            var obj = ServiceInfrastructure.Provider;
            var found = 0;
            names.ForEach(n =>
            {
                var actual = obj?.GetKeyedService<IHtmlTransformDetailBase>(n);
                if (actual != null) found++;
            });
            Assert.Equal(expected, found);
        }

        [Theory]
        [InlineData(typeof(IConnectionStringService))]
        [InlineData(typeof(ICryptographyService))]
        [InlineData(typeof(IDataCommandService))]
        [InlineData(typeof(IDataConnectionService))]
        [InlineData(typeof(ISettingsService))]
        [InlineData(typeof(ISmtpClientWrapper))]
        [InlineData(typeof(ISmtpService))]
        [InlineData(typeof(IUserSettingInfrastructure))]
        [InlineData(typeof(IHtmlTransformService))]
        [InlineData(typeof(MailMessageService))]
        [InlineData(typeof(ISmtpClientWrapper), "false")]
        [InlineData(typeof(ISmtpClientWrapper), "0")]
        [InlineData(typeof(ISmtpClientWrapper), "777-9311")]
        [InlineData(typeof(ISmtpClientWrapper), null)]
        public void SutCanInjectConfiguration(Type serviceType, string? emailEnabled = "true")
        {
            var config = GetConfiguration(emailEnabled);
            var collection = new ServiceCollection();
            collection.Initialize(config);
            var obj = collection.BuildServiceProvider();
            var actual = obj?.GetService(serviceType);
            if (actual is ISmtpClientWrapper wrapper)
            {
                var expected = SmtpClientWrapper.EvaluateConfig(config);
                Assert.Equal(expected, wrapper.EmailEnabled);
            }
            Assert.NotNull(actual);
        }

        private static IConfiguration? GetConfiguration(string? keyValue)
        {
            if (string.IsNullOrEmpty(keyValue)) return null;
            var keyName = SmtpClientWrapper.ConfigurationName;
            var mock = new Mock<IConfiguration>();
            mock.Setup(s => s[It.Is<string>(x => x == keyName)]).Returns(keyValue);
            return mock.Object;
        }
    }
}
