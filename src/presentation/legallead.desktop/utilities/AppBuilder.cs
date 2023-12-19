using legallead.desktop.entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.utilities
{
    internal static class AppBuilder
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        public static IConfiguration? Configuration { get; private set; }

        public static string? PermissionApiBase { get; private set; }

        public static void Build()
        {
            if (Configuration == null)
            {
                var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                Configuration = builder.Build();
            }
            if (string.IsNullOrEmpty(PermissionApiBase))
            {
                PermissionApiBase = Configuration["Permissions_API"] ?? string.Empty;
            }
            if (ServiceProvider == null)
            {
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                ServiceProvider = serviceCollection.BuildServiceProvider();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<UserBo>();
            services.AddTransient(s => new PermissionApi(PermissionApiBase ?? string.Empty));
            services.AddTransient(typeof(MainWindow));
        }
    }
}