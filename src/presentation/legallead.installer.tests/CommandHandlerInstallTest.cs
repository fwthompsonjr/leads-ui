
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
            const string appname = "legallead.reader.service";
            var exception = await Record.ExceptionAsync(async () =>
            {
                var service = GetProvider().GetRequiredService<CommandHandler>();
                await service.Install("", appname);
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
        static void CopyDirectory(string source, string target, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(source);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(target);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(target, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(target, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}