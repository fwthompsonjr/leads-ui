using legallead.desktop;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using legallead.ui.Models;
using Mmg = Microsoft.Maui.Graphics;
using System.Drawing;
using Microsoft.Maui.Graphics.Converters;

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


        private bool IsPageIntroduced {  get; set; }

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
            SetView(ContentHandler.GetLocalContent("home")?.Content);
        }

        private void SetView(string? html)
        {
            if (string.IsNullOrEmpty(html)) return;
            mainPage.Dispatcher.Dispatch(() =>
            {
                mainPage.WebViewSource.Html = html;
                try
                {
                    mainPage.WebViewer.Reload();
                }
                catch { 
                    // this empty catch block is intended
                    // if reload fails the content is not valid html
                    // and doesnt need a reload
                }
                
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
        private void SetStatus(int statusId)
        {
            var model = serviceProvider?.GetService<MainWindowViewModel>();
            var messages = serviceProvider?.GetService<CommonMessageList>()?.Messages;
            if (model == null || messages == null) return;
            var status = messages.Find(x => x.Id == statusId);
            if (status == null) return;
            mainPage.Dispatcher?.Dispatch(() =>
            {
                mainPage.StatusIcon.Background = GetColorFromString(status.Color);
                mainPage.StatusText.Text = status.Name;
                mainPage.StatusMessage.Text = status.Message;
                var current = mainPage.StatusConnection.Text;
                mainPage.StatusConnection.Text = status.Id switch
                {
                    1 => "Offline",
                    10 => "Connected",
                    _ => current
                };
            });
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
                            SetStatus(errorId);
                            SetErrorContent(500);
                            return;
                        }
                        int readyId = (int)CommonStatusTypes.Ready;
                        SetStatus(readyId);
                        timer.Interval = IntervalHomePage;
                        break;
                }
            }
            finally
            {
                IsPageIntroduced = true;
            }
        }


        private static SolidColorBrush GetColorFromString(string colorString)
        {
            var fallback = Brush.Black;
            try
            {
                ColorTypeConverter converter = new();
                var obj = converter.ConvertFromInvariantString(colorString);
                if (obj is not Mmg.Color color) return fallback;
                return new SolidColorBrush(color);
            }
            catch (Exception)
            {
                return fallback;
            }
        }

        private const int IntervalHomePage = 200;
        private const int IntervalBlankPage = 1000;
        private const int IntervalIntroductionPage = 2500;
    }
}
