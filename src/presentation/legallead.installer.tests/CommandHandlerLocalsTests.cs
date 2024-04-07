using Bogus;
using legallead.installer.Classes;
using legallead.installer.Commands;
using legallead.installer.Interfaces;
using legallead.installer.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace legallead.installer.tests
{
    public class CommandHandlerLocalsTests
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

        private static readonly Faker<DirectoryInfoModel> dirFaker =
            new Faker<DirectoryInfoModel>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.FullName, y => y.System.DirectoryPath())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        [Fact]
        public void SutCanBuildService()
        {
            var provider = GetProvider();
            var service = provider.GetRequiredService<CommandHandler>();
            Assert.NotNull(service);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void SutCanGetLocalsHappyPath(int recordCount)
        {
            var apps = appFaker.Generate(recordCount);
            if (recordCount == 10)
            {
                apps.ForEach(a => a.Versions = []);
            }
            var exception = Record.Exception(() =>
            {
                var provider = GetProvider();
                var service = provider.GetRequiredService<CommandHandler>();
                var installer = provider.GetRequiredService<Mock<ILeadAppInstaller>>();
                installer.Setup(s => s.GetInstalledApplications()).Returns(apps);
                service.Locals();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void InstallerGetAppsMissingSubfolderShouldBeEmpty()
        {
            var provider = GetProvider();
            var service = provider.GetRequiredService<LeadAppInstaller>();
            var manager = provider.GetRequiredService<Mock<ILeadFileOperation>>();
            manager.Setup(s => s.DirectoryExists(It.IsAny<string>())).Returns(false);
            var actual = service.GetInstalledApplications();
            Assert.NotNull(actual);
            Assert.Empty(actual);
        }

        [Fact]
        public void InstallerGetAppsMissingChildFoldersShouldBeEmpty()
        {
            var subFolder = GetSubFolder();
            var provider = GetProvider();
            var service = provider.GetRequiredService<LeadAppInstaller>();
            var manager = provider.GetRequiredService<Mock<ILeadFileOperation>>();
            DirectoryInfoModel[] subfolders = [];
            manager.Setup(s => s.DirectoryExists(It.IsAny<string>())).Returns(true);
            manager.Setup(s => s.GetDirectories(It.Is<string>(s => s.Equals(subFolder)))).Returns(subfolders);
            var actual = service.GetInstalledApplications();
            Assert.NotNull(actual);
            Assert.Empty(actual);
        }

        [Fact]
        public void InstallerGetAppsHappyPath()
        {
            var subFolder = GetSubFolder();
            var provider = GetProvider();
            var service = provider.GetRequiredService<LeadAppInstaller>();
            var manager = provider.GetRequiredService<Mock<ILeadFileOperation>>();
            var subfolders = dirFaker.Generate(3).ToArray();
            var childfolders = dirFaker.Generate(2).ToArray();
            manager.Setup(s => s.DirectoryExists(It.IsAny<string>())).Returns(true);
            manager.Setup(s => s.GetDirectories(It.Is<string>(s => s.Equals(subFolder)))).Returns(subfolders);
            manager.Setup(s => s.GetDirectories(It.Is<string>(s => !s.Equals(subFolder)))).Returns(childfolders);
            var actual = service.GetInstalledApplications();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }
        private static string GetSubFolder()
        {
            const string subDir = "_ll-testing";
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var applicationsDir = Path.Combine(homeDir, subDir);
            return applicationsDir;

        }

        private static ServiceProvider GetProvider()
        {
            var collection = new ServiceCollection();
            var mqReader = new Mock<IGitReader>();
            var mqInstaller = new Mock<ILeadAppInstaller>();
            var mqFileService = new Mock<ILeadFileOperation>();
            var mqLinkService = new Mock<IShortcutCreator>();

            // add mocks
            collection.AddSingleton(mqReader);
            collection.AddSingleton(mqInstaller);
            collection.AddSingleton(mqFileService);
            collection.AddSingleton(mqLinkService);
            // add implementations
            collection.AddSingleton(mqReader.Object);
            collection.AddSingleton(mqInstaller.Object);
            collection.AddSingleton(mqFileService.Object);
            collection.AddSingleton(mqLinkService.Object);
            collection.AddSingleton<CommandHandler>();
            collection.AddSingleton<LeadAppInstaller>(x =>
            {
                var manager = x.GetRequiredService<ILeadFileOperation>();
                return new MockAppInstaller(manager, GetSubFolder());
            });
            return collection.BuildServiceProvider();
        }

        private sealed class MockAppInstaller : LeadAppInstaller
        {
            private readonly string aliasLocation;
            public MockAppInstaller(ILeadFileOperation fileManager, string subfolder) : base(fileManager)
            {
                aliasLocation = subfolder;
            }
            public override string SubFolder => aliasLocation;
        }
    }
}
