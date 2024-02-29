using CefSharp;
using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.extensions;
using legallead.desktop.utilities;
using Newtonsoft.Json;
using System;
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
            var uri = GetDisplayUri(detail);
            if (!string.IsNullOrEmpty(uri))
            {
                web?.Load(uri);
                return;
            }
            // this is a fallback method to display the invoice using local resources
            var content = ContentProvider.LocalContentProvider.GetContent("invoice")?.Content;
            var main = GetMain();
            if (main == null) return;
            content = TransformHtml(content, detail);
            web?.SetHTML(main.Dispatcher, content);
        }

        private static string TransformHtml(string? html, GenerateInvoiceResponse? response)
        {
            if (response == null || string.IsNullOrEmpty(html)) return string.Empty;
            var config = AppBuilder.PaymentSessionKey ?? string.Empty;
            return response.GetHtml(html, config);
        }

        private static MainWindow? GetMain()
        {
            var dispatcher = Application.Current.Dispatcher;
            Window mainWindow = dispatcher.Invoke(() => { return Application.Current.MainWindow; });
            if (mainWindow is not MainWindow main) return null;
            return main;
        }

        private static string GetDisplayUri(GenerateInvoiceResponse? response)
        {
            const char slash = '/';
            if (response == null) return string.Empty;
            if (string.IsNullOrEmpty(response.SuccessUrl)) return string.Empty;
            if (string.IsNullOrEmpty(response.ExternalId)) return string.Empty;
            _ = Uri.TryCreate(response.SuccessUrl, UriKind.RelativeOrAbsolute, out var uri);
            if (uri == null) return string.Empty;
            var landing = $"payment-checkout?id={response.ExternalId}";
            var path = uri.PathAndQuery;
            var address = uri.ToString().Replace(path, string.Empty);
            if (!address.EndsWith(slash)) address = string.Concat(address, slash);
            address = string.Concat(address, landing);
            return address;
        }

        private static readonly List<string> scripts = new()
        {
            "jssearchpager.bind_preview( '{0}' );"
        };
    }
}
