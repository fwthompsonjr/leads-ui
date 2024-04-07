namespace legallead.installer.Commands
{
    public partial class CommandHandler
    {

        [Command("version", $"Display version information for {appName}")]
        public void Version()
        {
            var version = versionNumber.Equals(nodata)
                ? $"{versionNumber} : No details found"
                : $"{versionNumber} - {appName}";
            Console.WriteLine(version);
        }

    }
}
