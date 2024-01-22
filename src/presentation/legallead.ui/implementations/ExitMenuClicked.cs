using legallead.ui.interfaces;

namespace legallead.ui.implementations
{
    internal class ExitMenuClicked : IMenuClickHandler
    {
        public void Click(object? sender, EventArgs? e)
        {
            Environment.Exit(0);
        }
    }
}
