using legallead.ui.interfaces;
using legallead.ui.Utilities;

namespace legallead.ui.implementations
{
    internal class MyAccountProfileMenuClicked : IMenuClickHandler
    {
        public void Click()
        {
            var main = MainPageFinder.GetMain();
            if (main == null) return;
            var handler = main.MyAccountHandler;
            handler.SetHome();
            Task.Run(async () =>
            {
                Thread.Sleep(500);
                await main.WebViewer.EvaluateJavaScriptAsync("setDisplay('profile')");
            });
        }
    }
}