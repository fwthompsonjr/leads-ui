using legallead.desktop.utilities;
using legallead.ui.Models;
using legallead.ui.Utilities;
namespace legallead.ui
{
    public partial class MainPage : ContentPage
    {
        private readonly StatusBar statusBar;
        private readonly MainContentLoadHandler mainContentLoadHandler;
        public MainPage()
        {
            InitializeComponent();
            statusBar = new StatusBar(this);
            this.BindingContext = AppBuilder.ServiceProvider?.GetService<MainWindowViewModel>() ?? new();
            mainContentLoadHandler = new MainContentLoadHandler(this);
            mainWebViewer.Navigating += MainWebViewer_Navigating;
            InitializeContent();
        }


        private void InitializeContent()
        {
            mainContentLoadHandler.SetBlank();
        }

    }

}
