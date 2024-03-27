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
        private readonly string versionNumber;
        private readonly List<string> _packageNames;
        public CommandHandler(IGitReader reader)
        {
            _reader = reader;
            _packageNames = GitClientProvider.PackageNames;
            var version = Assembly.GetExecutingAssembly()
                ?.GetCustomAttribute<AssemblyFileVersionAttribute>()
                ?.Version;
            versionNumber = version ?? nodata;
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
            if (!_packageNames.Contains(app, StringComparer.OrdinalIgnoreCase))
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
            var list = models.SelectMany(x => x.Assets).ToList();
            var item = list.Find(x => x.Name.Equals(app) && x.Version.Equals(version));

            if (item == null)
            {
                Console.WriteLine("Unable to locate application: {0} version: {1}.", app, version);
                return;
            }
            Console.WriteLine("Downloading application: {0} version: {1}.", app, version);
            var data = await _reader.GetAsset(item);
            if (data == null)
            {
                Console.WriteLine("Failed to download application: {0} version: {1}.", app, version);
            }
        }
    }
}
