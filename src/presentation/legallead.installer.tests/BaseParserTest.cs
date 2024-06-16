using legallead.installer.Classes;
using legallead.installer.Commands;
using legallead.installer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace legallead.installer.tests
{
    using TestResources = Properties.Resources;
    public abstract class BaseParserTest
    {
        protected static string[] AppNames => appNames;
        protected static string AvailableResult => availableResult;
        protected static string InstalledResult => installedResult;
        protected static ServiceProvider GetProvider()
        {
            var collection = new ServiceCollection();
            var mqReader = new Mock<IGitReader>();
            var mqInstaller = new Mock<ILeadAppInstaller>();
            var mqFileService = new Mock<ILeadFileOperation>();
            var mqLinkService = new Mock<IShortcutCreator>();
            var mqLocalService = new Mock<ILocalsParser>();
            var mqAvailableService = new Mock<IAvailablesParser>();

            // add mocks
            collection.AddSingleton(mqReader);
            collection.AddSingleton(mqInstaller);
            collection.AddSingleton(mqFileService);
            collection.AddSingleton(mqLinkService);
            collection.AddSingleton(mqLocalService);
            collection.AddSingleton(mqAvailableService);
            // add implementations
            collection.AddSingleton(mqReader.Object);
            collection.AddSingleton(mqInstaller.Object);
            collection.AddSingleton(mqFileService.Object);
            collection.AddSingleton(mqLinkService.Object);
            collection.AddSingleton(mqLocalService.Object);
            collection.AddSingleton(mqAvailableService.Object);
            collection.AddSingleton<CommandHandler>();
            collection.AddSingleton<LeadAppInstaller>(x =>
            {
                var manager = x.GetRequiredService<ILeadFileOperation>();
                return new MockAppInstaller(manager, GetSubFolder());
            });
            return collection.BuildServiceProvider();
        }


        protected sealed class MockAppInstaller(ILeadFileOperation fileManager, string subfolder) : LeadAppInstaller(fileManager)
        {
            private readonly string aliasLocation = subfolder;

            public override string SubFolder => aliasLocation;
        }


        private static string GetSubFolder()
        {
            if (!string.IsNullOrEmpty(testingFolder)) return testingFolder;
            const string subDir = "_ll-testing";
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            testingFolder = Path.Combine(homeDir, subDir);
            return testingFolder;
        }

        private static string? testingFolder;
        private static readonly string[] appNames = ["legallead.desktop-windows", "legallead.reader.service"];
        private static readonly string availableResult = TestResources.sample_available;
        private static readonly string installedResult = TestResources.sample_installed;
    }
}
