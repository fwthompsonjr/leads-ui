using CefSharp;
using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.extensions;
using legallead.desktop.utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

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

        public void Invoice()
        {
            var detail = JsonConvert.DeserializeObject<GenerateInvoiceResponse>(json);
            var content = ContentProvider.LocalContentProvider.GetContent("invoice")?.Content;
            var main = GetMain();
            if (main == null) return;
            content = TransformHtml(content, detail);
            web?.SetHTML(main.Dispatcher, content);
        }

        private static string TransformHtml(string? html, GenerateInvoiceResponse? response)
        {
            if (response == null || string.IsNullOrEmpty(html)) return string.Empty;
            var config = AppBuilder.Configuration?.GetValue<string>("stripe:key") ?? string.Empty;
            return response.GetHtml(html, config);
        }

        private static MainWindow? GetMain()
        {
            var dispatcher = Application.Current.Dispatcher;
            Window mainWindow = dispatcher.Invoke(() => { return Application.Current.MainWindow; });
            if (mainWindow is not MainWindow main) return null;
            return main;
        }

        private static readonly List<string> scripts = new()
        {
            "jssearchpager.bind_preview( '{0}' );"
        };
    }
}
