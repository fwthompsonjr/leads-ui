using AngleSharp.Html;
using AngleSharp.Html.Parser;
using legallead.desktop.interfaces;

namespace legallead.desktop.implementations
{
    internal class ContentParser : IContentParser
    {
        public string BeautfyHTML(string html)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);
            using var writer = new StringWriter();
            document.ToHtml(writer, new PrettyMarkupFormatter
            {
                Indentation = "\t",
                NewLine = "\n"
            });
            return writer.ToString();
        }
    }
}