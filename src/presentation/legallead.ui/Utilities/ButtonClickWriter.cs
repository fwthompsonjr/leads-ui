using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.utilities;

namespace legallead.ui.Utilities
{
    internal static class ButtonClickWriter
    {
        public static string ReWrite(string? page)
        {
            const string onclick = "onclick";
            const string changescript = "verifyAndPost('{0}', '{1}');";
            if (string.IsNullOrEmpty(page)) return string.Empty;
            page = page.ToLower().Trim();
            if (!PageButtons.TryGetValue(page, out string? value))
            {
                return string.Empty;
            }
            var content = ContentHandler.GetLocalContent(page)?.Content;
            if (string.IsNullOrEmpty(content)) return string.Empty;
            if (PageAlias.TryGetValue(page, out string? alias))
            {
                page = alias;
            }
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
            var user = AppBuilder.ServiceProvider?.GetService<UserBo>();
            var dialogue = ContentHandler.GetLocalContent("commondialogue")?.Content;
            var jscript = ContentHandler.GetLocalContent("commondialoguescript")?.Content;
            if (user != null && dialogue != null && jscript != null)
            {
                var body = parent.SelectSingleNode("//body");
                var node_dialogue = doc.CreateElement("div");
                node_dialogue.InnerHtml = dialogue;
                body.PrependChild(node_dialogue);
                var node_jscript = doc.CreateElement("script");
                var attr = doc.CreateAttribute("name", "comon-dialogue");
                node_jscript.Attributes.Add(attr);
                node_jscript.InnerHtml = jscript;
                body.AppendChild(node_jscript);
            }
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
            { "myaccount", "frm-change-password-submit-button,frm-permissions-submit-button,frm-profile-submit-button" },
            { "mysearch", "search-submit-button" },
            { "mysearchtemplate", "search-submit-button" }
        };
        private static readonly Dictionary<string, string> PageAlias = new()
        {
            { "mysearchtemplate", "mysearch" }
        };
    }
}
