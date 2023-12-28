using legallead.desktop.entities;

namespace legallead.desktop.tests.entities
{
    public class ErrorContentHtmlTests
    {
        private static readonly object locker = new();
        private readonly List<ErrorContentHtml> errors;

        public ErrorContentHtmlTests()
        {
            lock (locker)
            {
                errors = ErrorContentHtml.ErrorContentList();
            }
        }

        [Fact]
        public void ErrorContentCanGetList()
        {
            var list = errors;
            var assertion = list != null && list.Count > 0;
            Assert.True(assertion);
        }

        [Fact]
        public void ErrorContentGetListDoesNotRebuild()
        {
            var list = errors;
            for (var i = 0; i < 2; i++)
            {
                var actual = ErrorContentHtml.ErrorContentList();
                Assert.Equal(list, actual);
            }
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell",
            "S2925:\"Thread.Sleep\" should not be used in tests",
            Justification = "Having issue with multi-threaded testing.")]
        public void ErrorContentContainsCode()
        {
            lock (locker)
            {
                var expected = new[] { 500, 503, 424, 400, 401, 404, 409 };
                for (int i = 0; i < expected.Length; i++)
                {
                    int statusCode = expected[i];
                    var list = errors;
                    var item = list.Find(x => x.StatusCode == statusCode);
                    if (item == null)
                    {
                        Thread.Sleep(500);
                        item = list.Find(x => x.StatusCode == statusCode);
                    }
                    Assert.NotNull(item);
                }
            }
        }

        [Fact]
        public void ErrorContentExpectedLength()
        {
            lock (locker)
            {
                const int expected = 7;
                var list = errors;
                Assert.NotNull(list);
                Assert.NotEmpty(list);
                Assert.Equal(expected, list.Count);
            }
        }

        [Fact]
        public void ErrorContentHasOneDefaultItem()
        {
            lock (locker)
            {
                var list = errors.Where(w => w.IsDefault);
                Assert.NotNull(list);
                Assert.Single(list);
            }
        }
    }
}