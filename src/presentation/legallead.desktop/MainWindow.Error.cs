using CefSharp.Wpf;
using legallead.desktop.interfaces;
using legallead.desktop.js;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private void InitializeErrorContent()
        {
            SetErrorContent(404);
        }

        private void SetErrorContent(int errorCode)
        {
            var errorService = AppBuilder.ServiceProvider?.GetRequiredService<IErrorContentProvider>();
            if (errorService == null) return;
            var errorContent = errorService.GetContent(errorCode);
            errorContent ??= errorService.GetContent(500);
            if (errorContent != null)
            {
                var blankHtml = ContentHandler.GetAddressBase64(errorContent);
                var browser = new ChromiumWebBrowser()
                {
                    Address = blankHtml
                };
                browser.JavascriptObjectRepository.Register("jsHandler", new JsHandler(browser));
                contentError.Content = browser;
            }
        }

        internal void SetInvoiceError()
        {
            Dispatcher.Invoke(() =>
            {
                var blankContent = ContentHandler.GetLocalContent("failed");
                if (blankContent == null) return;
                var blankHtml = ContentHandler.GetAddressBase64(blankContent);
                var browser = new ChromiumWebBrowser()
                {
                    Address = blankHtml
                };
                browser.JavascriptObjectRepository.Register("jsHandler", new JsHandler(browser));
                contentError.Content = browser;
                tabError.IsSelected = true;
            });
        }
    }
}