using HtmlAgilityPack;

namespace legallead.ui.Utilities
{
    internal static class ButtonClickWriter
    {
        public static string ReWrite(string? page)
        {
            const string onclick = "onclick";
            const string changescript = "window.location.href = 'http://internal.legalead.com/{0}/{1}';";
            if (string.IsNullOrEmpty(page)) return string.Empty;
            page = page.ToLower().Trim();
            if (!PageButtons.TryGetValue(page, out string? value))
            {
                return string.Empty;
            }
            var content = ContentHandler.GetLocalContent(page)?.Content;
            if (string.IsNullOrEmpty(content)) return string.Empty;
            var doc = GetDocument(content);
            var keys = value.Split(',').ToList();
            var parent = doc.DocumentNode;
            keys.ForEach(k =>
            {
                var finder = $"//*[@id ='{k}']";
                var node = parent.SelectSingleNode(finder);
                if (node != null)
                {
                    var change = string.Format(changescript, page, k);
                    var attr = node.Attributes.ToList().Find(a => a.Name == onclick);
                    if (attr != null)
                    {
                        attr.Value = change;
                    }
                    else
                    {
                        attr = doc.CreateAttribute(onclick);
                        attr.Value = change;
                        node.Attributes.Add(attr);
                    }
                }
            });
            return parent.OuterHtml;
        }


        private static HtmlDocument GetDocument(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
        private static readonly Dictionary<string, string> PageButtons = new()
        {
            { "home", "form-login-submit,form-register-submit" },
            { "myaccount", "frm-change-password-submit-button,frm-permissions-submit-button,frm-profile-submit-button" }
        };
    }
}
