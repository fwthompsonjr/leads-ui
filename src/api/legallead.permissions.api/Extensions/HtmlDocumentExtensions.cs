using HtmlAgilityPack;
using legallead.permissions.api.Services;
using System.Text;

namespace legallead.permissions.api.Extensions
{
    public static class HtmlDocumentExtensions
    {
        public static byte[]? GetInnerText(this byte[] data, string tagName) { 
            if (data == null) return null;
            var doc = GetDocument(data);
            var body = doc.DocumentNode.SelectSingleNode("//body");
            if (body == null) return null;
            var node = body.SelectSingleNode(tagName);
            if (node == null || string.IsNullOrEmpty(node.InnerHtml)) return null;
            var text = node.InnerHtml;
            return Encoding.UTF8.GetBytes(text);
        }
        public static string StandardizeBody(this string html)
        {
            try
            {
                const string xpath = "//body";
                var document = new HtmlDocument();
                document.LoadHtml(html);
                var body = document.DocumentNode.SelectSingleNode(xpath);
                if (body == null || string.IsNullOrEmpty(body.InnerHtml)) return html;
                var doc = GetDocument(body.InnerHtml);
                var raw = doc.DocumentNode.OuterHtml;
                var cleaned = HtmlStandardizeService.BeautifyHTML(raw);
                doc = new HtmlDocument();
                doc.LoadHtml(cleaned);
                var bd = doc.DocumentNode.SelectSingleNode(xpath);
                if (bd == null || string.IsNullOrEmpty(bd.InnerHtml)) return html;
                body.InnerHtml = bd.InnerHtml;
                return document.DocumentNode.OuterHtml;
            }
            catch (Exception)
            {
                return html;
            }
        }
        private static HtmlDocument GetDocument(byte[] source)
        {
            var decoded = Encoding.UTF8.GetString(source);
            return GetDocument(decoded);
        }
        private static HtmlDocument GetDocument(string source)
        {
            var html = string.Join(Environment.NewLine, new List<string>()
            {
                "<html>",
                "<body>",
                source,
                "</body>",
                "</html>"
            });
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
    }
}
