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
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace legallead.search.api.Utility
{
    public static class ServiceExtensions
    {
        public static void Initialize(this IServiceCollection services, IConfiguration? configuration = null)
        {
            string environ = GetConfigOrDefault(configuration, "DataEnvironment", "Local");
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
            services.AddScoped<IDapperCommand, DapperExecutor>();
            services.AddScoped(x =>
            {
                var command = x.GetRequiredService<IDapperCommand>();
                return new DataContext(command, null, environ, "app");
            });
            services.AddScoped<IUserSearchRepository, UserSearchRepository>(x =>
            {
                var context = x.GetRequiredService<DataContext>();
                return new UserSearchRepository(context);
            });
            services.AddScoped<ISearchQueueRepository, SearchQueueRepository>(x =>
            {
                var context = x.GetRequiredService<DataContext>();
                var service = x.GetRequiredService<IUserSearchRepository>();
                return new SearchQueueRepository(context, service);
            });
            services.AddScoped<IBgComponentRepository, BgComponentRepository>(x =>
            {
                var context = x.GetRequiredService<DataContext>();
                return new BgComponentRepository(context);
            });
            services.AddScoped(x =>
            {
                var context = x.GetRequiredService<IUserSearchRepository>();
                return new ApiController(context);
            });
            services.AddScoped<IExcelGenerator, ExcelGenerator>();
            // logging
            services.AddSingleton<LoggingDbServiceProvider>();
            services.AddScoped<ILoggingDbCommand, LoggingDbExecutor>();
            services.AddScoped<ILoggingDbContext>(s => {
                var command = s.GetRequiredService<ILoggingDbCommand>();
                return new LoggingDbContext(command, environ, "error");
            });
            // logging content repository
            services.AddScoped<ILogContentRepository>(s => {
                var context = s.GetRequiredService<ILoggingDbContext>();
                return new LogContentRepository(context);
            });
            // logging configuration
            services.AddScoped(p =>
            {
                var logprovider = p.GetRequiredService<LoggingDbServiceProvider>().Provider;
                return logprovider.GetRequiredService<ILogConfiguration>();
            });
            // logging service
            services.AddScoped<ILoggingService>(p =>
            {
                var guid = Guid.NewGuid();
                var repo = p.GetRequiredService<ILogContentRepository>();
                var cfg = p.GetRequiredService<ILogConfiguration>();
                return new LoggingService(guid, repo, cfg);
            });
            // logging repository
            services.AddScoped<ILoggingRepository>(p =>
            {
                var lg = p.GetRequiredService<ILoggingService>();
                return new LoggingRepository(lg);
            });

            services.AddHealthChecks()
                .AddCheck<DbConnectionHealthCheck>("DBConnection")
                .AddCheck<InfrastructureHealthCheck>("Infrastructure");

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
