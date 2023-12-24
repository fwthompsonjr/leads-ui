using CefSharp.Wpf;
using legallead.desktop.utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace legallead.desktop.handlers
{
    internal class BlankCompletedHandler : JsCompletedHandler
    {
        public BlankCompletedHandler(ChromiumWebBrowser? browser) : base(browser)
        {
        }

        public override void Complete(Window window, Dispatcher dispatcher, ContentControl control, string? customData = null)
        {
            var initialPage = AppBuilder.InitialViewName ?? "introduction";
            ContentHandler.LoadLocal(initialPage, dispatcher, control);
            dispatcher.Invoke(() => { window.Title = BrowserHelper.GetPageTitle(initialPage); });
        }

        public override void Submit(string formName, string json)
        {
        }
    }
}