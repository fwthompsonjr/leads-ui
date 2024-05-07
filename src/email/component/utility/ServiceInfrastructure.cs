using legallead.email.actions;
using legallead.email.implementations;
using legallead.email.interfaces;
using legallead.email.services;
using legallead.email.transforms;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.email.utility
{

    using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
    public static class ServiceInfrastructure
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
                services.Initialize();
                return services.BuildServiceProvider();
            }
        }

        public static void Initialize(this IServiceCollection services, IConfiguration? configuration = null)
        {
            if (configuration != null)
            {
                services.AddSingleton(configuration);
            }
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
            services.AddKeyedTransient<IHtmlTransformDetailBase, BeginSearchRequestedTemplate>(TemplateNames.BeginSearchRequested.ToString());
            services.AddKeyedTransient<IHtmlTransformDetailBase, LockedAccountResponseTemplate>(TemplateNames.LockedAccountResponse.ToString());
            services.AddKeyedTransient<IHtmlTransformDetailBase, ProfileChangedTemplate>(TemplateNames.ProfileChanged.ToString());
            // end keyed transients
            // register actions
            services.AddTransient<RegistrationCompleted>();
            services.AddTransient<SearchPaymentCompleted>();
            services.AddTransient<BeginSearchRequested>();
            services.AddTransient<LockedAccountResponse>();
            services.AddTransient<ProfileChanged>();
            // end register actions
            services.AddTransient(x =>
            {
                var settings = x.GetRequiredService<ISettingsService>();
                var infra = x.GetRequiredService<IUserSettingInfrastructure>();
                var transform = x.GetRequiredService<IHtmlTransformService>();
                var beauty = x.GetRequiredService<IHtmlBeautifyService>();
                return new MailMessageService(settings, infra, transform, beauty);
            });
        }
    }
}
