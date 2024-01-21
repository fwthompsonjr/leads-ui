using legallead.search.api.Utility;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.Initialize();

var app = builder.Build();
app.Initialize();
app.Run();

[ExcludeFromCodeCoverage]
internal static partial class Program
{ }