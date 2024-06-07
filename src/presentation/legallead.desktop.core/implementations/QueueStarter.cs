using legallead.desktop.interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.implementations
{
    internal class QueueStarter : IQueueStarter
    {
        private readonly IQueueSettings settingsModel;
        private readonly IQueueStopper stoppingService;
        public QueueStarter(IQueueSettings settings, IQueueStopper stopper)
        {
            settingsModel = settings;
            stoppingService = stopper;
            ParentName = TargetFolder(settings.FolderName);
        }

        public string ServiceName => stoppingService.ServiceName;

        public void Start()
        {
            if (!settingsModel.IsEnabled) { return; }
            if (string.IsNullOrEmpty(ServiceName)) { return; }
            if (string.IsNullOrEmpty(ParentName)) { return; }
            var name = settingsModel.Name;
            if (string.IsNullOrEmpty(name) || IsProcRunning(name)) { return; }
            var pattern = $"{name}.exe";
            stoppingService.Stop();
            var collection = GetFiles(ParentName, pattern).ToList();
            if (collection.Count == 0) return;
            collection.Sort((a,b) => b.CreationTime.CompareTo(a.CreationTime));
            var target = collection[0];
            LaunchExe(target);
        }

        private static bool IsProcRunning(string processName)
        {
            var processes = Process.GetProcessesByName(processName).ToList();
            return processes.Count > 0;
        }

        private string ParentName { get; set; }

        private static void LaunchExe(FileInfo target)
        {
            var path = target.FullName;
            if (!File.Exists(path)) return;            
            Process myProcess = new();
            var info = myProcess.StartInfo;
            info.WorkingDirectory = Path.GetDirectoryName(path);
            info.WindowStyle = ProcessWindowStyle.Normal;
            info.FileName = path;
            info.CreateNoWindow = false;
            info.UseShellExecute = true;
            myProcess.Start();
        }

        private static IEnumerable<FileInfo> GetFiles(string source, string pattern)
        {
            var empty = Enumerable.Empty<FileInfo>();
            if (string.IsNullOrEmpty (source)) { return empty; }
            if (!Directory.Exists (source)) { return empty; }
            var found = new DirectoryInfo (source).GetFiles(pattern, SearchOption.AllDirectories);
            return found;
        }

        private static string TargetFolder(string? subDirectory)
        {
            var parent = AppFolder;
            if (string.IsNullOrWhiteSpace(subDirectory)) { return string.Empty; }
            if (!parent.Contains(subDirectory, StringComparison.OrdinalIgnoreCase)) return string.Empty;
            var a = subDirectory.Length;
            var b = parent.IndexOf(subDirectory, StringComparison.OrdinalIgnoreCase);
            if (b == -1) return string.Empty;
            var sourceDir = parent.Substring(0, b + a);
            if (!Directory.Exists(sourceDir)) return string.Empty;
            return sourceDir;
        }

        private static string? _appFolder;
        private static string AppFolder => _appFolder ??= GetAppFolder();
        private static string GetAppFolder()
        {
            var exeFile = Assembly.GetExecutingAssembly().Location;
            if (string.IsNullOrEmpty(exeFile)) { return string.Empty; }
            var exePath = Path.GetDirectoryName(exeFile);
            if (string.IsNullOrEmpty(exePath) ||
                !Directory.Exists(exePath)) { return string.Empty; }
            return exePath;
        }
    }
}
