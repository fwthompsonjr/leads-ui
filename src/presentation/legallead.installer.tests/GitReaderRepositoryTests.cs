using legallead.installer.Classes;

namespace legallead.installer.tests
{
    public class GitReaderRepositoryTests
    {

        [Fact]
        public async Task ReaderCanGetRepository()
        {
            var issues = await Record.ExceptionAsync(async () =>
            {
                var repo = await GitReaderRepository.GetRepository();
                Assert.NotNull(repo);
            });
            Assert.Null(issues);
        }

        [Fact]
        public async Task ReaderCanFindReleases()
        {
            var issues = await Record.ExceptionAsync(async () =>
            {
                var results = await GitReaderRepository.GetReleases();
                Assert.NotNull(results);
            });
            Assert.Null(issues);
        }

        [Fact]
        public async Task ReaderCanFindPullRequests()
        {
            var issues = await Record.ExceptionAsync(async () =>
            {
                var results = await GitReaderRepository.GetResults();
                Assert.NotNull(results);
                Assert.False(results.TotalCount == 0);
            });
            Assert.Null(issues);

        }
    }
}
