using legallead.desktop.entities;
using legallead.desktop.implementations;
using legallead.desktop.interfaces;
using legallead.desktop.services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace legallead.desktop.utilities
{
    internal static class AppBuilder
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        public static IConfiguration? Configuration { get; private set; }

        public static string? PaymentSessionKey { get; private set; }
        public static string? PermissionApiBase { get; private set; }
        public static string? InitialViewName { get; private set; }
        public static BackgroundQueueServices? QueueService { get; internal set; }
        public static BackgroundMailService? MailService { get; internal set; }

        public static void Build()
        {
            if (Configuration == null)
            {
                var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    builder.AddJsonFile("appsettings.debug.json", optional: true, reloadOnChange: true);
                }
                Configuration = builder.Build();
            }
            if (string.IsNullOrEmpty(PermissionApiBase))
            {
                PermissionApiBase = GetPermissionApi(Configuration);
            }
            if (string.IsNullOrEmpty(PaymentSessionKey))
            {
                PaymentSessionKey = GetPaymentKey(Configuration);
            }
            if (string.IsNullOrEmpty(InitialViewName))
            {
                InitialViewName = Configuration["Initial_View"] ?? "introduction";
            }
            if (ServiceProvider == null)
            {
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                ServiceProvider = serviceCollection.BuildServiceProvider();
            }
        }

        private static string GetPermissionApi(IConfiguration configuration)
        {
            var keys = new[] {
              "Permissions_API",
              "api.permissions:destination",
              "api.permissions:remote",
              "api.permissions:local" }.ToList();
            var keyvalues = new List<string> { };
            foreach (var item in keys)
            {
                var value = configuration[item] ?? string.Empty;
                keyvalues.Add(value);
            }
            if (string.IsNullOrEmpty(keyvalues[1])) return keyvalues[0];
            return keyvalues[1] == "local" ? keyvalues[3] : keyvalues[2];
        }

        private static string GetPaymentKey(IConfiguration configuration)
        {
            var keys = new[] {
              "stripe.payment:key",
              "stripe.payment:names:test",
              "stripe.payment:names:prod", }.ToList();
            var keyvalues = new List<string> { };
            foreach (var item in keys)
            {
                var value = configuration[item] ?? string.Empty;
                keyvalues.Add(value);
            }
            if (string.IsNullOrEmpty(keyvalues[0])) return string.Empty;
            return keyvalues[0] == "test" ? keyvalues[1] : keyvalues[2];
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var provider = DesktopCoreServiceProvider.Provider;
            services.AddSingleton<UserBo>();
            services.AddSingleton<UserSearchBo>();
            services.AddSingleton<IUserSearchMapper>(s => new UserSearchMapper());
            services.AddSingleton<IUserRestrictionMapper>(s => new UserRestrictionMapper());
            services.AddTransient<IPermissionApi>(s => new PermissionPageClient(PermissionApiBase ?? string.Empty));
            services.AddSingleton<ISearchBuilder>(s =>
            {
                var api = s.GetRequiredService<IPermissionApi>();
                return new SearchBuilder(api);
            });
            services.AddSingleton(typeof(MainWindow));
            if (provider == null) return;
            services.AddTransient(s => provider.GetRequiredService<IContentParser>());
            services.AddSingleton(s => provider.GetRequiredService<IInternetStatus>());
            services.AddSingleton(s => provider.GetRequiredService<MenuConfiguration>());
            services.AddSingleton(s => provider.GetRequiredService<IErrorContentProvider>());
            services.AddSingleton(s => provider.GetRequiredService<IUserProfileMapper>());
            services.AddSingleton(s => provider.GetRequiredService<IUserPermissionsMapper>());
            services.AddSingleton(s => provider.GetRequiredService<ICopyrightBuilder>());
            services.AddSingleton(s => provider.GetRequiredService<IQueueStopper>());
            services.AddSingleton(s => provider.GetRequiredService<IQueueStarter>());
            services.AddSingleton(s => provider.GetRequiredService<IMailPersistence>());
            services.AddSingleton(s => provider.GetRequiredService<IMailReader>());
            services.AddSingleton(s => provider.GetRequiredService<CommonMessageList>());
            services.AddSingleton(s =>
            {
                var list = s.GetRequiredService<CommonMessageList>();
                return new CommonStatusHelper(list);
            });
        }
    }
}