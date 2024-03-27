using legallead.installer.Classes;
using Octokit;

namespace legallead.installer.tests
{
    public class GitClientTests
    {
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
            var releases = await GitClientProvider.GetReleases();
            Assert.NotNull(releases);
            Assert.NotEmpty(releases);
            var release = releases[0];
            var content = await GitClientProvider.GetAsset(release);
            Assert.NotNull(content);
        }

        private static async Task<Repository?> GetRepository()
        {
            var client = GitClientProvider.GetClient();
            var repo = (await client.Repository.GetAllForCurrent()).First(x => x.Name.Equals("leads-ui"));
            return repo;
        }
    }
}