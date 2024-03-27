using legallead.installer.Models;
using Octokit;
using System.Net;

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
        public static async Task<object?> GetAsset(Release release)
        {
            if (!RepositoryId.HasValue) { await GetRepository(); }
            var client = GetClient();
            var asset = await client.Repository.Release.GetAsset(RepositoryId.GetValueOrDefault(), release.Assets[0].Id);
            string downloadUrl = asset.BrowserDownloadUrl;

            // Download with WebClient
            using var wb = new HttpClient();
            wb.DefaultRequestHeaders.Add("Accept", "application/octet-stream");
            wb.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36");
            wb.DefaultRequestHeaders.Add("Authorization", $"token {AccessToken}");
            var content = await wb.GetByteArrayAsync(downloadUrl);
            return content;
        }
        public static async Task<IReadOnlyList<Release>?> GetReleases()
        {
            var repo = await GetRepository();
            if (repo == null) { return null; }
            var repositoryId = repo.Id;
            var releases = await GetClient().Repository.Release.GetAll(repositoryId);
            return releases;
        }
        private static long? RepositoryId;
        private static async Task<Repository?> GetRepository()
        {
            var client = GetClient();
            var repo = (await client.Repository.GetAllForCurrent()).First(x => x.Name.Equals("leads-ui"));
            if(!RepositoryId.HasValue && repo != null) { RepositoryId = repo.Id; }
            return repo;
        }

        private static string? _accessToken = string.Empty;
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
    }
}
