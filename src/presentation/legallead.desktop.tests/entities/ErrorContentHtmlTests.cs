using legallead.desktop.entities;

namespace legallead.desktop.tests.entities
{
    public class ErrorContentHtmlTests
    {
        [Fact]
        public void ErrorContentCanGetList()
        {
            var list = ErrorContentHtml.ErrorContentList();
            var assertion = list != null && list.Count > 0;
            Assert.True(assertion);
        }

        [Fact]
        public void ErrorContentGetListDoesNotRebuild()
        {
            var list = ErrorContentHtml.ErrorContentList();
            for (var i = 0; i < 2; i++)
            {
                var actual = ErrorContentHtml.ErrorContentList();
                Assert.Equal(list, actual);
            }
        }

        [Theory]
        [InlineData(500)]
        [InlineData(503)]
        [InlineData(424)]
        [InlineData(400)]
        [InlineData(401)]
        [InlineData(404)]
        [InlineData(409)]
        public void ErrorContentContainsCode(int statusCode)
        {
            var list = ErrorContentHtml.ErrorContentList();
            var item = list.Find(x => x.StatusCode == statusCode);
            Assert.NotNull(item);
        }
    }
}