using CefSharp.Wpf;
using legallead.desktop.js;

namespace legallead.desktop.models
{
    internal class ContentRegistrationResponse
    {
        public ChromiumWebBrowser? Browser { get; set; }

        public JsHandler? Handler { get; set; }
    }
}