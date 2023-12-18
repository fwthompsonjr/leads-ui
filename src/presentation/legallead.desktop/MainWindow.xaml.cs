using legallead.desktop.utilities;
using System.Windows;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentHandler.LoadLocal("home", content1);
        }
    }
}