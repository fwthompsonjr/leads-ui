using CefSharp.Wpf;
using legallead.desktop.js;
using legallead.desktop.utilities;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isBlankLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBrowserContent();
            ContentRendered += MainWindow_ContentRendered;
        }

        private void InitializeBrowserContent()
        {
            var blankContent = ContentHandler.GetLocalContent("blank");
            if (blankContent != null)
            {
                var blankHtml = ContentHandler.GetAddressBase64(blankContent);
                var browser = new ChromiumWebBrowser()
                {
                    Address = blankHtml
                };
                browser.JavascriptObjectRepository.Register("jsHandler", new JsHandler(browser));
                content1.Content = browser;
            }
        }

        private void MainWindow_ContentRendered(object? sender, System.EventArgs e)
        {
            if (isBlankLoaded) { return; }
            try
            {
                Task.Run(() =>
                {
                    Thread.Sleep(1500);
                    var initialPage = AppBuilder.InitialViewName ?? "introduction";
                    var helper = GetHelper();
                    helper.Load(initialPage, Dispatcher, content1);
                    helper.Handler?.OnPageLoaded();
                }).ConfigureAwait(false);
            }
            finally
            {
                isBlankLoaded = true;
            }
        }

        private BrowserHelper GetHelper()
        {
            var window = (Window)this;
            return new BrowserHelper(window);
        }
    }
}