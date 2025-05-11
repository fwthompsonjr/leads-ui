using git.project.reader.interfaces;
using git.project.reader.models;
using git.project.reader.Properties;
using Newtonsoft.Json;
using Octokit;
using System.Text;

namespace git.project.reader.assets
{
    public class ReleasesManager : IReleaseProvider
    {
        private readonly GitHubClient? _client;
        public ReleasesManager()
        {
            var pat = Settings.GetPat();
            if (string.IsNullOrWhiteSpace(pat))
            {
                _client = null;
                return;
            }
            _client = new GitHubClient(new ProductHeaderValue(ProductHeader))
            {
                Credentials = new Credentials(pat)
            };
        }

        public async Task<List<ReleaseModel>> ListReleases()
        {
            if (_client == null) return [];
            string owner = Settings.UserName;
            string repo = Settings.Repository;
            var releases = await _client.Repository.Release.GetAll(owner, repo);
            if (releases == null || releases.Count == 0) return [];
            var items = new List<ReleaseModel>();
            foreach (var release in releases)
            {
                items.Add(new ReleaseModel
                {
                    Url = release.Url,
                    HtmlUrl = release.HtmlUrl,
                    AssetsUrl = release.AssetsUrl,
                    UploadUrl = release.UploadUrl,
                    Id = release.Id,
                    NodeId = release.NodeId,
                    TagName = release.TagName,
                    TargetCommitish = release.TargetCommitish,
                    Name = release.Name,
                    Body = release.Body,
                    Draft = release.Draft,
                    Prerelease = release.Prerelease,
                    CreatedAt = release.CreatedAt,
                    PublishedAt = release.PublishedAt,
                    Author = release.Author,
                    TarballUrl = release.TarballUrl,
                    ZipballUrl = release.ZipballUrl,
                });
            }
            return items;
        }

        public async Task<List<AssetModel>> ListAssets(long releaseId)
        {
            if (_client == null) return [];
            string owner = Settings.UserName;
            string repo = Settings.Repository;
            var release = await _client.Repository.Release.Get(owner, repo, releaseId);
            if (release == null) return [];
            var assets = release.Assets.Select(x => new AssetModel
            {
                ReleaseId = releaseId,
                Name = x.Name,
                Id = x.Id,
                DownloadUrl = x.BrowserDownloadUrl
            });
            return [.. assets];
        }

        public async Task<byte[]?> DownloadAssetAsync(long releaseId, string assetName)
        {
            if (_client == null) return null;

            string owner = Settings.UserName;
            string repo = Settings.Repository;
            var release = await _client.Repository.Release.Get(owner, repo, releaseId);
            var asset = release.Assets.ToList().Find(a => a.Name.Equals(assetName));
            if (asset == null) return null;
            var response = await _client.Connection.Get<object>(new Uri(asset.BrowserDownloadUrl), TimeSpan.FromMinutes(2));
            if (response == null) return null;
            var httpResponseMessage = response.HttpResponse;
            if (httpResponseMessage.Body is not Stream httpResponse) return null;

            using var memoryStream = new MemoryStream();
            await httpResponse.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        private static SettingsModel? model;
        private static SettingsModel Settings => model ??= GetSettings();
        private static SettingsModel GetSettings()
        {
            var data = Resources.settings_json;
            var txt = Encoding.UTF8.GetString(data);
            var obj = JsonConvert.DeserializeObject<SettingsModel>(txt);
            return obj ?? new SettingsModel();
        }
        private const string ProductHeader = "git.project.reader";
    }
}
