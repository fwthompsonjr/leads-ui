using legallead.installer.Classes;
using legallead.installer.Commands;
using legallead.installer.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.installer.tests
{
    public class CommandHandlerInstallTest
    {
        [Fact]
        public async Task SutCanInstall()
        {
            if (!System.Diagnostics.Debugger.IsAttached) { return; }
            var service = GetProvider().GetRequiredService<CommandHandler>();
            await service.Install("3.2.4", "legallead.desktop-windows");
            Assert.True(true);
        }

        private static ServiceProvider GetProvider()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IGitReader, GitReader>();
            collection.AddSingleton<ILeadFileOperation, LeadFileOperation>();
            collection.AddSingleton<ILeadAppInstaller, LeadAppInstaller>();
            collection.AddSingleton<IShortcutCreator, ShortcutCreator>();
            collection.AddSingleton<CommandHandler>();
            return collection.BuildServiceProvider();
        }
    }
}
