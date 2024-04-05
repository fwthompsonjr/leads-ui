using legallead.installer.Models;

namespace legallead.installer.Commands
{
    public partial class CommandHandler
    {
        [Command("run", "Run application as defined by parameters")]
        public void Execute(
            [Option("n", "application name")] string name,
            [Option("v", "version number", DefaultValue = "")] string version = ""
        )
        {
            Console.WriteLine("Opening installed application :");
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Application name is missing.");
                Console.WriteLine("Execute locals command to display available versions");
                return;
            }
            var models = _applocator.GetInstalledApplications();
            if (models.Count == 0)
            {
                Console.WriteLine(" - No items found.");
                return;
            }
            var versions = models.SelectMany(x => x.Versions).ToList();
            var filtered = versions.FindAll(x => x.Name == name);
            filtered.Sort((b, a) => a.Version.CompareTo(b.Version));
            if (filtered.Count == 0)
            {
                Console.WriteLine($" - Unable to find application: {name}.");
                return;
            }
            if (filtered.Count == 1 || string.IsNullOrWhiteSpace(version))
            {
                TryToExecute(filtered[0]);
                return;
            }
            var selection = filtered.Find(x => x.Version == version);
            if (selection == null)
            {
                Console.WriteLine($" - Unable to find application: {name}, version: {version}.");
                return;
            }
            TryToExecute(filtered[0]);
        }

        private void TryToExecute(LocalVersionModel model)
        {
            var name = model.Name;
            var version = model.Version;
            var location = _fileService.FindExecutable(model.FullPath);
            if (string.IsNullOrWhiteSpace(location))
            {
                Console.WriteLine($" - Unable to find application: {name}, version: {version}.");
                return;
            }
            if (_fileService.LaunchExecutable(location)) return;
            Console.WriteLine($" - Unable to open application: {name}, version: {version}.");
        }
    }
}
