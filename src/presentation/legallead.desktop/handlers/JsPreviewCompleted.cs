using CefSharp;
using CefSharp.Wpf;
using System.Collections.Generic;

namespace legallead.desktop.handlers
{
    internal class JsPreviewCompleted
    {
        protected readonly ChromiumWebBrowser? web;
        private readonly string json;
        public JsPreviewCompleted(ChromiumWebBrowser? browser, string response)
        {
            web = browser;
            json = response;
        }
        public void Complete()
        {
            scripts.ForEach(s =>
            {
                web.ExecuteScriptAsync(string.Format(s, json));
            });
        }
        private static readonly List<string> scripts = new()
        {
            "jssearchpager.bind_preview( '{0}' );"
        };
    }
}
