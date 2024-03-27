using legallead.installer.Classes;

namespace legallead.installer.tests
{
    public class GitReaderTests
    {
        [Fact]
        public async Task ClientCanGetReleases()
        {
            var client = new GitReader();
            var releases = await client.GetReleases();
            Assert.NotNull(releases);
            Assert.NotEmpty(releases);
        }

        [Fact]
        public async Task ClientCanGetAsset()
        {
            var client = new GitReader();
            var releases = await client.GetReleases();
            Assert.NotNull(releases);
            Assert.NotEmpty(releases);
            var release = releases[0];
            Assert.NotEmpty(release.Assets);
            var content = await client.GetAsset(release.Assets[0]);
            Assert.NotNull(content);
        }
    }
}