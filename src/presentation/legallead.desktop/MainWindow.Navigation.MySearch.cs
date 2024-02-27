using CefSharp.Wpf;
using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.js;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
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

        internal void NavigateToMySearch()
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
                    Thread.Sleep(250);
                    await MapMySearchDetails();
                    Thread.Sleep(250);
                    await MapMySearchPurchases();
                });
                tabMySearch.IsSelected = true;
            });
        }
        internal void NavigateToMyActiveSearches()
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
                var content = RemoveJson(ContentHandler.GetLocalContent("mysearchactive"));
                var api = AppBuilder.ServiceProvider?.GetService<IPermissionApi>();
                var browser = contentMySearch.Content;
                if (browser is not ChromiumWebBrowser web) { return; }
                Task.Run(async () =>
                {
                    var text = await InjectJson(api, content, user);
                    web.SetHTML(Dispatcher, text);
                    tabMySearch.IsSelected = true;
                });
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
            await Task.Run(() =>
            {
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

        private async Task MapMySearchPurchases()
        {
            var provider = AppBuilder.ServiceProvider;
            if (provider == null) return;
            var user = provider.GetService<UserBo>();
            var api = provider.GetService<IPermissionApi>();
            if (user == null ||
                !user.IsAuthenicated ||
                api == null) return;
            await Task.Run(() =>
            {
                _ = Dispatcher.Invoke(() =>
                {
                    var container = contentMySearch.Content;
                    if (container is not ChromiumWebBrowser web) return string.Empty;
                    var html = web.GetHTML(Dispatcher);
                    web.SetHTML(Dispatcher, html);
                    return html;
                });
            });

        }

        private static string RemoveJson(ContentHtml? html)
        {
            if (html == null || string.IsNullOrEmpty(html.Content)) return string.Empty;
            var doc = new HtmlDocument();
            doc.LoadHtml(html.Content);
            var node = doc.DocumentNode.SelectSingleNode("//*[@id='text-my-active-searches-js']");
            if (node != null) node.InnerHtml = string.Empty;
            return doc.DocumentNode.OuterHtml;
        }
        private static async Task<string> InjectJson(IPermissionApi? api, string html, UserBo user)
        {
            if (api == null) return html;
            var payload = new { id = Guid.NewGuid().ToString(), name = "legallead.permissions.api" };
            var appresponse = await api.Post("search-get-actives", payload, user);
            if (appresponse == null || appresponse.StatusCode != 200) return html;
            var message = appresponse.Message;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.SelectSingleNode("//*[@id='text-my-active-searches-js']");
            if (node != null) node.InnerHtml = message;
            return doc.DocumentNode.OuterHtml;
        }
    }
}