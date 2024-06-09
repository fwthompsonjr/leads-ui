using legallead.installer.Classes;
using legallead.installer.Commands;
using legallead.installer.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.installer.tests
{
    public class VersionCheckTest
    {

        [Fact]
        public void SutCanGetLocal()
        {
            if (!System.Diagnostics.Debugger.IsAttached) { return; }
            const string appname = "legallead.reader.service";
            var provider = GetProvider();
            var exception = Record.Exception(() =>
            {
                var service = provider.GetRequiredService<CommandHandler>();
                var items = service.GetAvailables().GetAwaiter().GetResult();
                Assert.NotEmpty(items);
                if (!items.Contains(appname, StringComparison.OrdinalIgnoreCase)) return;
                var parser = provider.GetRequiredService<IAvailablesParser>();
                var number = parser.GetLatest(items, appname);
                Assert.NotEmpty(number);
            });
            Assert.Null(exception);
        }

        private static ServiceProvider GetProvider()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IGitReader, GitReader>();
            collection.AddSingleton<ILeadFileOperation, LeadFileOperation>();
            collection.AddSingleton<ILeadAppInstaller, LeadAppInstaller>();
            collection.AddSingleton<IShortcutCreator, ShortcutCreator>();
            collection.AddSingleton<ILocalsParser, LocalsParser>();
            collection.AddSingleton<IAvailablesParser, AvailablesParser>();
            collection.AddSingleton<CommandHandler>();
            return collection.BuildServiceProvider();
        }
    }
}
