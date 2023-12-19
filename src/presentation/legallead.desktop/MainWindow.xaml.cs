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
            var window = (Window)this;
            var dispatcher = Dispatcher;
            Helper = new BrowserHelper(window);
            Helper.Load("introduction", dispatcher, content1);
        }
    }
}