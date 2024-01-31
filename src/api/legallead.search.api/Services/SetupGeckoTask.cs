using legallead.records.search.DriverFactory;

namespace legallead.search.api.Services
{
    public class SetupGeckoTask : IHostedService
    {
        private bool IsDataReady = false;
        private readonly IWebHostEnvironment web;
        private Timer? _timer = null;

        public SetupGeckoTask(IWebHostEnvironment environment)
        {
            web = environment;
        }
        public bool IsHealthy => IsDataReady;

        protected void CopyToApp(string path)
        {
            var webDirectory = web.ContentRootPath;
            if (!Directory.Exists(webDirectory)) return;
            var sourceFile = Path.Combine(webDirectory, path);
            var directories = new List<string>
            {
                Path.GetDirectoryName(sourceFile) ?? string.Empty,
            };
            if (!File.Exists(sourceFile)) return;
            var appDir = FindParent(webDirectory, "app");
            if (appDir == null) return;
            var targets = new List<string>() { "debug", "release" };
            
            targets.ForEach(target =>
            {
                directories.Add(CopyFile(path, sourceFile, appDir, target) ?? string.Empty);
            });
            var scopes = new[] { EnvironmentVariableTarget.Machine, EnvironmentVariableTarget.User }.ToList();
            scopes.ForEach(scope => TryAppendEnvironment(scope, directories));            
        }

        private static string? CopyFile(string path, string sourceFile, string appDir, string subDir)
        {
            const string app = "/app/";
            const string appapp = "/app/app/";
            var children = new[] { "bin", subDir, "net6.0" }.ToList();
            var destination = string.Empty;
            children.ForEach(child =>
            {
                if (string.IsNullOrEmpty(destination)) { destination = child; }
                else { destination = $"{destination}/{child}"; }
                var item = Path.Combine(appDir, destination).Replace(appapp, app);
                if (!Directory.Exists(item)) Directory.CreateDirectory(item);
            });
            var subdir = string.Join("/", children);
            var targetDir = Path.Combine(appDir, subdir).Replace(appapp, app);
            var targetFile = Path.Combine(targetDir, path).Replace(appapp, app);
            if (File.Exists(targetFile)) return null;
            File.Copy(sourceFile, targetFile);
            return targetDir;
        }

        private static string? FindParent(string webDirectory, string parentName)
        {
            var di = new DirectoryInfo(webDirectory);
            if (di.Name.Equals(parentName, StringComparison.OrdinalIgnoreCase)) return di.Name;
            var parent = di.Parent?.Name;
            while (parent != null && !parent.Equals(parentName, StringComparison.OrdinalIgnoreCase))
            {
                parent = new DirectoryInfo(parent).Parent?.Name;
            }
            return parent;
        }
        private static void TryAppendEnvironment(EnvironmentVariableTarget scope, List<string> targets)
        {
            targets.ForEach(t => TryAppendEnvironment(scope, t));
        }
        private static void TryAppendEnvironment(EnvironmentVariableTarget scope, string path)
        {
            if(string.IsNullOrEmpty(path)) return;
            try
            {
                var name = "PATH";
                var oldValue = Environment.GetEnvironmentVariable(name, scope) ?? string.Empty;
                var newValue = string.IsNullOrEmpty(oldValue) ? path : string.Concat(oldValue, ";", path);
                Environment.SetEnvironmentVariable(name, newValue, scope);
            }
            catch (Exception)
            {
                // supression is intended
            }
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(OnTimer, null, TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void OnTimer(object? state)
        {
            if (IsDataReady) { return; }
            try
            {
                var contents = new[] { "chromedriver.exe", "geckodriver.exe" }.ToList();
                contents.ForEach(x =>
                {
                    CopyToApp(x);
                });
                IsDataReady = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
