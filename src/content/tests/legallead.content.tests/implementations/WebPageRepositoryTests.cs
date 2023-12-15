using legallead.content.implementations;
using legallead.content.interfaces;
using Moq;

namespace legallead.content.tests.implementations
{
    public class WebPageRepositoryTests
    {
        [Fact]
        public void RepoCanGetLineRepository()
        {
            var lines = new Mock<IWebContentLineRepository>();
            var pages = new Mock<IWebContentRepository>();
            var sut = new WebPageRepository(lines.Object, pages.Object);
            Assert.NotNull(sut.LineRepository);
        }

        [Fact]
        public void RepoCanGetContentRepository()
        {
            var lines = new Mock<IWebContentLineRepository>();
            var pages = new Mock<IWebContentRepository>();
            var sut = new WebPageRepository(lines.Object, pages.Object);
            Assert.NotNull(sut.ContentRepository);
        }
    }
}