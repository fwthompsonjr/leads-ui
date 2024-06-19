using legallead.desktop.interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace legallead.desktop.implementations
{
    internal class HistoryPersistence : IHistoryPersistence
    {
        private readonly IFileInteraction _fileService;
        public HistoryPersistence(IFileInteraction? fileService)
        {
            _fileService = fileService ?? new FileInteraction();
        }
        public void Clear()
        {
            var fileName = HistoryFile;
            ClearFileContent(fileName);
        }

        public void Save(string json)
        {
            var fileName = HistoryFile;
            lock (sync)
            {
                _fileService.WriteAllText(fileName, json);
            }
        }

        public string? Fetch()
        {
            var fileName = HistoryFile;
            lock (sync)
            {
                return _fileService.ReadAllText(fileName);
            }
        }

        private static readonly object sync = new();
        private static string AppFolder => appFolder ??= GetFolder();
        private static string HistoryFolder => historyFolder ??= GetHistoryFolder();
        private static string HistoryFile => historyFile ??= GetHistoryFile();

        private static string? appFolder;
        private static string? historyFolder;
        private static string? historyFile;

        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static string GetFolder()
        {
            var exeName = Assembly.GetExecutingAssembly().Location;
            var exePath = Path.GetDirectoryName(exeName);
            if (string.IsNullOrEmpty(exePath)) return string.Empty;
            if (!Directory.Exists(exePath)) return string.Empty;
            return exePath;
        }
        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static string GetHistoryFolder()
        {
            const string folderName = "_history";
            var parent = AppFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, folderName);
            if (!Directory.Exists(child)) Directory.CreateDirectory(child);
            return child;
        }

        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static string GetHistoryFile()
        {
            const string folderName = "user-history.txt";
            var parent = HistoryFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, folderName);
            if (!File.Exists(child)) return string.Empty;
            return child;
        }
        [ExcludeFromCodeCoverage(Justification = "Performs file i/o operations")]
        private static void ClearFileContent(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return; }
            lock (sync)
            {
                File.Delete(fileName);
                File.WriteAllText(fileName, string.Empty);
            }
        }
    }
}
