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
            if (string.IsNullOrEmpty(fileName)) { return; }
            lock (sync)
            {
                File.Delete(fileName);
                File.WriteAllText(fileName, string.Empty);
            }
        }

        public void Save(string json)
        {
            var fileName = MailFile;
            if (string.IsNullOrEmpty(fileName)) { return; }
            lock (sync)
            {
                var array = Encoding.UTF8.GetBytes(json);
                var content = Convert.ToBase64String(array);
                File.Delete(fileName);
                File.WriteAllText(fileName, content);
            }
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

        private static readonly object sync = new();
        private static string AppFolder => appFolder ??= GetFolder();
        private static string MailFolder => mailFolder ??= GetMailFolder();
        private static string MailFile => mailFile ??= GetMailFile();

        private static string? appFolder;
        private static string? mailFolder;
        private static string? mailFile;
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
        private static string GetMailFile()
        {
            const string folderName = "user-data.txt";
            var parent = MailFolder;
            if (!Directory.Exists(parent)) return string.Empty;
            var child = Path.Combine(parent, folderName);
            if (!File.Exists(child)) return string.Empty;
            return child;
        }
    }
}
