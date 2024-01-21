using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.logging;
using legallead.logging.interfaces;
using legallead.search.api.Controllers;
using legallead.search.api.Health;
using legallead.search.api.Models;
using legallead.search.api.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace legallead.search.api.Utility
{
    public static class ServiceExtensions
    {
        public static void Initialize(this IServiceCollection services, IConfiguration? configuration = null)
        {
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
            services.AddScoped(x =>
            {
                var command = new DapperExecutor();
                return new DataContext(command);
            });
            services.AddScoped<IUserSearchRepository, UserSearchRepository>(x =>
            {
                var context = x.GetRequiredService<DataContext>();
                return new UserSearchRepository(context);
            });
            services.AddScoped<ISearchQueueRepository, SearchQueueRepository>(x =>
            {
                var context = x.GetRequiredService<DataContext>();
                return new SearchQueueRepository(context);
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
            services.AddScoped(p =>
            {
                var logprovider = p.GetRequiredService<LoggingDbServiceProvider>().Provider;
                return logprovider.GetRequiredService<ILogConfiguration>();
            });
            // logging
            services.AddScoped(p =>
            {
                var logprovider = p.GetRequiredService<LoggingDbServiceProvider>().Provider;
                return logprovider.GetRequiredService<ILoggingService>();
            });
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
    }
}
