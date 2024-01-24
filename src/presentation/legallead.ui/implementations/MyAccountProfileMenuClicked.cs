using legallead.ui.interfaces;

namespace legallead.ui.implementations
{
    internal class MyAccountProfileMenuClicked : MyAccountPermissionsMenuClicked, IMenuClickHandler
    {
        protected override int PageId => 1;
    }
}