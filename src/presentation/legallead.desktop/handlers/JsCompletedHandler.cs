using CefSharp.Wpf;
using legallead.desktop.utilities;
using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace legallead.desktop.handlers
{
    internal abstract class JsCompletedHandler
    {
        protected ChromiumWebBrowser? Web { get; private set; }

        protected JsCompletedHandler(ChromiumWebBrowser? browser)
        {
            Web = browser;
            AppBuilder.Build();
        }

        public abstract void Complete(Window window, Dispatcher dispatcher, ContentControl control, string? customData = null);

        public abstract void Submit(string formName, string json);

        protected static T? TryDeserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}