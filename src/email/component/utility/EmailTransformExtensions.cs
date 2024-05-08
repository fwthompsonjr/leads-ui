using AngleSharp.Dom;
using HtmlAgilityPack;
using legallead.email.models;
using System.Diagnostics.CodeAnalysis;

namespace legallead.email.utility
{
    public static class EmailTransformExtensions
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
            return Substitute(doc, dictionary);
        }
        public static string ToHtml(this UserLockedModel search,
            UserAccountByEmailBo? user,
            string html)
        {
            if (string.IsNullOrEmpty(search.Email)) { return html; }
            var doc = GetDocument(html);
            if (doc == null) { return html; }
            var dictionary = new Dictionary<string, string>()
            {
                { "//span[@name='locked-account-user-name']", UserKeyOrDefault(user?.UserName) },
                { "//span[@name='locked-account-email']", UserKeyOrDefault(user?.Email) },
            };
            return Substitute(doc, dictionary);
        }

        public static string ToHtml(this ProfileChangedModel search,
            UserAccountByEmailBo? user,
            string html)
        {
            if (string.IsNullOrEmpty(search.Email)) { return html; }
            var doc = GetDocument(html);
            if (doc == null) { return html; }
            var dictionary = new Dictionary<string, string>()
            {
                { "//span[@name='profile-change-user-name']", UserKeyOrDefault(user?.UserName) },
                { "//span[@name='profile-change-email']", UserKeyOrDefault(user?.Email) },
                { "//span[@name='profile-change-request-name']", search.Name },
                { "//td[@name='profile-change-detail-message-3']", GetChangeItems(search) },
            };
            return Substitute(doc, dictionary);
        }
        public static string ToHtml(this PermissionChangeResponse search,
            UserAccountByEmailBo? user,
            string html)
        {
            if (string.IsNullOrEmpty(search.Email)) { return html; }
            var doc = GetDocument(html);
            if (doc == null) { return html; }
            var dictionary = new Dictionary<string, string>()
            {
                { "//span[@name='permission-request-user-name']", UserKeyOrDefault(user?.UserName) },
                { "//span[@name='permission-request-email']", UserKeyOrDefault(user?.Email) },
                { "//span[@name='permission-request-request-type']", search.Name },
                { "//td[@name='permission-request-detail-message-3']", GetPermissionItems(search) },
            };
            return Substitute(doc, dictionary);
        }

        private static string GetPermissionItems(PermissionChangeResponse search)
        {
            const string item = "<span style='color:blue;'>{0} : </span><span style='color: gray'>{1}</span><br/>";
            if (string.IsNullOrEmpty(search.Name)) { return dash; }
            if (search.Name == "Discount" && search.DiscountRequest != null)
            {
                var collection = search.DiscountRequest.Choices.Select(s =>
                {
                    var location = $"{s.CountyName}, {s.StateName}";
                    var activate = s.IsSelected ? "ACTIVE" : "DISABLED";
                    return string.Format(item, location, activate);
                });
                return string.Join(Environment.NewLine, collection);
            }
            if (search.Name == "PermissionLevel" && search.LevelRequest != null)
            {
                return string.Format(item, "Change Permission Level", search.LevelRequest.Level);
            }
            return dash;
        }

        private static string Substitute(HtmlDocument doc, Dictionary<string, string> dictionary)
        {
            var keys = dictionary.Keys.ToList();
            keys.ForEach(key =>
            {
                var node = doc.DocumentNode.SelectSingleNode(key);
                if (node != null) node.InnerHtml = dictionary[key];
            });
            return doc.DocumentNode.OuterHtml;
        }


        private static string GetChangeItems(ProfileChangedModel search)
        {
            if (search.ChangeItems.Count == 0) return dash;
            const string item = "<span style='color:blue;'>{0} : </span><span>{1}</span><br/>";
            var items = search.ChangeItems.Select(s =>
            {
                var line = string.Format(item, s.FieldName, s.Description); return line;
            });
            return string.Join(Environment.NewLine, items);
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
            if (id == 0) { return dash; }
            var dte = DateTimeOffset.FromUnixTimeMilliseconds(id).DateTime;
            return dte.ToString("MMM d, yyyy");
        }
        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static string UserKeyOrDefault(string? item)
        {
            if (string.IsNullOrEmpty(item)) return dash;
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
            }
            catch
            {
                return null;
            }
        }
        private const string dash = " - ";
    }
}
