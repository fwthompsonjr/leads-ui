using legallead.installer.Classes;
using legallead.installer.Interfaces;
using System.Reflection;

namespace legallead.installer.Commands
{
    public partial class CommandHandler : ConsoleAppBase
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
    }
}
