using legallead.desktop;
using legallead.desktop.entities;
using legallead.desktop.utilities;
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

        public CommonMessage CommonStatus { get; set; } = GetStatus(1);

        private static CommonMessage GetStatus(int statusId)
        {
            var fallback = new CommonMessage { Color = "Black" };
            var isMapped = Enum.TryParse<CommonStatusTypes>(statusId.ToString(), out var statusType);
            if (!isMapped) return new CommonMessage { Color = "Gray", Id = statusId, Message = "Initializing application content", Name = "Initializing" };
            var messages = AppBuilder.ServiceProvider?.GetService<CommonMessageList>()?.Messages;
            if (messages == null) return fallback;

            var status = messages.Find(x => x.Id == statusId);
            if (status == null) return fallback;
            return status;
        }
    }
}
