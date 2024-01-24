using legallead.desktop.utilities;
using legallead.ui.Models;
using legallead.ui.Utilities;
namespace legallead.ui
{
    public partial class MainPage : ContentPage
    {
        private readonly StatusBar statusBar;
        private readonly MainContentLoadHandler mainContentLoadHandler;
        private readonly MyAccountContentLoadHandler myAccountContentLoadHandler;
        private MenuBarItem[]? toolbars;
        public MainPage()
        {
            InitializeComponent();
            statusBar = new StatusBar(this);
            this.BindingContext = AppBuilder.ServiceProvider?.GetService<MainWindowViewModel>() ?? new();
            mainContentLoadHandler = new MainContentLoadHandler();
            myAccountContentLoadHandler = new MyAccountContentLoadHandler();
            mainWebViewer.Navigating += MainWebViewer_Navigating;
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object? sender, EventArgs e)
        {
            toolbars = [this.bndMenuMyAccount, this.bndMenuMySearch];
            InitializeContent();
        }

        private void InitializeContent()
        {
            mainContentLoadHandler.SetBlank();
        }

    }

}
