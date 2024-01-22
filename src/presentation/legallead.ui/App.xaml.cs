using legallead.desktop.utilities;

namespace legallead.ui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            AppBuilder.Build();
            MainPage = new AppShell();
        }
    }
}
