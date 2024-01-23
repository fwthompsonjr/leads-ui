using legallead.ui.interfaces;
using legallead.ui.Utilities;

namespace legallead.ui.implementations
{
    internal class HomeMenuClicked : IMenuClickHandler
    {
        public void Click()
        {
            var main = MainPageFinder.GetMain();
            if (main == null) return;
            var handler = main.HomeHandler;
            handler.SetHome();
        }
    }
}