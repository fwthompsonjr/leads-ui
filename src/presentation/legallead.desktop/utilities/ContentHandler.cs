using CefSharp;
using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.handlers;
using legallead.desktop.interfaces;
using legallead.desktop.js;
using legallead.desktop.models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
            var contentProvider = ContentProvider.LocalContentProvider;
            var raw = contentProvider.GetContent(name);
            if (raw == null) return null;
            var provider = AppBuilder.ServiceProvider;
            var beutifier = provider?.GetRequiredService<IContentParser>();
            if (beutifier == null) return raw;
            raw.Content = beutifier.BeautfyHTML(raw.Content);
            return raw;
        }

        internal static string GetAddressBase64(ContentHtml content)
        {
            const string bs64address = "data:text/html;base64,{0}";
            var base64EncodedHtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(content.Content));
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
    }
}