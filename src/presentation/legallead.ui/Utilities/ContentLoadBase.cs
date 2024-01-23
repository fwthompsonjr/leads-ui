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
                TryContentReload();
            });
        }

        private void TryContentReload()
        {
            try
            {
                mainPage?.WebViewer.Reload();
            }
            catch
            {
                // this empty catch block is intended
                // if reload fails the content is not valid html
                // and doesnt need a reload
            }
        }
    }
}