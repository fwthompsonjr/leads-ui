
// Builder returns IHost so you can configure application hosting option.
using legallead.installer.Classes;
using legallead.installer.Commands;
using legallead.installer.Interfaces;
using Microsoft.Extensions.DependencyInjection;

var app = ConsoleApp.CreateBuilder(args)
    .ConfigureServices(s =>
    {
        s.AddSingleton<IGitReader, GitReader>();
    })
    .Build();
app.AddCommands<CommandHandler>();
app.Run();