using legallead.installer.Classes;
using legallead.installer.Interfaces;
using System.ComponentModel;
using System.Reflection;

namespace legallead.installer.Commands
{
    public class CommandHandler : ConsoleAppBase
    {
        private const string appName = "legallead.installer";
        private const string nodata = "0.0.0";
        private readonly IGitReader _reader;
        private readonly ILeadAppInstaller _applocator;
        private readonly ILeadFileOperation _fileService;
        private readonly string versionNumber;
        private readonly List<string> _packageNames;
        public CommandHandler(IGitReader reader, 
            ILeadAppInstaller applocator,
            ILeadFileOperation fileService)
        {
            _reader = reader;
            _packageNames = GitClientProvider.PackageNames;
            var version = Assembly.GetExecutingAssembly()
                ?.GetCustomAttribute<AssemblyFileVersionAttribute>()
                ?.Version;
            versionNumber = version ?? nodata;
            _applocator = applocator;
            _fileService = fileService;
        }

        [RootCommand]
        public void WhoAmI()
        {
            var details = new[]
            {
                $"Application: {appName}",
                $"Version: {versionNumber}"
            };
            var content = string.Join(Environment.NewLine, details);
            Console.WriteLine(content);
        }

        [Command("version", $"Display version information for {appName}")]
        public void Version()
        {
            var version = versionNumber.Equals(nodata)
                ? $"{versionNumber} : No details found"
                : $"{versionNumber} - {appName}";
            Console.WriteLine(version);
        }

        [Command("list", "Display release details for legallead applications")]
        public async Task List()
        {
            Console.WriteLine("Listing available versions for legallead applications.");
            var models = await _reader.GetReleases();
            if (models == null || models.Count == 0)
            {
                Console.WriteLine(" - No items found.");
                return;
            }
            foreach (var model in models)
            {
                Console.WriteLine(" - {0}: {1:D}", model.Name, model.PublishDate);
                if (model.Assets.Count == 0) continue;
                var details = model.Assets.Select(x =>
                {
                    return $"     -- {x.AssetId}: {x.Name} {x.Version}";
                });
                Console.WriteLine(string.Join(Environment.NewLine, details));
            }
        }

        [Command("locals", "Display all local installed applications")]
        public void Locals()
        {
            Console.WriteLine("Listing installed versions for legallead applications.");
            var models = _applocator.GetInstalledApplications();
            if (models.Count == 0)
            {
                Console.WriteLine(" - No items found.");
                return;
            }
            foreach (var model in models)
            {
                Console.WriteLine(" - {0}: {1:D}", model.Name, model.PublishDate);
                if (model.Versions.Count == 0) continue;
                var details = model.Versions.Select(x =>
                {
                    return $"     -- {x.Version} {x.PublishDate:D}";
                });
                Console.WriteLine(string.Join(Environment.NewLine, details));
            }
        }

        [Command("install", "Install legallead application")]
        public async Task Install(
            [Option("v", "version number", DefaultValue = "")] string version = "",
            [Option("n", "application name", DefaultValue = "")] string app = "",
            [Option("i", "application id", DefaultValue = "")] string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                if (!int.TryParse(id, out var assetId))
                {
                    Console.WriteLine("Assert Id is invalid.");
                    return;
                }
                var assets = await _reader.GetAssets();
                var find = assets?.Find(a => a.AssetId == assetId);
                if (find == null)
                {
                    Console.WriteLine("Assert Id is invalid.");
                    Console.WriteLine("Execute list command to display available versions");
                    return;
                }
                version = find.Version;
                app = find.Name;
            }
            if (string.IsNullOrWhiteSpace(version))
            {
                Console.WriteLine("Version is missing.");
                Console.WriteLine("Execute list command to display available versions");
                return;
            }
            if (string.IsNullOrWhiteSpace(app))
            {
                Console.WriteLine("Application name is missing.");
                Console.WriteLine("Execute list command to display available versions");
                return;
            }
            if (!_reader.VerifyPackageName(app))
            {
                Console.WriteLine("Application name is invalid.");
                Console.WriteLine("Available names are: ");
                Console.WriteLine(string.Join(", ", _packageNames));
                return;
            }
            var models = await _reader.GetReleases();
            if (models == null || models.Count == 0)
            {
                Console.WriteLine("Unable to locate application: {0} version: {1}.", app, version);
                return;
            }
            var item = _reader.FindAsset(models, version, app);
            if (item == null)
            {
                Console.WriteLine("Unable to locate application: {0} version: {1}.", app, version);
                return;
            }
            Console.WriteLine("Downloading application: {0} version: {1}.", app, version);
            var data = await _reader.GetAsset(item);
            if (data is not byte[] contents)
            {
                Console.WriteLine("Failed to download application: {0} version: {1}.", app, version);
                return;
            }
            var installPath = Path.Combine(_applocator.SubFolder, app);
            _fileService.CreateDirectory(installPath);
            installPath = Path.Combine(installPath, version);
            _fileService.CreateDirectory(installPath);
            var completion = _fileService.Extract(installPath, contents);
            if (!completion) return;
            Console.WriteLine("Completed download application: {0} version: {1}.", app, version);
            Console.WriteLine(" - {0}", installPath);
            if (!_reader.AllowShortcuts) return;
            Console.WriteLine("Creating application shortcut: {0} version: {1}.", app, version);
            ShortcutBuilder.CreateShortCut(item, installPath);

            Console.WriteLine("Creating desktop shortcut: {0} version: {1}.", app, version);
            ShortcutBuilder.CreateShortCut(item, installPath, true);
        }
    }
}
