using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.utilities;
using legallead.ui.interfaces;
using legallead.ui.Utilities;

namespace legallead.ui.implementations
{
    internal class PageNavigatorGetSession : IPageNavigator
    {
        public Task Submit(string url)
        {
            var user = AppBuilder.ServiceProvider?.GetService<UserBo>();
            var main = MainPageFinder.GetMain();
            if (user == null ||
                main == null)
            {
                SetSession(main, "-unset-");
                return Task.CompletedTask;
            }
            if (user.IsSessionTimeout())
            {
                UserAuthenicationHelper.LogoutRequested();
                return Task.CompletedTask;
            }
            SetSession(main, user.GetSessionId());
            return Task.CompletedTask;
        }

        private static void SetSession(MainPage? main, string sessionId)
        {
            var web = main?.WebViewer;
            var wcontent = main?.WebViewSource.Html;
            if (main == null || web == null || string.IsNullOrEmpty(wcontent)) { return; }
            var doc = new HtmlDocument();
            doc.LoadHtml(wcontent);
            var node = doc.DocumentNode.SelectSingleNode("//span[@id = 'spn-user-session-status']");
            if (node == null || node.Attributes["value"].Value == sessionId) { return; }
            node.Attributes["value"].Value = sessionId;
            main.WebViewSource.Html = doc.DocumentNode.OuterHtml;
            web.Reload();
        }
    }
}
