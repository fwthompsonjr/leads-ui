using CefSharp.Wpf;
using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.js;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private void InitializeMyHistoryContent()
        {
            const string nohistory = "//*[@id='dv-history-item-no-history']";
            const string itemlist = "//*[@id='dv-history-item-list']";
            const string itemview = "//*[@id='dv-history-item-preview']";
            const string filterstatus = "//*[@id='cbo-search-history-filter']";
            const string filtercounty = "//*[@id='cbo-search-history-county']";
            const string restrictionstatus = "//*[@id='user-restriction-status']";
            const string countattribute = "data-item-count";
            var blankContent = ContentHandler.GetLocalContent("viewhistory");
            if (blankContent != null)
            {
                var historyService = AppBuilder.ServiceProvider?.GetService<IHistoryPersistence>();
                var data = historyService?.Fetch();
                var restriction = historyService?.Restriction();
                var mapper = AppBuilder.ServiceProvider?.GetService<IUserSearchMapper>();
                if (mapper != null && !string.IsNullOrEmpty(data))
                {
                    var content = blankContent.Content;
                    var table = mapper.Map(historyService, data, out var rows);
                    var doc = new HtmlDocument();
                    doc.LoadHtml(content);
                    AppendTable(doc, itemlist, table);
                    ApplyFilter(filterstatus, filtercounty, historyService, mapper, doc);
                    AppendRestriction(doc, restriction, restrictionstatus);
                    ToggleVisibility(nohistory, itemlist, itemview, countattribute, rows, doc);
                    blankContent.Content = doc.DocumentNode.OuterHtml;
                }
                var blankHtml = ContentHandler.GetAddressBase64(blankContent);
                var browser = new ChromiumWebBrowser()
                {
                    Address = blankHtml
                };
                browser.JavascriptObjectRepository.Register("jsHandler", new ViewHistoryJsHandler(browser));
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
            var content = Dispatcher.Invoke(() =>
            {
                var content = RemoveJson(ContentHandler.GetLocalContent("mysearchactive"));
                var browser = contentMySearch.Content;
                if (browser is not ChromiumWebBrowser web) { return content; }
                tabMySearch.IsSelected = true;
                web.SetHTML(Dispatcher, content);
                Thread.Sleep(500);
                return content;

            });

        }


        internal void NavigateToMyHistorySearches()
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
                InitializeMyHistoryContent();
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
            await Task.Run(() =>
            {
                _ = Dispatcher.Invoke(async () =>
                {
                    var container = contentMySearch.Content;
                    if (container is not ChromiumWebBrowser web) return string.Empty;
                    var html = web.GetHTML(Dispatcher);
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
                _ = Dispatcher.Invoke(async () =>
                {
                    var container = contentMySearch.Content;
                    if (container is not ChromiumWebBrowser web) return string.Empty;
                    var html = web.GetHTML(Dispatcher);
                    html = await InjectRestrictionAlert(html);
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

        private static async Task<string> InjectRestrictionAlert(string html)
        {
            var provider = AppBuilder.ServiceProvider;
            if (provider == null) return html;
            var user = provider.GetService<UserBo>();
            var api = provider.GetService<IPermissionApi>();
            var mapper = provider.GetService<IUserRestrictionMapper>();
            if (user == null ||
                !user.IsAuthenicated ||
                api == null ||
                mapper == null) return html;
            html = await mapper.Map(api, user, html);
            return html;
        }

        private static void AppendTable(HtmlDocument doc, string itemlist, string table)
        {
            var element = doc.DocumentNode.SelectSingleNode(itemlist);
            if (element != null) { element.InnerHtml = table; }
        }

        private static void ApplyFilter(string filterstatus, string filtercounty, IHistoryPersistence? historyService, IUserSearchMapper? mapper, HtmlDocument doc)
        {
            const string finder = "//*[@id='search-history-heading-caption']";
            var filter = historyService?.Filter() ?? string.Empty;
            var current = ObjectExtensions.TryGet<UserSearchFilterBo>(filter);
            var caption = current.GetCaption();
            var subheading = doc.DocumentNode.SelectSingleNode(finder);
            subheading.InnerHtml = caption;
            var cbo = doc.DocumentNode.SelectSingleNode(filterstatus);
            mapper?.SetFilter(historyService, cbo);
            cbo = doc.DocumentNode.SelectSingleNode(filtercounty);
            mapper?.SetCounty(historyService, cbo);
        }

        private static void AppendRestriction(HtmlDocument doc, string? restriction, string restrictionstatus)
        {
            if (string.IsNullOrEmpty(restriction)) return;
            var obj = ObjectExtensions.TryGet<MySearchRestrictions>(restriction);
            var isrestricted = obj.IsLocked.GetValueOrDefault(true) ? "true" : "false";
            var node = doc.DocumentNode.SelectSingleNode(restrictionstatus);
            if (node != null) { node.Attributes["value"].Value = isrestricted; }
        }

        private static void ToggleVisibility(
            string nohistory,
            string itemlist,
            string itemview,
            string countattribute,
            int rows,
            HtmlDocument doc)
        {
            HtmlNode? element;

            const string previewtable = "//*[@automationid='search-preview-table']";
            var tablestyle = rows == 0 ? "display: none" : "width: 95%";
            var dvs = new List<string> { itemlist, itemview, nohistory };
            var display = rows == 0 ? "0" : "1";
            dvs.ForEach(d =>
            {
                element = doc.DocumentNode.SelectSingleNode(d);
                if (element != null) element.Attributes[countattribute].Value = display;
            });
            element = doc.DocumentNode.SelectSingleNode(previewtable);
            if (element == null) return;
            element.Attributes["style"].Value = tablestyle;
        }
    }
}