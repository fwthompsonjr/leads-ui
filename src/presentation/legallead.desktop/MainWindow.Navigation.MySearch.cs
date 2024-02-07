using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.js;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

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
            var provider = AppBuilder.ServiceProvider;
            if (provider == null) return;
            var user = provider.GetService<UserBo>();
            var api = provider.GetService<IPermissionApi>();
            var mapper = provider.GetService<IUserSearchMapper>();
            if (user == null || 
                !user.IsAuthenicated || 
                api == null ||
                mapper == null) return;
            await Task.Run(() => {
                _ = Dispatcher.Invoke(async () =>
                {
                    var container = contentMySearch.Content;
                    if (container is not ChromiumWebBrowser web) return string.Empty;
                    var html = web.GetHTML(Dispatcher);
                    html = await mapper.Map(api, user, html, "history");
                    web.SetHTML(Dispatcher, html);
                    return html;
                });
            });
        }
    }
}