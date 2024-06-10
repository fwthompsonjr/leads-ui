using System.Diagnostics.CodeAnalysis;

namespace legallead.installer.Commands
{
    public partial class CommandHandler
    {

        [Command("upgrade", $"Upgrade installed application")]
        [ExcludeFromCodeCoverage(Justification = "Method interacts with 3rd party resources")]
        public void Upgrade(
            [Option("n", "application name", DefaultValue = "legallead.reader.service")] string name = "legallead.reader.service"
        )
        {
            var installed = GetLocals();
            var available = GetAvailables().GetAwaiter().GetResult();
            var installedVersion = _localsSvc.GetLatest(installed, name);
            var latestVersion = _availablesSvc.GetLatest(available, name);
            if (string.IsNullOrEmpty(latestVersion))
            {
                Console.WriteLine($"No available versions of {name} to install");
                return;
            }
            if (latestVersion.Equals(installedVersion))
            {
                Console.WriteLine($"Versions of {name} is latest, {latestVersion}");
                return;
            }
            Install("", name).GetAwaiter().GetResult();
        }

    }
}