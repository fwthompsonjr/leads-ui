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
    }
}