using CefSharp.Wpf;
using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.implementations;
using legallead.desktop.interfaces;
using legallead.desktop.js;
using legallead.desktop.models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace legallead.desktop.utilities
{
    internal static class ContentHandler
    {
        public static ContentRegistrationResponse? LoadLocal(string name, Dispatcher dispatcher, ContentControl browserContainer)
        {
            var content = GetLocalContent(name);
            if (content == null)
            {
                browserContainer.Content = "Error: Page load failure for page: {name}";
                return null;
            }
            var response = dispatcher.Invoke(() =>
            {
                var browser = new ChromiumWebBrowser()
                {
                    Address = GetAddressBase64(content)
                };
                var jsHandler = GetJsHandler(name, browser);
                browser.JavascriptObjectRepository.Register("jsHandler", jsHandler);
                browserContainer.Content = browser;
                return new ContentRegistrationResponse { Browser = browser, Handler = jsHandler };
            });
            return response;
        }

        internal static ContentHtml? GetLocalContent(string name)
        {
            var provider = AppBuilder.ServiceProvider;
            var contentProvider = ContentProvider.LocalContentProvider;
            if (contentProvider.SearchUi == null)
            {
                var searchUI = provider?.GetService<ISearchBuilder>();
                if (searchUI != null)
                {
                    contentProvider.SearchUi = searchUI;
                }
            }
            var raw = contentProvider.GetContent(name);
            if (raw == null) return null;
            var beutifier = provider?.GetRequiredService<IContentParser>();
            if (beutifier == null) return raw;
            raw.Content = beutifier.BeautfyHTML(raw.Content);
            return raw;
        }

        internal static string DecodeFromBase64(string content)
        {
            const string bs64address = "data:text/html;base64,";
            if (!content.StartsWith(bs64address)) return content;
            content = content.Replace(bs64address, string.Empty);
            return Encoding.UTF8.GetString(Convert.FromBase64String(content));
        }

        internal static string GetAddressBase64(ContentHtml content)
        {
            const string bs64address = "data:text/html;base64,{0}";
            if (System.Diagnostics.Debugger.IsAttached)
            {
                content = InjectUser(content);
            }
            var html = parser.BeautfyHTML(content.Content);
            var base64EncodedHtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(html));
            return string.Format(bs64address, base64EncodedHtml);
        }

        private static JsHandler GetJsHandler(string name, ChromiumWebBrowser? browser)
        {
            if (!KnownHandlers.Exists(n => n.Equals(name, StringComparison.OrdinalIgnoreCase))) return new JsHandler(browser);
            if (name.Equals("blank")) return new BlankJsHandler(browser);
            if (name.Equals("introduction")) return new IntroductionJsHandler(browser);
            if (name.Equals("home")) return new HomeJsHandler(browser);
            return new JsHandler(browser);
        }

        private static readonly List<string> KnownHandlers = new()
        {
            "blank",
            "introduction",
            "home"
        };

        private static ContentHtml InjectUser(ContentHtml content)
        {
            AppBuilder.Build();
            var config = AppBuilder.Configuration;
            if (config == null) return content;
            var values = new[] {
                config["debug.user:name"],
                config["debug.user:code"] }.ToList();
            if (values.Exists(a => string.IsNullOrWhiteSpace(a))) { return content; }
            var document = new HtmlDocument();
            document.LoadHtml(content.Content);
            var parent = document.DocumentNode;
            var searches = new[] { "//*[@id=\"username\"]", "//*[@id=\"login-password\"]" };
            for (var i = 0; i < searches.Length; i++)
            {
                var element = parent.SelectSingleNode(searches[i]);
                if (element == null) continue;
                element.SetAttributeValue("value", values[i]);
            }
            content.Content = parent.OuterHtml;
            return content;
        }

        private static readonly ContentParser parser = new();
    }
}