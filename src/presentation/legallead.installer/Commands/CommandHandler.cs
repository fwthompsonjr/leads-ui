using legallead.installer.Classes;
using legallead.installer.Interfaces;
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
            var models = await _reader.GetReleases();
            if (models == null || models.Count == 0)
            {
                Console.WriteLine("No items found.");
                return;
            }
            foreach (var model in models)
            {
                Console.WriteLine("{0}: {1:f}", model.Name, model.PublishDate);
            }
        }

        [Command("install", "Install legallead application")]
        public async Task Install(
            [Option("v", "version number")] string version,
            [Option("n", "application name")] string app)
        {
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
        }
    }
}
