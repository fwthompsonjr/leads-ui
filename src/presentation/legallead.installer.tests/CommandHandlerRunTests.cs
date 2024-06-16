using Bogus;
using legallead.installer.Classes;
using legallead.installer.Commands;
using legallead.installer.Interfaces;
using legallead.installer.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace legallead.installer.tests
{
    public class CommandHandlerRunTests
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
                var nbr = x.Random.Int(2, 5);
                m.Versions = versionFaker.Generate(nbr);
                m.Versions.ForEach(x => x.Name = m.Name);
            });

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
        [InlineData(7)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(12)]
        [InlineData(15)]
        [InlineData(17)]
        public void SutCanExecuteHappyPath(int recordCount)
        {
            var fkr = new Faker();
            var apps = appFaker.Generate(recordCount);
            var models = apps.SelectMany(x => x.Versions).ToList();
            var model = recordCount == 0 ? null : fkr.PickRandom(models);
            var name = recordCount == 7 ? string.Empty : model?.Name ?? string.Empty;
            var version = recordCount == 5 ? model?.Version ?? string.Empty : string.Empty;
            var fileName = recordCount != 9 ? model?.FullPath ?? string.Empty : string.Empty;
            var rsp = recordCount != 10;
            if (recordCount == 15 || recordCount == 0) name = "abcdefg";
            if (recordCount == 12) version = "123";
            if (recordCount == 17) version = string.Empty;
            if (recordCount == 5 || recordCount == 12 || recordCount == 17)
            {
                var dup = apps.Find(x => x.Name == name);
                if (dup != null) { apps.Add(dup); }
            }
            var exception = Record.Exception(() =>
            {
                var provider = GetProvider();
                var service = provider.GetRequiredService<CommandHandler>();
                var installer = provider.GetRequiredService<Mock<ILeadAppInstaller>>();
                var fileMgr = provider.GetRequiredService<Mock<ILeadFileOperation>>();
                installer.Setup(s => s.GetInstalledApplications()).Returns(apps);
                fileMgr.Setup(s => s.FindExecutable(It.IsAny<string>())).Returns(fileName);
                fileMgr.Setup(s => s.LaunchExecutable(It.IsAny<string>())).Returns(rsp);
                service.Execute(name, version);
            });
            Assert.Null(exception);
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

        private sealed class MockAppInstaller(ILeadFileOperation fileManager, string subfolder) : LeadAppInstaller(fileManager)
        {
            private readonly string aliasLocation = subfolder;

            public override string SubFolder => aliasLocation;
        }
    }
}