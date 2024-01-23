using legallead.desktop;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;

namespace legallead.ui.Utilities
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell",
            "S3604:Member initializer values should not be redundant",
            Justification = "Technical debt. Will address in future release.")]
    internal class MainContentLoadHandler(MainPage main)
    {

        private readonly MainPage mainPage = main;
        private readonly IServiceProvider? serviceProvider = AppBuilder.ServiceProvider;
        private System.Timers.Timer? timer;


        private bool IsPageIntroduced { get; set; }

        public void SetBlank()
        {
            if (IsPageIntroduced) return;
            SetView(ContentHandler.GetLocalContent("blank")?.Content);
            timer ??= new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = IntervalBlankPage;
            timer.Start();
        }

        public void SetHome()
        {
            var homepage = ButtonClickWriter.ReWrite("home");
            SetView(homepage);
        }

        private void SetView(string? html)
        {
            if (string.IsNullOrEmpty(html)) return;
            mainPage.Dispatcher.Dispatch(() =>
            {
                mainPage.WebViewSource.Html = html;
                TryContentReload();
            });
        }

        private void SetErrorContent(int errorCode)
        {
            var errorService = AppBuilder.ServiceProvider?.GetRequiredService<IErrorContentProvider>();
            if (errorService == null) return;
            var errorContent = errorService.GetContent(errorCode)?.Content;
            errorContent ??= errorService.GetContent(500)?.Content;
            SetView(errorContent);
        }

        private void TryContentReload()
        {
            try
            {
                mainPage.WebViewer.Reload();
            }
            catch
            {
                // this empty catch block is intended
                // if reload fails the content is not valid html
                // and doesnt need a reload
            }
        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (timer == null) return;
                if (timer.Interval == IntervalBlankPage && IsPageIntroduced) return;
                switch (timer.Interval)
                {
                    case IntervalHomePage:
                        timer.Stop();
                        SetView(ContentHandler.GetLocalContent("home")?.Content);
                        break;
                    case IntervalBlankPage:
                        SetView(ContentHandler.GetLocalContent("introduction")?.Content);
                        timer.Interval = IntervalIntroductionPage;
                        break;
                    case IntervalIntroductionPage:
                        var user = serviceProvider?.GetService<UserBo>();
                        if (user == null || !user.IsInitialized)
                        {
                            timer.Stop();
                            int errorId = (int)CommonStatusTypes.Error;
                            StatusBarHelper.SetStatus(errorId);
                            SetErrorContent(500);
                            return;
                        }
                        int readyId = (int)CommonStatusTypes.Ready;
                        StatusBarHelper.SetStatus(readyId);
                        timer.Interval = IntervalHomePage;
                        break;
                }
            }
            finally
            {
                IsPageIntroduced = true;
            }
        }

        private const int IntervalHomePage = 200;
        private const int IntervalBlankPage = 1000;
        private const int IntervalIntroductionPage = 2500;
    }
}
