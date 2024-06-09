using Bogus;
using legallead.installer.Commands;
using legallead.installer.Interfaces;
using legallead.installer.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace legallead.installer.tests
{
    public class CommandUninstallUnitTests
    {
        private static readonly Faker<LocalVersionModel> versionFaker =
            new Faker<LocalVersionModel>()
            .RuleFor(x => x.PublishDate, y => y.Date.Recent())
            .RuleFor(x => x.FullPath, y => y.System.DirectoryPath())
            .RuleFor(x => x.Version, y => y.System.Semver());

        private static readonly Faker<LocalAppModel> appFaker =
            new Faker<LocalAppModel>()
            .RuleFor(x => x.Name, y => y.System.Version().ToString())
            .RuleFor(x => x.PublishDate, y => y.Date.Recent())
            .FinishWith((x, m) =>
            {
                var nbr = x.Random.Int(1, 5);
                m.Versions = versionFaker.Generate(nbr);
                m.Versions.ForEach(x => x.Name = m.Name);
            });

        [Fact]
        public void SutCanUnInstallWithoutVersion()
        {
            var provider = GetProvider();
            var appSvc = provider.GetRequiredService<Mock<ILeadAppInstaller>>();
            var fileSvc = provider.GetRequiredService<Mock<ILeadFileOperation>>();
            var apps = appFaker.Generate(10);
            appSvc.Setup(s => s.GetInstalledApplications()).Returns(apps);
            fileSvc.SetupSequence(s => s.DirectoryExists(It.IsAny<string>()))
                .Returns(true)
                .Returns(false);
            var selected = apps[2];
            selected.Versions = [selected.Versions[0]];
            string name = selected.Versions[0].Name;
            var exception = Record.Exception(() =>
            {
                var service = provider.GetRequiredService<CommandHandler>();
                service.Uninstall(name);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("", "1.2.3")]
        [InlineData("non-app", "")]
        public void SutCanUnInstallVersion(string name, string version)
        {
            var provider = GetProvider();
            var appSvc = provider.GetRequiredService<Mock<ILeadAppInstaller>>();
            var fileSvc = provider.GetRequiredService<Mock<ILeadFileOperation>>();
            var apps = appFaker.Generate(10);
            appSvc.Setup(s => s.GetInstalledApplications()).Returns(apps);
            fileSvc.SetupSequence(s => s.DirectoryExists(It.IsAny<string>()))
                .Returns(true)
                .Returns(false);
            var exception = Record.Exception(() =>
            {
                var service = provider.GetRequiredService<CommandHandler>();
                service.Uninstall(name, version);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true, false, 0)]
        [InlineData(false, false, 1)]
        [InlineData(true, true, 2)]
        [InlineData(true, false, 3)]
        [InlineData(true, false, 4)]
        [InlineData(true, false, 5)]
        [InlineData(true, false, 6)]
        [InlineData(true, false, 7)]
        public void SutCanUnInstall(bool soureExists, bool sourceDeleted, int testId)
        {
            var faker = new Faker();
            var provider = GetProvider();
            var appSvc = provider.GetRequiredService<Mock<ILeadAppInstaller>>();
            var fileSvc = provider.GetRequiredService<Mock<ILeadFileOperation>>();
            var apps = testId == 6 ? appFaker.Generate(1) : appFaker.Generate(10);
            if (testId == 7) apps = [];
            var versions = testId == 7 ? [] : apps.SelectMany(s => s.Versions);
            var selection = testId == 7 ? null : faker.PickRandom(versions);
            var name = selection?.Name ?? "1213";
            var version = testId != 3 ? selection?.Version ?? "" : string.Empty;
            appSvc.Setup(s => s.GetInstalledApplications()).Returns(apps);
            fileSvc.SetupSequence(s => s.DirectoryExists(It.IsAny<string>()))
                .Returns(soureExists)
                .Returns(sourceDeleted);
            if (testId == 4)
            {
                var fault = faker.System.Exception();
                fileSvc.Setup(s => s.DeleteDirectory(It.IsAny<string>(), It.IsAny<bool>())).Throws(fault);
            }
            if (testId == 5) version = "no-match";
            var exception = Record.Exception(() =>
            {
                var service = provider.GetRequiredService<CommandHandler>();
                service.Uninstall(name, version);
            });
            Assert.Null(exception);
        }

        private static ServiceProvider GetProvider()
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
            return collection.BuildServiceProvider();
        }
    }
}