using HtmlAgilityPack;
using legallead.email.models;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace legallead.email.utility
{
    public static class UserRecordSearchExtensions
    {
        public static string ToHtml(this UserRecordSearch search, 
            UserAccountByEmailBo? user,
            string html)
        {
            var doc = GetDocument(html);
            if (doc == null) { return html; }
            var request = search.Search;
            var dictionary = new Dictionary<string, string>()
            {
                { "//span[@name='begin-search-requested-user-name']", UserKeyOrDefault(user?.UserName) },
                { "//span[@name='begin-search-requested-email']", UserKeyOrDefault(user?.Email) },
                { "//td[@name='td-begin-search-requested-state-code']", request.State.ToUpper() },
                { "//td[@name='td-begin-search-requested-county-name']", request.County.Name.ToProperCase() },
                { "//td[@name='td-begin-search-requested-start-date']", request.Start.FromUnixTime() },
                { "//td[@name='td-begin-search-requested-end-date']", request.End.FromUnixTime() }
            };
            var keys = dictionary.Keys.ToList();
            keys.ForEach(key =>
            {
                var node = doc.DocumentNode.SelectSingleNode(key);
                if (node != null) node.InnerHtml = dictionary[key];
            });
            return doc.DocumentNode.OuterHtml;
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static string ToProperCase(this string text)
        {
            if (string.IsNullOrEmpty(text)) { return text; }
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static string FromUnixTime(this long id)
        {
            if (id == 0) { return " - "; }
            var dte = DateTimeOffset.FromUnixTimeMilliseconds(id).DateTime;
            return dte.ToString("MMM d, yyyy");
        }
        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static string UserKeyOrDefault(string? item)
        {
            if (string.IsNullOrEmpty(item)) return " - ";
            return item;
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static HtmlDocument? GetDocument(string html)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                return doc;
            } catch
            {
                return null;
            }
        }
    }
}
