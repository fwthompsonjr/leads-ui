
// Builder returns IHost so you can configure application hosting option.
using legallead.installer.Classes;
using legallead.installer.Commands;
using legallead.installer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

var app = ConsoleApp.CreateBuilder(args)
    .ConfigureServices(s =>
    {
        s.AddSingleton<IGitReader, GitReader>();
        s.AddSingleton<ILeadFileOperation, LeadFileOperation>();
        s.AddSingleton<ILeadAppInstaller, LeadAppInstaller>();
    })
    .Build();
app.AddCommands<CommandHandler>();
app.Run();

[ExcludeFromCodeCoverage]
internal static partial class Program
{ }