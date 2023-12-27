using CefSharp.Wpf;
using legallead.desktop.handlers;

namespace legallead.desktop.js
{
    internal class HomeJsHandler : JsHandler
    {
        public override void Submit(string formName, string json)
        {
            var handler = new JsHomeFormSubmittedHandler(web);
            handler.Submit(formName, json);
        }

        public HomeJsHandler(ChromiumWebBrowser? browser) : base(browser)
        {
        }
    }
}