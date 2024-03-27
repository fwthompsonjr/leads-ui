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
        public CommandHandler(IGitReader reader)
        {
            _reader = reader;
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
        public void ShowVersion()
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
    }
}
