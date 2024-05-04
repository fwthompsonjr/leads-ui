using legallead.email.actions;
using legallead.email.implementations;
using legallead.email.interfaces;
using legallead.email.services;
using legallead.email.transforms;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.email.utility
{
    internal static class ServiceInfrastructure
    {
        private static readonly object locker = new();
        private static IServiceProvider? _provider;
        public static IServiceProvider? Provider
        {
            get { return _provider ??= InitializeProvider(); }
            internal set { _provider = value; }
        }

        private static ServiceProvider? InitializeProvider()
        {
            lock (locker)
            {
                var services = new ServiceCollection();
                services.AddSingleton<IConnectionStringService, ConnectionStringService>();
                services.AddSingleton<ICryptographyService, CryptographyService>();
                services.AddSingleton<IDataCommandService, DataCommandService>();
                services.AddSingleton<IDataConnectionService, DataConnectionService>();
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddSingleton<ISmtpClientWrapper, SmtpClientWrapper>();
                services.AddSingleton<IMailLoggingService, MailLoggingService>();
                services.AddSingleton<ISmtpService, SmtpService>();
                services.AddSingleton<IUserSettingInfrastructure, UserSettingInfrastructure>();
                services.AddSingleton<IHtmlBeautifyService, HtmlBeautifyService>();
                // transients
                services.AddTransient<IHtmlTransformService, HtmlTransformService>();
                services.AddKeyedTransient<IHtmlTransformDetailBase, RegistrationCompletedTemplate>(TemplateNames.RegistrationCompleted.ToString());
                services.AddKeyedTransient<IHtmlTransformDetailBase, SearchPaymentCompletedTemplate>(TemplateNames.SearchPaymentCompleted.ToString());
                // end keyed transients
                // register actions
                services.AddTransient<RegistrationCompleted>();
                services.AddTransient<SearchPaymentCompleted>();
                // end register actions
                services.AddTransient(x =>
                {
                    var settings = x.GetRequiredService<ISettingsService>();
                    var infra = x.GetRequiredService<IUserSettingInfrastructure>();
                    var transform = x.GetRequiredService<IHtmlTransformService>();
                    var beauty = x.GetRequiredService<IHtmlBeautifyService>();
                    return new MailMessageService(settings, infra, transform, beauty);
                });
                return services.BuildServiceProvider();
            }
        }
    }
}
