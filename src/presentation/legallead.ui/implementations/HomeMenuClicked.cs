using legallead.ui.interfaces;

namespace legallead.ui.implementations
{
    internal class HomeMenuClicked : IMenuClickHandler
    {
        public void Click()
        {
            var page = Application.Current?.MainPage;
            if (page is not AppShell app) return;
            if (app.CurrentPage is not MainPage main) return;
            var handler = main.HomeHandler;
            handler.SetHome();
        }
    }
}