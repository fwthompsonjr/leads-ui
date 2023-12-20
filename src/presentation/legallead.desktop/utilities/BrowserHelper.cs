using CefSharp.Wpf;
using legallead.desktop.handlers;
using legallead.desktop.models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace legallead.desktop.utilities
{
    internal class BrowserHelper
    {
        private readonly Window _window;

        public BrowserHelper(Window source)
        {
            _window = source;
        }

        public ChromiumWebBrowser? Browser { get; private set; }

        protected Dispatcher? GetDispatcher { get; set; }

        public void Load(string name, Dispatcher dispatcher, ContentControl browserContainer)
        {
            _window.Title = name.ToTitleCase();
            var response = ContentHandler.LoadLocal(name, dispatcher, browserContainer);
            Browser = response?.Browser;
            GetDispatcher = dispatcher;
            SetInitializationCompletedHandler(name, browserContainer, response);
        }

        private void SetInitializationCompletedHandler(string name, ContentControl browserContainer, ContentRegistrationResponse? response)
        {
            if (response?.Handler == null) return;
            var initHandler = GetInitializationHandler(name);
            if (initHandler == null) return;
            response.Handler.OnInitCompleted = GetHandler(initHandler, browserContainer);
        }

        private Action<object?>? GetHandler(JsCompletedHandler initHandler, ContentControl browserContainer)
        {
            if (GetDispatcher == null) return null;
            return (h) =>
            {
                initHandler.Complete(_window, GetDispatcher, browserContainer);
            };
        }

        private static JsCompletedHandler? GetInitializationHandler(string name)
        {
            if (name.Equals("introduction")) return new InitializationCompletedHandler();
            if (name.Equals("home")) return new JsHomeFormSubmittedHandler();
            return default;
        }
    }
}