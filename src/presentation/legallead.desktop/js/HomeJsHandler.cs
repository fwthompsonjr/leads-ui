using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.handlers;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;

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