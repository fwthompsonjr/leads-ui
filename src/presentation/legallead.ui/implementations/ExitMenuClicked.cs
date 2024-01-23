using legallead.ui.interfaces;

namespace legallead.ui.implementations
{
    internal class ExitMenuClicked : IMenuClickHandler
    {
        public void Click()
        {
            Environment.Exit(0);
        }
    }
}
