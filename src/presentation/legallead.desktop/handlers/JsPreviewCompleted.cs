using CefSharp;
using CefSharp.Wpf;
using System.Collections.Generic;
using System.Threading;

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
                if (s.Contains('{'))
                {
                    web.ExecuteScriptAsync(string.Format(s, json));
                }   
                else
                {
                    Thread.Sleep(500);
                    web.ExecuteScriptAsync(s);
                }
            });
        }
        private static readonly List<string> scripts = new()
        {
            "jssearchpager.bind_preview( '{0}' );"
        };
    }
}
