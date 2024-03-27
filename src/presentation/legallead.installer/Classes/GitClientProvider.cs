using legallead.installer.Models;
using Octokit;

namespace legallead.installer.Classes
{
    public static class GitClientProvider
    {
        public static GitHubClient GetClient()
        {
            var credentials = new Credentials(AccessToken);
            var client = new GitHubClient(new ProductHeaderValue("legallead.installer"))
            {
                Credentials = credentials
            };
            return client;
        }

        public static async Task<object?> GetAsset(ReleaseAssetModel model)
        {
            var key = $"{model.RepositoryId}-{model.AssetId}";
            if (AssetContents.ContainsKey(key)) { return AssetContents[key]; }
            var client = GetClient();
            var asset = await client.Repository.Release.GetAsset(
                model.RepositoryId, model.AssetId);
            string downloadUrl = asset.BrowserDownloadUrl;

            // Download with WebClient
            using var wb = new HttpClient();
            wb.DefaultRequestHeaders.Add("Accept", "application/octet-stream");
            wb.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
            wb.DefaultRequestHeaders.Add("Authorization", $"token {AccessToken}");
            var content = await wb.GetByteArrayAsync(downloadUrl);
            AssetContents[key] = content;
            return content;
        }

        public static async Task<List<ReleaseModel>?> GetReleases()
        {
            if (ReleaseList != null) return ReleaseList;
            var repo = await GetRepository();
            if (repo == null) { return null; }
            var repositoryId = repo.Id;
            var releases = await GetClient().Repository.Release.GetAll(repositoryId);
            if (releases == null) { 
                ReleaseList = [];
                return ReleaseList; 
            }
            ReleaseList = TranslateFrom(repositoryId, releases);
            return ReleaseList;
        }
        
        private static async Task<Repository?> GetRepository()
        {
            if (CurrentRepository != null) return CurrentRepository;
            var client = GetClient();
            var repo = (await client.Repository.GetAllForCurrent()).First(x => x.Name.Equals("leads-ui"));
            if (!RepositoryId.HasValue && repo != null) { RepositoryId = repo.Id; }
            CurrentRepository = repo;
            return CurrentRepository;
        }

        private static string AccessToken
        {
            get
            {
                if (!string.IsNullOrEmpty(_accessToken)) return _accessToken;
                var setting = SettingProvider.Common();
                _accessToken = setting.Key;
                _packages ??= setting.Packages;
                return _accessToken;
            }
        }

        internal static List<string> PackageNames
        {
            get
            {
                if (_packages != null) return _packages;
                var setting = SettingProvider.Common();
                _packages ??= setting.Packages;
                return _packages;
            }
        }

        private static bool IsPackageNameMatched(string packageName)
        {
            var items = PackageNames.Select(x => packageName.StartsWith(x)).ToList();
            return items.Exists(x => x);
        }


        private static List<ReleaseModel> TranslateFrom(long repositoryId, IEnumerable<Release> releases)
        {
            var results = new List<ReleaseModel>();
            foreach (var release in releases)
            {
                var releaseModel = new ReleaseModel
                {
                    Id = release.Id,
                    Name = release.TagName,
                    RepositoryId = repositoryId,
                    PublishDate = release.PublishedAt.GetValueOrDefault().Date
                };
                var assets = release.Assets.Where(w =>
                {
                    return IsPackageNameMatched(w.Name);
                });
                if (!assets.Any()) { continue; }
                releaseModel.Assets = TranslateFrom(repositoryId, assets);
                releaseModel.Assets.ForEach(a => a.Version = releaseModel.Name);
                results.Add(releaseModel);
            }
            return results;
        }

        private static List<ReleaseAssetModel> TranslateFrom(long repositoryId, IEnumerable<ReleaseAsset> assets)
        {
            var results = assets.Select(a =>
            {
                var cleanedName = Path.GetFileNameWithoutExtension(a.Name);
                var pos = cleanedName.LastIndexOf('-');
                if (pos != -1) { cleanedName = cleanedName.Substring(0, pos); }
                return new ReleaseAssetModel
                {
                    RepositoryId = repositoryId,
                    Name = cleanedName,
                    AssetId = a.Id,
                    DownloadUrl = a.BrowserDownloadUrl
                };
            }).ToList();
            return results;
        }

        private static long? RepositoryId;
        private static Repository? CurrentRepository;
        private static List<ReleaseModel>? ReleaseList;

        private static List<string>? _packages = default;
        private static string? _accessToken = string.Empty;
        private static readonly Dictionary<string, object?> AssetContents = new();
    }
}
