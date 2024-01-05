using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.js;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private void InitializeMySearchContent()
        {
            var blankContent = ContentHandler.GetLocalContent("mysearch");
            if (blankContent != null)
            {
                var blankHtml = ContentHandler.GetAddressBase64(blankContent);
                var browser = new ChromiumWebBrowser()
                {
                    Address = blankHtml
                };
                browser.JavascriptObjectRepository.Register("jsHandler", new JsHandler(browser));
                contentMySearch.Content = browser;
            }
        }

        private void NavigateToMySearch()
        {
            var user = AppBuilder.ServiceProvider?.GetRequiredService<UserBo>();
            if (user == null || !user.IsAuthenicated)
            {
                Dispatcher.Invoke(() =>
                {
                    SetErrorContent(401);
                    tabError.IsSelected = true;
                });
                return;
            }
            Dispatcher.Invoke(() =>
            {
                InitializeMySearchContent();
                Task.Run(async () =>
                {
                    Thread.Sleep(500);
                    await MapMySearchDetails();
                });
                tabMySearch.IsSelected = true;
            });
        }

        private async Task MapMySearchDetails()
        {
            _ = Dispatcher.Invoke(() =>
            {
                var container = contentMySearch.Content;
                if (container is not ChromiumWebBrowser web) return string.Empty;
                return web.GetHTML(Dispatcher);
            });
        }
    }
}