using legallead.email.transforms;

namespace legallead.email.tests.transforms
{
    public class HtmlBeautifyServiceTests
    {
        // 
        private readonly IHtmlBeautifyService parser;
        private static readonly string testHtml = Properties.Resources.parser_test_html;
        public HtmlBeautifyServiceTests()
        {
            parser = new HtmlBeautifyService();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ParserCanParse(int index)
        {
            var test = ContentSamples[index];
            var actual = parser.BeautifyHTML(test);
            Assert.False(string.IsNullOrEmpty(actual));
            Assert.NotEqual(test, actual);
        }

        private static readonly List<string> ContentSamples = new() {
            "<html></html>",
            "<html><body></body></html>",
            "<html><body><div><p>This is a sentence.</div></body></html>",
            testHtml
        };
    }
}
