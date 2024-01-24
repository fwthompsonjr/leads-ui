using legallead.ui.interfaces;

namespace legallead.ui.implementations
{
    internal class MyAccountMenuClicked : MyAccountClickBase, IMenuClickHandler
    {
        protected override string PageTarget => "myaccount";
    }
}