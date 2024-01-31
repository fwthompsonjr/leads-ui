using component;
using legallead.reader.component.utility;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Initialize();
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService(s => s.GetRequiredService<SearchGenerationService>());

var host = builder.Build();
host.Run();
