using legallead.ui.interfaces;
using legallead.ui.Utilities;

namespace legallead.ui.implementations
{
    internal class MyAccountMenuClicked : IMenuClickHandler
    {
        public void Click()
        {
            var main = MainPageFinder.GetMain();
            if (main == null) return;
            var handler = main.MyAccountHandler;
            handler.SetHome();
        }
    }
}