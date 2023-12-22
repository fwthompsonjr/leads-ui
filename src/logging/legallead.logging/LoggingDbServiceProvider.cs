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
            builder.AddScoped<ILoggingDbCommand, LoggingDbExecutor>();
            builder.AddScoped<ILoggingDbContext, LoggingDbContext>();
            builder.AddScoped<ILogContentRepository, LogContentRepository>();
            _serviceProvider = builder.BuildServiceProvider();
        }

        public IServiceProvider Provider => _serviceProvider;
    }
}