using component;
using legallead.reader.component.utility;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args).UseSystemd();
builder.ConfigureServices(services =>
{
    services.Initialize();
    services.AddWindowsService();
    services.AddHostedService<Worker>();
    services.AddHostedService(s => s.GetRequiredService<SearchGenerationService>());
});

var host = builder.Build();
host.Run();
