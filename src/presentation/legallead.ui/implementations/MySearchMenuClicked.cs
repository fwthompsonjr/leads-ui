using legallead.ui.interfaces;

namespace legallead.ui.implementations
{
    internal class MySearchMenuClicked : MyAccountClickBase, IMenuClickHandler
    {
        protected override string PageTarget => "mysearch";
    }
}