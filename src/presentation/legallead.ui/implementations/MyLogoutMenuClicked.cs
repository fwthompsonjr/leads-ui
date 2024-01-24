using legallead.ui.interfaces;
using legallead.ui.Utilities;

namespace legallead.ui.implementations
{
    internal class MyLogoutMenuClicked : IMenuClickHandler
    {
        public void Click()
        {
            const string js = "showLogout()";
            var main = MainPageFinder.GetMain();
            if (main == null) return;
            main.Dispatcher.Dispatch(async () => {
                await main.WebViewer.EvaluateJavaScriptAsync(js);
            });
        }
    }
}