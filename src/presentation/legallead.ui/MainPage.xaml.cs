using legallead.desktop.utilities;
using legallead.ui.Models;
using legallead.ui.Utilities;

namespace legallead.ui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = AppBuilder.ServiceProvider?.GetService<MainWindowViewModel>() ?? new();
            InitializeContent();
        }

        private void InitializeContent()
        {
            var bindingobj = AppBuilder.ServiceProvider?.GetService<MainWindowViewModel>() ?? new();
            this.BindingContext = bindingobj;
            var blank = ContentHandler.GetLocalContent("blank")?.Content;
            if (string.IsNullOrEmpty(blank)) return;
            this.Dispatcher.Dispatch(() =>
            {
                this.mainWebContent.Html = blank;
            });
        }
    }

}
