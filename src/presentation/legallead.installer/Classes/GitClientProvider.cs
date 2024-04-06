using legallead.installer.Models;
using Octokit;
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace legallead.installer.Classes
{
    [ExcludeFromCodeCoverage(Justification = "Interacts with remote resource. Tested in integration only")]
    public static class GitClientProvider
    {
        static GitClientProvider()
        {
            _ = ProductName;
            _ = AccessToken;
            _ = RepositoryName;
            _ = PackageNames;
        }

        public static GitHubClient GetClient()
        {
            var credentials = new Credentials(AccessToken);
            var client = new GitHubClient(new ProductHeaderValue(ProductName))
            {
                Credentials = credentials
            };
            return client;
        }

        public static async Task<object?> GetAsset(ReleaseAssetModel model)
        {
            var key = $"{model.RepositoryId}-{model.AssetId}";
            if (AssetContents.TryGetValue(key, out object? value)) { return value; }
            var client = GetClient();
            var asset = await client.Repository.Release.GetAsset(
                model.RepositoryId, model.AssetId);
            if (asset == null) { return null; }
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
            if (releases == null)
            {
                ReleaseList = [];
                return ReleaseList;
            }
            ReleaseList = TranslateFrom(repositoryId, releases);
            return ReleaseList;
        }

        public static bool AllowShortcuts
        {
            get
            {
                if (_createShortcut != null) return _createShortcut.Value;
                var setting = SettingProvider.Common();
                _createShortcut = setting.CreateShortcut;
                return _createShortcut.GetValueOrDefault();
            }
        }

        private static async Task<Repository?> GetRepository()
        {
            if (CurrentRepository != null) return CurrentRepository;
            var client = GetClient();
            var repo = (await client.Repository.GetAllForCurrent()).First(x => x.Name.Equals(RepositoryName));
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
                return _accessToken;
            }
        }

        private static string ProductName
        {
            get
            {
                if (!string.IsNullOrEmpty(_productName)) return _productName;
                var setting = SettingProvider.Common();
                _productName = setting.Product;
                return _productName;
            }
        }

        private static string RepositoryName
        {
            get
            {
                if (!string.IsNullOrEmpty(_repositoryName)) return _repositoryName;
                var setting = SettingProvider.Common();
                _repositoryName = setting.Repository;
                return _repositoryName;
            }
        }

        internal static List<string> PackageNames
        {
            get
            {
                if (_packages != null) return _packages;
                var setting = SettingProvider.Common();
                _packages = setting.Packages;
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
                if (pos != -1) { cleanedName = cleanedName[..pos]; }
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
        private static Repository? CurrentRepository
        {
            get
            {
                var search = new RepositoryStorage();
                return search.Find()?.FirstOrDefault();
            }
            set
            {
                if (value == null) { return; }
                var storage = new RepositoryStorage
                {
                    Detail = [value]
                };
                storage.Save();
            }
        }

        private static List<ReleaseModel>? ReleaseList
        {
            get
            {
                var search = new ReleaseModelStorage();
                return search.Find();
            }
            set
            {
                if (value == null) { return; }
                var storage = new ReleaseModelStorage
                {
                    Detail = value
                };
                storage.Save();
            }
        }

        private static List<string>? _packages = default;
        private static string? _accessToken = string.Empty;
        private static string? _productName = string.Empty;
        private static string? _repositoryName = string.Empty;
        private static bool? _createShortcut;
        private static readonly Dictionary<string, object?> AssetContents = [];
    }
}
