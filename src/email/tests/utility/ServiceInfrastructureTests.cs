using legallead.email.interfaces;
using legallead.email.services;
using legallead.email.transforms;
using legallead.email.utility;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.email.tests.utility
{
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
        public void SutCanGetType(Type serviceType)
        {

            var obj = ServiceInfrastructure.Provider;
            var actual = obj?.GetService(serviceType);
            Assert.NotNull(actual);
        }

        [Fact]
        public void SutCanTemplates()
        {
            var names = Enum.GetNames<TemplateNames>().ToList();
            var expected = names.Count;
            var obj = ServiceInfrastructure.Provider;
            var found = 0;
            names.ForEach(n =>
            {
                var actual = obj?.GetKeyedService<IHtmlTransformDetailBase>(n);
                if (actual != null) found++;
            });
            Assert.Equal(expected, found);
        }
    }
}
