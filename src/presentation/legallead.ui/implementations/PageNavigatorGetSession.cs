using legallead.desktop.entities;
using legallead.desktop.utilities;
using legallead.ui.interfaces;
using legallead.ui.Utilities;
using System.Text;

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
            if (user.IsAuthenicated)
            {
                SetSession(main, user.SessionId);
            }
            return Task.CompletedTask;
        }

        private static void SetSession(MainPage? main, string sessionId)
        {
            var web = main?.WebViewer;
            if (main == null || web == null) { return; }
            var js = new StringBuilder("let sessioninput = document.getElementById('spn-user-session-status');");
            js.AppendLine();
            js.AppendLine("if (undefined == sessioninput || null == sessioninput) return;");
            js.AppendLine($"sessioninput.setAttribute( 'value', '{sessionId}' );");
            var script = js.ToString();
            main.Dispatcher.Dispatch(async () =>
            {
                _ = await web.EvaluateJavaScriptAsync(script);

            });
        }
    }
}
