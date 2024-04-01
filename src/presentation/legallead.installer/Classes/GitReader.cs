using legallead.installer.Interfaces;
using legallead.installer.Models;
using System.Diagnostics.CodeAnalysis;

namespace legallead.installer.Classes
{
    public class GitReader : IGitReader
    {
        public bool AllowShortcuts => GitClientProvider.AllowShortcuts;

        public async Task<List<ReleaseModel>?> GetReleases()
        {
            var releases = await GitClientProvider.GetReleases();
            return releases;
        }

        public async Task<List<ReleaseAssetModel>?> GetAssets()
        {
            var releases = await GitClientProvider.GetReleases();
            if (releases == null) return null;
            return releases.SelectMany(s => s.Assets).ToList();
        }

        public ReleaseAssetModel? FindAsset(List<ReleaseModel> models, string version, string app)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;

            if (string.IsNullOrWhiteSpace(version)) return null;
            if (string.IsNullOrWhiteSpace(app)) return null;
            var list = models.SelectMany(x => x.Assets).ToList();
            var item = list.Find(x => x.Name.Equals(app, oic) && x.Version.Equals(version, oic));
            return item;
        }
        public bool VerifyPackageName(string packageName)
        {
            if (string.IsNullOrWhiteSpace(packageName)) return false;
            packageName = packageName.Trim();
            return GitClientProvider.PackageNames.Contains(packageName, StringComparer.OrdinalIgnoreCase);
        }
        [ExcludeFromCodeCoverage(Justification = "Method is tested from static class")]
        public async Task<object?> GetAsset(ReleaseAssetModel model)
        {
            var data = await GitClientProvider.GetAsset(model);
            return data;
        }
    }
}
