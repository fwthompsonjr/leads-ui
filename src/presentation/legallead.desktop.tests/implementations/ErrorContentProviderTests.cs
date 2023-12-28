using legallead.desktop.entities;
using legallead.desktop.implementations;

namespace legallead.desktop.tests.implementations
{
    public class ErrorContentProviderTests
    {
        private static readonly object locker = new();
        private static readonly ErrorContentProvider contentProvider = new();

        public ErrorContentProviderTests()
        {
            lock (locker)
            {
                _ = ErrorContentHtml.ErrorContentList();
            }
        }

        [Fact]
        public void ProviderCanBeCreated()
        {
            Assert.NotNull(contentProvider);
        }

        [Fact]
        public void ProviderHasNames()
        {
            var actual = contentProvider.Names;
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void ProviderHasContentNames()
        {
            var actual = contentProvider.ContentNames;
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void ProviderHasExpectedName()
        {
            lock (locker)
            {
                var expected = new[] { 500, 503, 424, 400, 401, 404, 409 };
                for (int i = 0; i < expected.Length; i++)
                {
                    int index = expected[i];
                    var actual = contentProvider.IsValid(index);
                    Assert.True(actual);
                }
            }
        }

        [Fact]
        public void ProviderCanGetContent()
        {
            lock (locker)
            {
                var expected = new[] { 500, 503, 424, 400, 401, 404, 409, 100, 200 };
                for (int i = 0; i < expected.Length; i++)
                {
                    int index = expected[i];
                    var valid = contentProvider.IsValid(index);
                    var actual = contentProvider.GetContent(index);
                    if (valid) Assert.NotNull(actual);
                    else Assert.Null(actual);
                }
            }
        }
    }
}