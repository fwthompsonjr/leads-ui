using legallead.desktop;
using legallead.desktop.entities;

namespace legallead.ui.Utilities
{
    internal class MainContentLoadHandler : ContentLoadBase
    {

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
