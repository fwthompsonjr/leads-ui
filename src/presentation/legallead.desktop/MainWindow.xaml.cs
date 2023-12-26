using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.js;
using legallead.desktop.models;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isBlankLoaded = false;
        private readonly MenuConfiguration? menuConfiguration;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBrowserContent();
            InitializeErrorContent();
            menuConfiguration = AppBuilder.ServiceProvider?.GetRequiredService<MenuConfiguration>();
            ContentRendered += MainWindow_ContentRendered;
            mnuExit.Click += MnuExit_Click;
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

        private void InitializeErrorContent()
        {
            SetErrorContent(404);
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

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MnuHome_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem _) return;
            tabHome.IsSelected = true;
        }

        private void MnuError_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem item) return;
            var menuId = Convert.ToInt32(item.Name.Replace("mnu", ""));
            if (menuId == 0) return;
            tabError.IsSelected = true;
            SetErrorContent(menuId);
        }

        private BrowserHelper GetHelper()
        {
            var window = (Window)this;
            return new BrowserHelper(window);
        }

        private void SetErrorContent(int errorCode)
        {
            var errorService = AppBuilder.ServiceProvider?.GetRequiredService<IErrorContentProvider>();
            var errorContent = errorService?.GetContent(errorCode);
            if (errorContent != null)
            {
                var blankHtml = ContentHandler.GetAddressBase64(errorContent);
                var browser = new ChromiumWebBrowser()
                {
                    Address = blankHtml
                };
                browser.JavascriptObjectRepository.Register("jsHandler", new JsHandler(browser));
                contentError.Content = browser;
            }
        }
    }
}