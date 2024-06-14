using legallead.desktop.interfaces;
using System.Reflection;
using System.Text;

namespace legallead.desktop.implementations
{
    internal class MailPersistence : IMailPersistence
    {
        public void Clear()
        {
            var fileName = MailFile;
            var folderName = MailSubFolder;
            ClearFileContent(fileName);
            ClearChildContent(folderName);
        }

        public void Save(string json)
        {
            var fileName = MailFile;
            if (string.IsNullOrEmpty(fileName)) { return; }
            lock (sync)
            {
                var array = Encoding.UTF8.GetBytes(json);
                var content = Convert.ToBase64String(array);
                if (File.Exists(fileName)) File.Delete(fileName);
                File.WriteAllText(fileName, content);
            }
        }
        public void Save(string id, string json)
        {
            var folderName = MailSubFolder;
            if (string.IsNullOrEmpty(folderName)) { return; }
            if (!Guid.TryParse(id, out var indx)) { return; }
            var suffix = $"{indx:D}.txt";
            var fileName = Path.Combine(folderName, suffix);
            var array = Encoding.UTF8.GetBytes(json);
            var content = Convert.ToBase64String(array);
            if (File.Exists(fileName)) File.Delete(fileName);
            File.WriteAllText(fileName, content);
        }

        public bool DoesItemExist(string id)
        {
            var folderName = MailSubFolder;
            if (string.IsNullOrEmpty(folderName)) { return false; }
            if (!Guid.TryParse(id, out var indx)) { return false; }
            var suffix = $"{indx:D}.txt";
            var fileName = Path.Combine(folderName, suffix);
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName)) { return false; }
            return true;
        }

        public string? Fetch()
        {
            var fileName = MailFile;
            if (string.IsNullOrEmpty(fileName)) { return string.Empty; }
            lock (sync)
            {
                try
                {
                    var content = File.ReadAllText(fileName);
                    var array = Convert.FromBase64String(content);
                    var converted = Encoding.UTF8.GetString(array);
                    return converted;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public string? Fetch(string id)
        {
            var folderName = MailSubFolder;
            if (string.IsNullOrEmpty(folderName)) { return null; }
            if (!Guid.TryParse(id, out var indx)) { return null; }
            var suffix = $"{indx:D}.txt";
            var fileName = Path.Combine(folderName, suffix);
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName)) { return null; }
            try
            {
                using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);
                var content = reader.ReadToEnd();
                var array = Convert.FromBase64String(content);
                var converted = Encoding.UTF8.GetString(array);
                return converted;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static readonly object sync = new();
        private static string AppFolder => appFolder ??= GetFolder();
        private static string MailFolder => mailFolder ??= GetMailFolder();
        private static string MailSubFolder => mailSubFolder ??= GetMailSubFolder();
        private static string MailFile => mailFile ??= GetMailFile();

        private static string? appFolder;
        private static string? mailFolder;
        private static string? mailFile;
        private static string? mailSubFolder;
        private static string GetFolder()
        {
            var exeName = Assembly.GetExecutingAssembly().Location;
            var exePath = Path.GetDirectoryName(exeName);
            if (string.IsNullOrEmpty(exePath)) return string.Empty;
            if (!Directory.Exists(exePath)) return string.Empty;
            return exePath;
        }
        private static string GetMailFolder()
        {
            const string folderName = "_mailbox";
            var parent = AppFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, folderName);
            if (!Directory.Exists(child)) return string.Empty;
            return child;
        }
        private static string GetMailSubFolder()
        {
            const string folderName = "_letters";
            var parent = MailFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, folderName);
            if (!Directory.Exists(child)) Directory.CreateDirectory(child);
            return child;
        }
        private static string GetMailFile()
        {
            const string folderName = "user-data.txt";
            var parent = MailFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, folderName);
            if (!File.Exists(child)) return string.Empty;
            return child;
        }
        private static void ClearFileContent(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return; }
            lock (sync)
            {
                File.Delete(fileName);
                File.WriteAllText(fileName, string.Empty);
            }
        }
        private static void ClearChildContent(string folderName)
        {
            if (string.IsNullOrEmpty(folderName)) { return; }
            lock (sync)
            {
                var di = new DirectoryInfo(folderName);
                di.Delete(true);
                if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);
            }
        }
    }
}
