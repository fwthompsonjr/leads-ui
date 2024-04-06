using Bogus;
using legallead.installer.Commands;
using legallead.installer.Interfaces;
using legallead.installer.Models;
using Moq;
using System.Reflection;

namespace legallead.installer.tests
{
    public class CommandHandlerTests
    {

        private static readonly Faker<ReleaseAssetModel> assetFaker =
            new Faker<ReleaseAssetModel>()
            .RuleFor(x => x.AssetId, y => y.Random.Int(100, 2000))
            .RuleFor(x => x.RepositoryId, y => y.Random.Long(1, 1000))
            .RuleFor(x => x.Name, y => Path.GetFileNameWithoutExtension(y.System.CommonFileName()))
            .RuleFor(x => x.DownloadUrl, y => y.Internet.Url())
            .RuleFor(x => x.Version, y => y.System.Semver());

        private static readonly Faker<ReleaseModel> modelFaker =
            new Faker<ReleaseModel>()
            .RuleFor(x => x.Name, y => y.System.Version().ToString())
            .RuleFor(x => x.Id, y => y.Random.Long(1, 1000))
            .RuleFor(x => x.RepositoryId, y => y.Random.Long(1, 1000))
            .RuleFor(x => x.PublishDate, y => y.Date.Recent())
            .FinishWith((x, m) =>
            {
                var nbr = x.Random.Int(1, 5);
                m.Assets = assetFaker.Generate(nbr);
            });

        [Fact]
        public void HandlerCanBeConstructed()
        {
            var mock = GetMock();
            var handler = GetCommandHandler(mock);
            Assert.NotNull(handler);
        }

        [Fact]
        public void HandlerCanExecuteRootCommand()
        {
            var exception = Record.Exception(() =>
            {
                var mock = GetMock();
                var handler = GetCommandHandler(mock);
                handler.WhoAmI();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void HandlerCanExecuteVersionCommand()
        {
            var exception = Record.Exception(() =>
            {
                var mock = GetMock();
                var handler = GetCommandHandler(mock);
                handler.Version();
            });
            Assert.Null(exception);
        }

        private static Mock<IGitReader> GetMock()
        {
            return new Mock<IGitReader>();
        }

        private static CommandHandler GetCommandHandler(Mock<IGitReader> mock, bool extractResult = true)
        {
            var locator = new Mock<ILeadAppInstaller>();
            var fileservice = new Mock<ILeadFileOperation>();
            var appFolder = Path.Combine(CurrentDir, "command_handler_test");
            var linkservice = new Mock<IShortcutCreator>();
            if (!Directory.Exists(appFolder)) { Directory.CreateDirectory(appFolder); }
            locator.SetupGet(s => s.SubFolder).Returns(appFolder);
            fileservice.Setup(s => s.Extract(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(extractResult);
            return new CommandHandler(mock.Object, locator.Object, fileservice.Object, linkservice.Object);
        }


        private static string? _currentDir;
        private static string CurrentDir
        {
            get
            {
                if (_currentDir != null) return _currentDir;
                _currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return _currentDir ?? string.Empty;
            }
        }
    }
}
