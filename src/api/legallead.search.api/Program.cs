using legallead.search.api;
using legallead.search.api.Services;
using legallead.search.api.Utility;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddSingleton(builder.Environment);
services.Initialize();
services.AddHostedService<SetupGeckoTask>();

var app = builder.Build();
app.Initialize();

app.Run();

[ExcludeFromCodeCoverage]
internal static partial class Program
{ }