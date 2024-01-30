using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.logging;
using legallead.logging.helpers;
using legallead.logging.implementations;
using legallead.logging.interfaces;
using legallead.search.api.Controllers;
using legallead.search.api.Health;
using legallead.search.api.Models;
using legallead.search.api.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace legallead.search.api.Utility
{
    public static class ServiceExtensions
    {
        public static void Initialize(this IServiceCollection services, IConfiguration? configuration = null)
        {
            string environ = GetConfigOrDefault(configuration, "DataEnvironment", "Test");
            var setting = new BackgroundServiceSettings
            {
                Enabled = configuration?.GetValue<bool>("BackgroundServices:Enabled") ?? true,
                Delay = configuration?.GetValue<int>("BackgroundServices:Delay") ?? 45,
                Interval = configuration?.GetValue<int>("BackgroundServices:Interval") ?? 10
            };
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSingleton<IBackgroundServiceSettings>(x => setting);
            services.AddSingleton<IDapperCommand, DapperExecutor>();
            services.AddSingleton(x =>
            {
                var command = x.GetRequiredService<IDapperCommand>();
                return new DataContext(command, null, environ, "app");
            });
            services.AddSingleton<IUserSearchRepository, UserSearchRepository>(x =>
            {
                var context = x.GetRequiredService<DataContext>();
                return new UserSearchRepository(context);
            });
            services.AddSingleton<ISearchQueueRepository, SearchQueueRepository>(x =>
            {
                var context = x.GetRequiredService<DataContext>();
                var service = x.GetRequiredService<IUserSearchRepository>();
                return new SearchQueueRepository(context, service);
            });
            services.AddSingleton<IBgComponentRepository, BgComponentRepository>(x =>
            {
                var context = x.GetRequiredService<DataContext>();
                return new BgComponentRepository(context);
            });
            services.AddSingleton(x =>
            {
                var context = x.GetRequiredService<IUserSearchRepository>();
                return new ApiController(context);
            });
            services.AddSingleton<IExcelGenerator, ExcelGenerator>();
            // logging
            services.AddSingleton<LoggingDbServiceProvider>();
            services.AddSingleton<ILoggingDbCommand, LoggingDbExecutor>();
            services.AddSingleton<ILoggingDbContext>(s => {
                var command = s.GetRequiredService<ILoggingDbCommand>();
                return new LoggingDbContext(command, environ, "error");
            });
            // logging content repository
            services.AddSingleton<ILogContentRepository>(s => {
                var context = s.GetRequiredService<ILoggingDbContext>();
                return new LogContentRepository(context);
            });
            // logging configuration
            services.AddSingleton(p =>
            {
                var logprovider = p.GetRequiredService<LoggingDbServiceProvider>().Provider;
                return logprovider.GetRequiredService<ILogConfiguration>();
            });
            // logging service
            services.AddSingleton<ILoggingService>(p =>
            {
                var guid = Guid.NewGuid();
                var repo = p.GetRequiredService<ILogContentRepository>();
                var cfg = p.GetRequiredService<ILogConfiguration>();
                return new LoggingService(guid, repo, cfg);
            });
            // logging repository
            services.AddSingleton<ILoggingRepository>(p =>
            {
                var lg = p.GetRequiredService<ILoggingService>();
                return new LoggingRepository(lg);
            });

            services.AddHealthChecks()
                .AddCheck<DbConnectionHealthCheck>("DBConnection")
                .AddCheck<InfrastructureHealthCheck>("Infrastructure");

            services.AddSingleton(s =>
            {
                var logger = s.GetRequiredService<ILoggingRepository>();
                var search = s.GetRequiredService<ISearchQueueRepository>();
                var component = s.GetRequiredService<IBgComponentRepository>();
                var settings = s.GetRequiredService<IBackgroundServiceSettings>();
                var excel = s.GetRequiredService<IExcelGenerator>();
                return new SearchGenerationService(logger, search, component, settings, excel);
            });
            services.AddHostedService(s => s.GetRequiredService<SearchGenerationService>());
            services.AddSingleton(s => { return s; });
        }

        public static void Initialize(this WebApplication app)
        {
            var statuscodes = new Dictionary<HealthStatus, int>()
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            };
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            var health = new HealthCheckOptions { ResultStatusCodes = statuscodes };
            var details = new HealthCheckOptions
            {
                ResultStatusCodes = statuscodes,
                ResponseWriter = WriteHealthResponse.WriteResponse
            };
            app.MapHealthChecks("/health", health);
            app.MapHealthChecks("/health-details", details);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

        [ExcludeFromCodeCoverage]
        private static string GetConfigOrDefault(IConfiguration? configuration, string key, string backup)
        {
            try
            {
                if (configuration == null) return backup;
                return configuration.GetValue<string>(key) ?? backup;
            }
            catch (Exception)
            {
                return backup;
            }
        }
    }
}
