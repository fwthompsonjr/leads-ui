using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace db.integration.tests
{
    internal static class ServiceSetup
    {
        public static IServiceProvider AppServices
        {
            get
            {
                while (Provider == null) { Init(); }
                return Provider;
            }
        }
        private static void Init()
        {
            if (Provider != null) return;
            const string environ = "Test";
            lock (locker)
            {
                var services = new ServiceCollection();
                services.AddScoped<IDapperCommand, DapperExecutor>();
                services.AddSingleton<IDataInitializer, DataInitializer>();
                services.AddScoped(d =>
                {
                    var command = d.GetRequiredService<IDapperCommand>();
                    var dbint = d.GetRequiredService<IDataInitializer>();
                    return new DataContext(command, dbint, environ);
                });
                services.AddScoped<ILeadUserRepository, LeadUserRepository>();
                services.AddScoped<IInvoiceRepository, InvoiceRepository>();
                services.AddScoped<IUserUsageRepository, UserUsageRepository>();
                Provider = services.BuildServiceProvider();
            }
        }

        private static IServiceProvider? Provider = null;
        private static readonly object locker = new();
    }
}
