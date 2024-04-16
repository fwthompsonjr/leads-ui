namespace legallead.installer.Commands
{
    public partial class CommandHandler
    {
        [Command("install", "Install legallead application")]
        public async Task Install(
            [Option("v", "version number", DefaultValue = "")] string version = "",
            [Option("n", "application name", DefaultValue = "")] string name = "",
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
                name = find.Name;
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Application name is missing.");
                Console.WriteLine("Execute list command to display available versions");
                return;
            }
            if (!_reader.VerifyPackageName(name))
            {
                Console.WriteLine("Application name is invalid.");
                Console.WriteLine("Available names are: ");
                Console.WriteLine(string.Join(", ", _packageNames));
                return;
            }
            var models = await _reader.GetReleases();
            if (models == null || models.Count == 0)
            {
                Console.WriteLine("Unable to locate application: {0} version: {1}.", name, version);
                return;
            }
            var item = _reader.FindAsset(models, version, name);
            if (item == null)
            {
                Console.WriteLine("Unable to locate application: {0} version: {1}.", name, version);
                return;
            }
            name = item.Name;
            version = item.Version;
            Console.WriteLine("Downloading application: {0} version: {1}.", name, version);
            var data = await _reader.GetAsset(item);
            if (data is not byte[] contents)
            {
                Console.WriteLine("Failed to download application: {0} version: {1}.", name, version);
                return;
            }
            var installPath = Path.Combine(_applocator.SubFolder, name);
            _fileService.CreateDirectory(installPath);
            installPath = Path.Combine(installPath, version);
            _fileService.CreateDirectory(installPath);
            var completion = _fileService.Extract(installPath, contents);
            if (!completion) return;
            Console.WriteLine("Completed download application: {0} version: {1}.", name, version);
            Console.WriteLine(" - {0}", installPath);
            if (!_reader.AllowShortcuts) return;
            _linkService.Install(_linkService, item, installPath, name, version);
        }

    }
}
