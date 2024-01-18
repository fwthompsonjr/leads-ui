using HtmlAgilityPack;
using legallead.records.search.Classes;
using OpenQA.Selenium;

namespace legallead.records.search.Tools
{
    internal static class ElementExtensions
    {

        public static HtmlNode? FindParent(this HtmlNode element, string tagName)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            const string locator = "..";
            var target = element.SelectSingleNode(locator);
            while (target != null && !target.Name.Equals(tagName, oic))
            {
                target = target.SelectSingleNode(locator);
            }
            if (target != null && target.Name.Equals(tagName, oic)) return target;
            return null;
        }

        public static IWebElement? FindParent(this IWebElement element, string tagName)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            const string locator = "..";
            var search = By.XPath(locator);
            var target = element.TryFindElement(search);
            while (target != null && !target.TagName.Equals(tagName, oic))
            {
                target = target.TryFindElement(search);
            }
            if (target != null && target.TagName.Equals(tagName, oic)) return target;
            return null;
        }

        public static HtmlNode? GetNode(this string content)
        {
            try
            {
                var document = new HtmlDocument();
                var doc = $"<html><body>{content}</body></html>";
                document.LoadHtml(doc);
                var body = document.DocumentNode.SelectSingleNode("//body");
                return body?.FirstChild;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
