using legallead.installer.Models;

namespace legallead.installer.Commands
{
    public partial class CommandHandler
    {
        [Command("uninstall", "Uninstalls application as defined by parameters")]
        public void Uninstall(
            [Option("n", "application name")] string name,
            [Option("v", "version number", DefaultValue = "")] string version = ""
        )
        {
            Console.WriteLine("Removing installed application :");
            Console.WriteLine($" name : {name}");
            if (!string.IsNullOrWhiteSpace(version)) { Console.WriteLine($" version : {version}"); }
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
            if (filtered.Count == 1 && string.IsNullOrWhiteSpace(version))
            {
                TryToDelete(filtered[0]);
                return;
            }
            var selection = filtered.Find(x => x.Version == version);
            if (selection == null)
            {
                Console.WriteLine($" - Unable to find application: {name}, version: {version}.");
                return;
            }
            TryToDelete(selection);
        }
        private void TryToDelete(LocalVersionModel model)
        {
            var name = model.Name;
            var version = model.Version;
            var failmsg = $" - Unable to remove application: {name}, version: {version}.";
            var success = $" - Successfully removed application: {name}, version: {version}.";
            try
            {
                var isFound = _fileService.DirectoryExists(model.FullPath);
                if (!isFound)
                {
                    Console.WriteLine(failmsg);
                    return; 
                }
                _fileService.DeleteDirectory(model.FullPath, true);
                if (_fileService.DirectoryExists(model.FullPath))
                    Console.WriteLine(failmsg);
                else
                    Console.WriteLine(success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(failmsg);
            }
        }
    }
}
