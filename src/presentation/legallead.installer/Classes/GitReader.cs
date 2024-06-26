﻿using legallead.installer.Interfaces;
using legallead.installer.Models;
using System.Diagnostics.CodeAnalysis;

namespace legallead.installer.Classes
{
    public class GitReader : IGitReader
    {
        public bool AllowShortcuts => GitClientProvider.AllowShortcuts;

        [ExcludeFromCodeCoverage(Justification = "Method interacts with 3rd party resources")]
        public async Task<List<ReleaseModel>?> GetReleases()
        {
            var releases = await GitClientProvider.GetReleases();
            var alternates = await GitReaderRepository.GetReleases();
            return GetReleases(releases, alternates);
        }

        [ExcludeFromCodeCoverage(Justification = "Method interacts with 3rd party resources")]
        public async Task<List<ReleaseAssetModel>?> GetAssets()
        {
            var releases = await GetReleases();
            return GetAssets(releases);
        }

        public ReleaseAssetModel? FindAsset(List<ReleaseModel> models, string version, string app)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;

            if (string.IsNullOrWhiteSpace(version) && string.IsNullOrWhiteSpace(app)) return null;
            var alt = models
                .Where(w => w.RepositoryName == "leads-reader")
                .SelectMany(x => x.Assets).ToList().FindAll(x => x.Name.StartsWith(app, oic));
            var list = models
                .Where(w => w.RepositoryName != "leads-reader")
                .SelectMany(x => x.Assets).ToList().FindAll(x => x.Name.StartsWith(app, oic));
            list.AddRange(alt);
            if (list.Count == 1 || string.IsNullOrEmpty(version)) return list[0];
            var item = list.Find(x => x.Name.Equals(app, oic) && x.Version.Equals(version, oic));
            return item;
        }
        public bool VerifyPackageName(string packageName)
        {
            if (string.IsNullOrWhiteSpace(packageName)) return false;
            packageName = packageName.Trim();
            var found = GitClientProvider.PackageNames.Find(x => x.StartsWith(packageName, StringComparison.OrdinalIgnoreCase));
            return found != null;
        }
        [ExcludeFromCodeCoverage(Justification = "Method is tested from static class")]
        public async Task<object?> GetAsset(ReleaseAssetModel model)
        {
            var data = await GitClientProvider.GetAsset(model);
            return data;
        }

        protected static List<ReleaseModel>? GetReleases(
            List<ReleaseModel>? releases,
            List<ReleaseModel>? alternates)
        {
            if (releases == null && alternates != null) return alternates;
            if (releases != null && alternates != null) { releases.AddRange(alternates); }
            return releases;
        }

        protected static List<ReleaseAssetModel>? GetAssets(List<ReleaseModel>? releases)
        {
            if (releases == null) return null;
            var assets = releases.SelectMany(s => s.Assets).ToList();
            assets.Sort((a, b) =>
            {
                var aa = a.Name.CompareTo(b.Name);
                if (aa != 0) return aa;
                return b.Version.CompareTo(a.Version);
            });
            return assets;
        }
    }
}
