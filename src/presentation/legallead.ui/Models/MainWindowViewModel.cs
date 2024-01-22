using legallead.ui.implementations;
using System.Windows.Input;

namespace legallead.ui.Models
{
    internal class MainWindowViewModel
    {
        public ICommand OnItemClicked { get; set; } = new MenuItemClickedCommand();

        public bool IsMyAccountVisible { get; set; } = false;

        public bool IsMySearchVisible { get; set; } = false;

        public string ContentHtml { get; set; } = string.Empty;

    }
}
