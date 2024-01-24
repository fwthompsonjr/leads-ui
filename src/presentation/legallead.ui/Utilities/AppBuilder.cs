using legallead.desktop.entities;
using legallead.desktop.implementations;
using legallead.desktop.interfaces;
using legallead.ui.Models;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace legallead.desktop.utilities
{
    internal static class AppBuilder
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        public static IConfiguration? Configuration { get; private set; }

        public static string? PermissionApiBase { get; private set; }
        public static string? InitialViewName { get; private set; }

        public static void Build()
        {
            if (Configuration == null)
            {
                var settingjs = GetConfiguration(1);
                var debugjs = GetConfiguration(0);
                var builder = new ConfigurationBuilder();
                if (settingjs != null) builder.AddJsonStream(settingjs);
                if (System.Diagnostics.Debugger.IsAttached && debugjs != null)
                {
                    builder.AddJsonStream(debugjs);
                }

                Configuration = builder.Build();
            }
            if (string.IsNullOrEmpty(PermissionApiBase))
            {
                PermissionApiBase = GetPermissionApi(Configuration);
            }
            if (string.IsNullOrEmpty(InitialViewName))
            {
                InitialViewName = Configuration["Initial_View"] ?? "introduction";
            }
            if (ServiceProvider == null)
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddSingleton(Configuration);
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

        private static void ConfigureServices(IServiceCollection services)
        {
            var provider = DesktopCoreServiceProvider.Provider;
            services.AddSingleton<UserBo>();
            services.AddSingleton<UserSearchBo>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<IPermissionApi>(s => new PermissionPageClient(PermissionApiBase ?? string.Empty));
            services.AddSingleton<ISearchBuilder>(s =>
            {
                var api = s.GetRequiredService<IPermissionApi>();
                return new SearchBuilder(api);
            });
            if (provider == null) return;
            services.AddTransient(s => provider.GetRequiredService<IContentParser>());
            services.AddSingleton(s => provider.GetRequiredService<IInternetStatus>());
            services.AddSingleton(s => provider.GetRequiredService<MenuConfiguration>());
            services.AddSingleton(s => provider.GetRequiredService<IErrorContentProvider>());
            services.AddSingleton(s => provider.GetRequiredService<IUserProfileMapper>());
            services.AddSingleton(s => provider.GetRequiredService<IUserPermissionsMapper>());
            services.AddSingleton(s => provider.GetRequiredService<ICopyrightBuilder>());
            services.AddSingleton(s => provider.GetRequiredService<CommonMessageList>());
        }

        private static Stream GetConfiguration(int index)
        {
            var app = Properties.Resources.appsettings;
            var debug = Properties.Resources.appsettings_debug;
            var content = index == 0 ? debug : app;
            byte[] array = Encoding.UTF8.GetBytes(content);
            return new MemoryStream(array);
        }
    }
}