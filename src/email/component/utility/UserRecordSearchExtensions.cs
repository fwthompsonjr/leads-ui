using HtmlAgilityPack;
using legallead.email.models;
using System.Diagnostics.CodeAnalysis;

namespace legallead.email.utility
{
    public static class UserRecordSearchExtensions
    {
        public static string ToHtml(this UserRecordSearch search, string html)
        {
            const string detailTable = "//table[@name='table-begin-search-requested-detail-item-parameters']";
            var doc = GetDocument(html);
            if (doc == null) { return html; }
            var request = search.Search;
            ConvertFrom(request.Details, doc, detailTable);
            var dictionary = new Dictionary<string, string>()
            {
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

        private static void ConvertFrom(List<SearchRequestItem> details, HtmlDocument doc, string detailTable)
        {
            if (details.Count == 0) { return; }
            var table = doc.DocumentNode.SelectSingleNode(detailTable);
            if (table == null) { return; }
            details.ForEach(d =>
            {
                var tr = doc.CreateElement("tr");
                var td1 = doc.CreateElement("td");
                var td2 = doc.CreateElement("td");
                td1.InnerHtml = d.Name;
                td2.InnerHtml = d.Text;
                tr.AppendChild(td1);
                tr.AppendChild(td2);
                table.AppendChild(tr);
            });
        }

        private static string ToProperCase(this string text)
        {
            if (string.IsNullOrEmpty(text)) { return text; }
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        private static string FromUnixTime(this long id)
        {
            if (id == 0) { return " - "; }
            var dte = DateTimeOffset.FromUnixTimeMilliseconds(id).DateTime;
            return dte.ToString("MMM d, yyyy");
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
