using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using System.Diagnostics.CodeAnalysis;

namespace legallead.ui.Utilities
{
    [SuppressMessage("Minor Code Smell", "S3604:Member initializer values should not be redundant", Justification = "Technical debt. Will address in future release.")]
    internal class ContentLoadBase
    {
        protected System.Timers.Timer? timer;
        protected MainPage? mainPage;
        protected readonly IServiceProvider? serviceProvider = AppBuilder.ServiceProvider;


        internal static string GetHTML(string page)
        {
            return ButtonClickWriter.ReWrite(page);
        }

        protected void SetErrorContent(int errorCode)
        {
            var errorService = AppBuilder.ServiceProvider?.GetRequiredService<IErrorContentProvider>();
            if (errorService == null) return;
            var errorContent = errorService.GetContent(errorCode)?.Content;
            errorContent ??= errorService.GetContent(500)?.Content;
            SetView(errorContent);
        }

        protected void SetView(string? html)
        {
            mainPage = MainPageFinder.GetMain();
            if (mainPage == null || string.IsNullOrEmpty(html)) return;
            mainPage.Dispatcher.Dispatch(() =>
            {
                mainPage.WebViewSource.Html = html;
                MainPageFinder.TryContentReload();
            });
        }
    }
}