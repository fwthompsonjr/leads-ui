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

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(5)]
        public async Task HandlerCanExecuteListCommand(int? recordCount)
        {
            var records = (recordCount) switch
            {

                null => default,
                0 => [],
                _ => modelFaker.Generate(recordCount.GetValueOrDefault())
            };

            var exception = await Record.ExceptionAsync(async () =>
            {
                var mock = GetMock();
                mock.Setup(m => m.GetReleases()).ReturnsAsync(records);
                var handler = GetCommandHandler(mock);
                await handler.List();
            });
            Assert.Null(exception);
        }


        [Theory]
        [InlineData("", "")]
        [InlineData("   ", "")]
        [InlineData("temp", "")]
        [InlineData("temp", "  ")]
        [InlineData("temp", "temp")]
        [InlineData("temp", "missing")]
        [InlineData("temp", "empty")]
        [InlineData("temp", "none")]
        [InlineData("temp", "no-assets")]
        [InlineData("temp", "no-data")]
        public async Task HandlerCanExecuteInstallCommand(string versionName, string appName)
        {
            List<string> custom = ["temp", "missing"];
            var records = modelFaker.Generate(15);
            var recordset = appName.Equals("empty") ? [] : records;
            recordset = appName.Equals("none") ? null : recordset;
            var list = records.SelectMany(x => x.Assets).ToList();
            var indx = new Faker().PickRandom(list);
            var asset = appName.Equals("temp") | appName.Equals("no-data") ? indx : null;
            var assetData = appName.Equals("temp") ? new Faker().System.Random.Bytes(1000) : null;
            var exception = await Record.ExceptionAsync(async () =>
            {
                var mock = GetMock();
                mock.Setup(m => m.GetReleases()).ReturnsAsync(recordset);
                mock.Setup(m => m.VerifyPackageName(It.Is<string>(s => !custom.Contains(s)))).Returns(true);
                mock.Setup(m => m.VerifyPackageName(It.Is<string>(s => s == "temp"))).Returns(true);
                mock.Setup(m => m.VerifyPackageName(It.Is<string>(s => s == "missing"))).Returns(false);
                mock.Setup(m => m.FindAsset(
                    It.IsAny<List<ReleaseModel>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())).Returns(asset);
                mock.Setup(m => m.GetAsset(It.IsAny<ReleaseAssetModel>())).ReturnsAsync(assetData);
                var handler = GetCommandHandler(mock);
                await handler.Install(versionName, appName, string.Empty);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("temp")]
        [InlineData("alpha")]
        [InlineData("5000")]
        public async Task HandlerCanExecuteInstallByIndex(string? appId)
        {
            List<string> custom = ["temp", "missing"];
            const string appName = "temp";
            var records = modelFaker.Generate(15);
            var recordset = appName.Equals("empty") ? [] : records;
            recordset = appName.Equals("none") ? null : recordset;
            var list = records.SelectMany(x => x.Assets).ToList();
            var indx = new Faker().PickRandom(list);
            if (!string.IsNullOrEmpty(appId) && appId.Equals("temp")) appId = indx.AssetId.ToString();

            var asset = appName.Equals("temp") | appName.Equals("no-data") ? indx : null;
            var assetData = appName.Equals("temp") ? new Faker().System.Random.Bytes(1000) : null;
            var exception = await Record.ExceptionAsync(async () =>
            {
                var mock = GetMock();
                mock.SetupGet(x => x.AllowShortcuts).Returns(true);
                mock.Setup(m => m.GetReleases()).ReturnsAsync(recordset);
                mock.Setup(m => m.GetAssets()).ReturnsAsync(list);
                mock.Setup(m => m.VerifyPackageName(It.Is<string>(s => !custom.Contains(s)))).Returns(true);
                mock.Setup(m => m.VerifyPackageName(It.Is<string>(s => s == "temp"))).Returns(true);
                mock.Setup(m => m.VerifyPackageName(It.Is<string>(s => s == "missing"))).Returns(false);
                mock.Setup(m => m.FindAsset(
                    It.IsAny<List<ReleaseModel>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())).Returns(asset);
                mock.Setup(m => m.GetAsset(It.IsAny<ReleaseAssetModel>())).ReturnsAsync(assetData);
                var handler = GetCommandHandler(mock, false);
                await handler.Install("", "", appId ?? "");
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
