using CefSharp.Wpf;
using legallead.desktop.entities;
using System.Windows.Threading;

namespace legallead.desktop.utilities
{
    internal static class WebBroswerExtensions
    {
        public static string GetHTML(this ChromiumWebBrowser web, Dispatcher dispatcher)
        {
            var html = dispatcher.Invoke(() => { return web.Address; });
            if (string.IsNullOrEmpty(html)) return string.Empty;
            return ContentHandler.DecodeFromBase64(html);
        }

        public static void SetHTML(this ChromiumWebBrowser web, Dispatcher dispatcher, string source)
        {
            var conversion = ContentHandler.GetAddressBase64(new ContentHtml { Content = source });
            dispatcher.Invoke(() =>
            {
                web.Address = conversion;
            });
        }
    }
}