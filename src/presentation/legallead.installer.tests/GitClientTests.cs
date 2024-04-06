using legallead.installer.Classes;
using Octokit;
using System.Diagnostics;
using System.Reflection;

namespace legallead.installer.tests
{
    public class GitClientTests
    {
        private static readonly object locker = new();

        [Fact]
        public async Task ClientCanGetUser()
        {
            var client = GitClientProvider.GetClient();
            var user = await client.User.Current();
            Assert.NotNull(user);
        }

        [Fact]
        public async Task ClientCanGetRepositories()
        {
            var repo = await GetRepository();
            Assert.NotNull(repo);
        }

        [Fact]
        public async Task ClientCanGetReleases()
        {
            var releases = await GitClientProvider.GetReleases();
            Assert.NotNull(releases);
            Assert.NotEmpty(releases);
        }

        [Fact]
        public async Task ClientCanGetAsset()
        {
            if (!Debugger.IsAttached) return;
            var releases = await GitClientProvider.GetReleases();
            Assert.NotNull(releases);
            Assert.NotEmpty(releases);
            var release = releases[0];
            Assert.NotEmpty(release.Assets);
            var content = await GitClientProvider.GetAsset(release.Assets[0]);
            Assert.NotNull(content);
        }

        [Fact]
        public async Task ClientCanGetAssetIntegration()
        {
            if (!Debugger.IsAttached) return;
            var releases = await GitClientProvider.GetReleases();
            Assert.NotNull(releases);
            Assert.NotEmpty(releases);
            var release = releases[0];
            Assert.NotEmpty(release.Assets);
            var content = await GitClientProvider.GetAsset(release.Assets[0]);
            Assert.NotNull(content);
            if (content is not byte[] data) return;
            var manager = new LeadFileOperation();
            string extractDir = Path.Combine(CurrentDir, "tmp_extract");
            lock (locker)
            {
                try
                {
                    manager.Extract(extractDir, data);
                    Assert.True(Directory.Exists(extractDir));
                }
                finally
                {
                    if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
                }
            }
        }
        private static async Task<Repository?> GetRepository()
        {
            var client = GitClientProvider.GetClient();
            var repo = (await client.Repository.GetAllForCurrent()).First(x => x.Name.Equals("leads-ui"));
            return repo;
        }
        private static string? _currentDir;
        private static string CurrentDir
        {
            get
            {
                if (_currentDir != null) return _currentDir;
                _currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return _currentDir ?? string.Empty;
            }
        }
    }
}