
using legallead.installer.Models;
using Octokit;
using System.Diagnostics.CodeAnalysis;

namespace legallead.installer.Classes
{
    [ExcludeFromCodeCoverage(Justification = "Interacts with remote resource. Tested in integration only")]
    public static class GitReaderRepository
    {
        static GitReaderRepository()
        {
            _ = ProductName;
            _ = AccessToken;
            _ = RepositoryName;
            _ = PackageNames;
            _ = ReaderName;
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
        public static async Task<List<ReleaseModel>?> GetReleases()
        {
            var repo = await GetRepository();
            if (repo == null) { return null; }
            var repositoryId = repo.Id;
            var releases = await GetClient().Repository.Release.GetAll(repositoryId);
            if (releases == null)
            {
                return [];
            }
            var translated = TranslateFrom(repositoryId, releases);
            return translated;
        }

        public static async Task<Repository?> GetRepository()
        {
            var client = GetClient();
            var repo = (await client.Repository.GetAllForCurrent()).First(x => x.Name.Equals(ReaderName));
            if (!RepositoryId.HasValue && repo != null) { RepositoryId = repo.Id; }
            return repo;
        }

        public static async Task<SearchIssuesResult> GetResults()
        {
            var client = GetClient();
            var request = new SearchIssuesRequest();
            var repoName = $"fwthompsonjr/{ReaderName}";
            var threeMonthsAgo = new DateTimeOffset(DateTime.Now.Subtract(TimeSpan.FromDays(90)));
            request.Repos.Add(repoName);
            request.Created = new DateRange(threeMonthsAgo, SearchQualifierOperator.GreaterThan);
            request.Type = IssueTypeQualifier.PullRequest;
            request.Labels = new List<string>() { "approved" };
            request.State = ItemState.Closed;
            var issues = await client.Search.SearchIssues(request);
            return issues;
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
        private static string ReaderName
        {
            get
            {
                if (!string.IsNullOrEmpty(_readerName)) return _readerName;
                var setting = SettingProvider.Common();
                _readerName = setting.ReadRepository;
                return _readerName;
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
                    PublishDate = release.PublishedAt.GetValueOrDefault().Date,
                    RepositoryName = ReaderName
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


        private static List<string>? _packages = default;
        private static string? _accessToken = string.Empty;
        private static string? _productName = string.Empty;
        private static string? _repositoryName = string.Empty;
        private static string? _readerName = string.Empty;
    }
}