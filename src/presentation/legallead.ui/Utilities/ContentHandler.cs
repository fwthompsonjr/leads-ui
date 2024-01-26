
using legallead.desktop.entities;
using legallead.desktop.implementations;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;

namespace legallead.ui.Utilities
{
    internal static class ContentHandler
    {
        internal static string DialogueJs => ContentHtmlNames.CommonDialogueJs;

        internal static void InitializeSearch()
        {
            var provider = AppBuilder.ServiceProvider;
            var contentProvider = ContentProvider.LocalContentProvider;

            if (contentProvider.SearchUi == null)
            {
                var searchUI = provider?.GetService<ISearchBuilder>();
                if (searchUI != null)
                {
                    contentProvider.SearchUi = searchUI;
                }
            }
        }
        internal static ContentHtml? GetLocalContent(string name)
        {
            var provider = AppBuilder.ServiceProvider;
            var contentProvider = ContentProvider.LocalContentProvider;
            var raw = contentProvider.GetContent(name);
            if (raw == null) return null;
            var beutifier = provider?.GetRequiredService<IContentParser>();
            if (beutifier == null) return raw;
            raw.Content = beutifier.BeautfyHTML(raw.Content);
            return raw;
        }
    }
}
