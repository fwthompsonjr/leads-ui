using legallead.permissions.api;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var environmentName = builder.Environment.EnvironmentName;
var config =
    new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{environmentName}.json", true)
        .AddEnvironmentVariables()
        .Build();
var services = builder.Services;
services.RegisterDataServices();
services.RegisterAuthentication(config);
services.AddControllers();
services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
services.AddSingleton<IConfiguration>(config);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "legallead.permissions.api", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "legallead.permissions.api v1"));
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
// Resolve the StartupTasks from the ServiceProvider
var startupTasks = app.Services.GetServices<IStartupTask>();

// Run the StartupTasks
foreach (var startupTask in startupTasks)
{
    Task.Factory.StartNew(async () => await startupTask.Execute());
}
app.Run();

[ExcludeFromCodeCoverage]
internal static partial class Program
{ }