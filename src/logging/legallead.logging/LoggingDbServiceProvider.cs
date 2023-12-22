using legallead.logging.helpers;
using legallead.logging.implementations;
using legallead.logging.interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.logging
{
    public class LoggingDbServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public LoggingDbServiceProvider()
        {
            var builder = new ServiceCollection();
            builder.AddSingleton<ILogConfiguration, LogConfiguration>();
            builder.AddScoped<ILoggingDbCommand, LoggingDbExecutor>();
            builder.AddScoped<ILoggingDbContext, LoggingDbContext>();
            builder.AddScoped<ILogContentRepository, LogContentRepository>();
            builder.AddScoped<ILoggingService>(a =>
            {
                var cfg = a.GetRequiredService<ILogConfiguration>();
                var repo = a.GetRequiredService<ILogContentRepository>();
                return new LoggingService(null, repo, cfg);
            });
            _serviceProvider = builder.BuildServiceProvider();
        }

        public IServiceProvider Provider => _serviceProvider;
    }
}