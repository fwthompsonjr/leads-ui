using legallead.desktop.entities;
using System.Text;
using System;
using System.Windows.Controls;
using CefSharp.Wpf;
using CefSharp;

namespace legallead.desktop.utilities
{
    internal static class ContentHandler
    {
        public static void LoadLocal(string name, WebBrowser browser)
        {
            var content = GetLocalContent(name);
            if (content == null) return;
            browser.NavigateToString(content.Content);
        }

        public static void LoadLocal(string name, ContentControl browserContainer)
        {
            var content = GetLocalContent(name);
            if (content == null)
            {
                browserContainer.Content = "Error: Page load failure for page: {name}";
                return;
            }
            var browser = new ChromiumWebBrowser()
            {
                Address = GetAddressBase64(content)
            };
            browserContainer.Content = browser;
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
    }
}