using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace legallead.desktop.handlers
{
    internal class InitializationCompletedHandler : JsCompletedHandler
    {
        public InitializationCompletedHandler(ChromiumWebBrowser? browser) : base(browser)
        {
        }

        public override void Complete(Window window, Dispatcher dispatcher, ContentControl control, string? customData = null)
        {
            var provider = AppBuilder.ServiceProvider;
            if (provider == null) return;
            GetStatusHelper()?.SetVersion();
            var user = provider.GetRequiredService<UserBo>();
            if (user == null || !user.IsInitialized)
            {
                // load an error page, unable to communicate remote
                GetStatusHelper()?.SetStatus(CommonStatusTypes.Error);
                SetErrorContent(500, dispatcher, control);
                return;
            }

            const string target = "home";
            GetStatusHelper()?.SetStatus(CommonStatusTypes.Ready);
            ContentHandler.LoadLocal(target, dispatcher, control);
            dispatcher.Invoke(() => { window.Title = BrowserHelper.GetPageTitle(target); });
        }

        public override void Submit(string formName, string json)
        {
        }


        private static void SetErrorContent(int errorCode, Dispatcher dispatcher, ContentControl control)
        {
            var errorService = AppBuilder.ServiceProvider?.GetRequiredService<IErrorContentProvider>();
            if (errorService == null) return;
            var errorContent = errorService.GetContent(errorCode);
            errorContent ??= errorService.GetContent(500);
            if (errorContent != null)
            {
                dispatcher.Invoke(() =>
                {
                    var blankHtml = ContentHandler.GetAddressBase64(errorContent);
                    var browser = new ChromiumWebBrowser()
                    {
                        Address = blankHtml
                    };
                    control.Content = browser;
                });
            }
        }

        private static CommonStatusHelper? GetStatusHelper()
        {
            var tmp = AppBuilder.ServiceProvider?.GetService(typeof(CommonStatusHelper));
            if (tmp is CommonStatusHelper helper) return helper;
            return null;
        }
    }
}