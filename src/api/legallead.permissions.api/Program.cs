using legallead.permissions.api;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Health;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var environmentName = builder.Environment.EnvironmentName;
var isDevelopment = environmentName.Equals("development", StringComparison.OrdinalIgnoreCase);
var config = isDevelopment switch
{
    true => new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{environmentName}.json", true)
        .AddEnvironmentVariables()
        .Build(),
    _ => new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build()
};
var services = builder.Services;
services.RegisterDataServices(config);
services.RegisterAuthentication(config);
services.RegisterEmailServices();
services.AddSingleton<BeginSearchRequested>();
services.AddSingleton<RegistrationCompleted>();
services.AddSingleton<RegistrationCompleted>();
services.AddSingleton<ProfileChanged>();
services.AddSingleton<PermissionChangeRequested>();
services.AddSingleton<PasswordChanged>();
services.AddControllers();
services.AddScoped<DownloadController>();
services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "legallead.permissions.api", Version = "v1" });
});

services.AddSingleton<IInternalServiceProvider>(new InternalServiceProvider(services));
services.RegisterHealthChecks();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "legallead.permissions.api v1"));

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
var statuscodes = new Dictionary<HealthStatus, int>()
{
    [HealthStatus.Healthy] = StatusCodes.Status200OK,
    [HealthStatus.Degraded] = StatusCodes.Status200OK,
    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
};
var health = new HealthCheckOptions { ResultStatusCodes = statuscodes };
var details = new HealthCheckOptions
{
    ResultStatusCodes = statuscodes,
    ResponseWriter = WriteHealthResponse.WriteResponseAsync
};
app.MapHealthChecks("/health", health);
app.MapHealthChecks("/health-details", details);
#pragma warning disable ASP0014 // Suggest using top level route registrations
// this allows backward compatibility to 6.0 projects
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

// Resolve the StartupTasks from the ServiceProvider
var startupTasks = app.Services.GetServices<IStartupTask>();

// Run the StartupTasks
foreach (var startupTask in startupTasks)
{
    _ = Task.Factory.StartNew(async () => await startupTask.ExecuteAsync().ConfigureAwait(false)).ConfigureAwait(false);
}
await app.RunAsync();

[ExcludeFromCodeCoverage]
internal static partial class Program
{ }