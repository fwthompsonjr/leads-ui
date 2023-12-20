using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.handlers;
using legallead.desktop.js;
using legallead.desktop.models;
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
                var jsHandler = GetJsHandler(name);
                browser.JavascriptObjectRepository.Register("jsHandler", jsHandler);
                browserContainer.Content = browser;
                return new ContentRegistrationResponse { Browser = browser, Handler = jsHandler };
            });
            return response;
        }

        private static ContentHtml? GetLocalContent(string name)
        {
            var contentProvider = ContentProvider.LocalContentProvider;
            return contentProvider.GetContent(name);
        }

        private static string GetAddressBase64(ContentHtml content)
        {
            const string bs64address = "data:text/html;base64,{0}";
            var base64EncodedHtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(content.Content));
            return string.Format(bs64address, base64EncodedHtml);
        }

        private static JsHandler GetJsHandler(string name)
        {
            if (!KnownHandlers.Exists(n => n.Equals(name, StringComparison.OrdinalIgnoreCase))) return new JsHandler();
            if (name.Equals("introduction")) return new IntroductionJsHandler();
            if (name.Equals("home")) return new HomeJsHandler();
            return new JsHandler();
        }

        private static readonly List<string> KnownHandlers = new()
        {
            "introduction",
            "home"
        };
    }
}