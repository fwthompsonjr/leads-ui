using legallead.installer.Interfaces;
using legallead.installer.Models;
using System.Diagnostics.CodeAnalysis;

namespace legallead.installer.Classes
{
    [ExcludeFromCodeCoverage(Justification = "This behavior uses interop services. Tested by integration only")]
    internal static class ShortcutBuilder
    {

        public static string? Create(this IShortcutCreator builder, ReleaseAssetModel model, string shortcutPath, bool forDesktop = false)
        {
            var response = GetTargetDirectory(model, shortcutPath, forDesktop);
            if (!response.Item1 || response.Item3 == null) return null;
            var targetDir = response.Item2 ?? string.Empty;
            var matched = response.Item3;
            return builder.Build(model, targetDir, matched);
        }

        private static Tuple<bool, string?, FileInfo?> GetTargetDirectory(ReleaseAssetModel model, string shortcutPath, bool forDesktop = false)
        {
            var invalid = new Tuple<bool, string?, FileInfo?>(false, null, null);
            if (!Directory.Exists(shortcutPath)) { return invalid; }
            var di = new DirectoryInfo(shortcutPath);
            var found = di.GetFiles("*.exe", SearchOption.AllDirectories).ToList();
            if (found.Count == 0) { return invalid; }
            var matched = found.Find(w =>
            {
                var shortName = Path.GetFileNameWithoutExtension(w.Name);
                return IsNameMatched(model.Name, shortName);
            });
            if (matched == null) return invalid;
            var targetDir =
                forDesktop ?
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) :
                Path.GetDirectoryName(Path.GetDirectoryName(matched.FullName));

            if (targetDir == null || !Directory.Exists(targetDir)) return invalid;
            return new Tuple<bool, string?, FileInfo?>(true, targetDir, matched);
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
