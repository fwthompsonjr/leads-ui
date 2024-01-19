using legallead.search.api.Utility;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.Initialize();

var app = builder.Build();
app.Initialize();
app.Run();
