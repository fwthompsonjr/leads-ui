using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.utilities;
using System.Collections.ObjectModel;

namespace legallead.ui.Utilities
{
    internal static class ButtonClickWriter
    {
        static ButtonClickWriter()
        {
            InitializeCommon();
        }

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
                InjectButtonClickMethod(page, k, onclick, changescript, doc, parent);
            });
            var user = AppBuilder.ServiceProvider?.GetService<UserBo>();
            if (user != null)
            {
                AppendCommonDialogue(doc, parent);
            }
            return parent.OuterHtml;
        }

        private static void InitializeCommon()
        {
            var keys = CommonControls.Keys.ToList();
            keys.ForEach(k =>
            {
                var txt = CommonControls[k];
                if(string.IsNullOrEmpty(txt))
                {
                    var remote = ContentHandler.GetLocalContent(k)?.Content;
                    if(remote != null) { CommonControls[k] = remote; }
                }
            });
        }

        private static void InjectButtonClickMethod(string page, string k, string onclick, string changescript, HtmlDocument doc, HtmlNode parent)
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
        }

        private static void AppendCommonDialogue(HtmlDocument doc, HtmlNode parent)
        {
            const string find = "id=\"spn-user-session-status\"";
            var user = AppBuilder.ServiceProvider?.GetService<UserBo>();
            var dialogue = CommonControls["commondialogue"];
            var jscript = CommonControls["commondialoguescript"];
            if (user == null) { return; }
            var body = parent.SelectSingleNode("//body");
            var node_dialogue = doc.CreateElement("div");
            string replace = $"{find} value=\"{user.GetSessionId()}\" ";
            if (dialogue.Contains(find, StringComparison.CurrentCultureIgnoreCase))
            {
                dialogue = dialogue.Replace(find, replace);
            }
            node_dialogue.InnerHtml = dialogue;
            body.PrependChild(node_dialogue);
            var node_jscript = doc.CreateElement("script");
            var attr = doc.CreateAttribute("name", "common-dialogue");
            node_jscript.Attributes.Add(attr);
            node_jscript.InnerHtml = jscript;
            body.AppendChild(node_jscript);
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
        private static readonly Dictionary<string, string> CommonControls = new() {
            { "commondialogue", "" },
            { "commondialoguescript", ContentHandler.DialogueJs },
        };
    }
}
