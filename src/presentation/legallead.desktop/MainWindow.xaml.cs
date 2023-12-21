using legallead.desktop.utilities;
using System.Windows;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BrowserHelper? Helper { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            var helper = GetHelper();
            helper.Load("blank", Dispatcher, content1);
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var window = (Window)this;
            var dispatcher = Dispatcher;
            var initialPage = AppBuilder.InitialViewName ?? "introduction";
            Helper = new BrowserHelper(window);
            Helper.Load(initialPage, dispatcher, content1);
        }

        private BrowserHelper GetHelper()
        {
            var window = (Window)this;
            return new BrowserHelper(window);
        }
    }
}