using legallead.desktop.utilities;
using legallead.ui.Models;

namespace legallead.ui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = AppBuilder.ServiceProvider?.GetService<MainWindowViewModel>() ?? new();
        }
    }

}
