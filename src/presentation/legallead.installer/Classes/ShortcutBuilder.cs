using legallead.installer.Models;
using System.Diagnostics.CodeAnalysis;

namespace legallead.installer.Classes
{
    [ExcludeFromCodeCoverage(Justification = "This behavior is not in use. To be deleted.")]
    internal static class ShortcutBuilder
    {
        public static void CreateShortCut(ReleaseAssetModel model, string shortcutPath, bool forDesktop = false)
        {
            try
            {
                if (!Directory.Exists(shortcutPath)) { return; }
                var di = new DirectoryInfo(shortcutPath);
                var found = di.GetFiles("*.exe", SearchOption.AllDirectories).ToList();
                if (found.Count == 0) { return; }
                var matched = found.Find(w =>
                {
                    var shortName = Path.GetFileNameWithoutExtension(w.Name);
                    return IsNameMatched(model.Name, shortName);
                });
                if (matched == null) return;
                var targetDir =
                    forDesktop ?
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop) :
                    Path.GetDirectoryName(Path.GetDirectoryName(matched.FullName));

                if (targetDir == null || !Directory.Exists(targetDir)) return;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool IsNameMatched(string name, string shortName)
        {
            const char dash = '-';
            if (string.IsNullOrEmpty(name)) { return false; }
            if (name.Contains(dash))
            {
                name = name.Split(dash)[0];
            }
            var isMatched = shortName.Contains(name, StringComparison.OrdinalIgnoreCase);
            return isMatched;
        }
    }
}
