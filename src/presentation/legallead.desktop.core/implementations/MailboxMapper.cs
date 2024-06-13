using legallead.desktop.interfaces;
using legallead.desktop.models;

namespace legallead.desktop.implementations
{
    internal class MailboxMapper : UserProfileMapper, IUserMailboxMapper
    {
        public string Substitute(IMailPersistence? persistence, string source)
        {
            var document = GetDocument(source);
            if (document == null) return source;
            var data = persistence?.Fetch() ?? string.Empty;
            var content = GetFirstPage(persistence, data);
            var replacements = new[]
            {
                new { find = "//*[@id=\"tarea-json\"]", replace = data},
                new { find = "//*[@id=\"tarea-current-html\"]", replace = content},
            };
            foreach (var item in replacements)
            {
                if (!IsValidXPath(item.find)) continue;
                var element = document.DocumentNode.SelectSingleNode(item.find);
                if (element == null) continue;
                element.InnerHtml = item.replace;
            }
            return document.DocumentNode.OuterHtml;
        }
        private static string GetFirstPage(IMailPersistence? persistence, string messages)
        {
            if (persistence == null || string.IsNullOrEmpty(messages)) return string.Empty;
            var list = ObjectExtensions.TryGet<List<MailStorageItem>>(messages);
            if (list.Count == 0) return string.Empty;
            var id = list[0].Id ?? string.Empty;
            return persistence.Fetch(id) ?? string.Empty;
        }
    }
}
