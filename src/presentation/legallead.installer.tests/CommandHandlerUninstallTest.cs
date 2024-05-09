using legallead.installer.Classes;
using legallead.installer.Commands;
using legallead.installer.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.installer.tests
{
    public class CommandHandlerUninstallTest
    {
        private static readonly object _lock = new();
        [Fact]
        public void SutCanUnInstall()
        {
            if (!System.Diagnostics.Debugger.IsAttached) { return; }
            const string appname = "legallead.desktop-windows";
            var provider = GetProvider();
            var fileSvc = provider.GetService<ILeadAppInstaller>();
            var models = fileSvc?.GetInstalledApplications() ?? [];
            if (models.Count == 0) return;
            var versions = models.SelectMany(x => x.Versions).Where(x => x.Name == appname).ToList();
            if (versions.Count == 0) return;

            lock (_lock)
            {
                var sourceDir = versions[0].FullPath;
                var destinationDir = string.Concat(sourceDir, "-backup");
                if (Directory.Exists(destinationDir)) { Directory.Delete(destinationDir, true); }
                CopyDirectory(sourceDir, destinationDir, true);
                try
                {
                    var exception = Record.Exception(() =>
                    {
                        var service = GetProvider().GetRequiredService<CommandHandler>();
                        service.Uninstall(appname);
                    });
                    Assert.Null(exception);
                }
                finally
                {
                    if (Directory.Exists(destinationDir) && !Directory.Exists(sourceDir))
                    {
                        CopyDirectory(destinationDir, sourceDir, true);
                    }
                    if (Directory.Exists(sourceDir) && Directory.Exists(destinationDir))
                    {
                        Directory.Delete(destinationDir, true);
                    }
                }
            }
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