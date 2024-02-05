using HtmlAgilityPack;
using legallead.desktop;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.Configuration;

namespace legallead.ui.Utilities
{
    internal class MainContentLoadHandler : ContentLoadBase
    {
        private const int IntroductionRetriesMax = 40;
        private int IntroductionRetries = 0;
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
            var homepage = Transform(ButtonClickWriter.ReWrite("home"));
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
                        Task.Run(() =>
                        {
                            var provider = serviceProvider;
                            if (provider == null)
                            {
                                IntroductionRetries = IntroductionRetriesMax - 2;
                                return;
                            }
                            var api = provider.GetRequiredService<IPermissionApi>();
                            var user = provider.GetRequiredService<UserBo>();
                            if (api == null || user == null || user.IsInitialized) return;
                            var list = api.Get("list").Result;
                            if (list == null || list.StatusCode != 200 || string.IsNullOrEmpty(list.Message)) return;
                            var applications = list.Message.TryDeserialize<ApiContext[]>();
                            user.Applications = applications;
                        });
                        break;
                    case IntervalIntroductionPage:
                        var user = serviceProvider?.GetService<UserBo>();
                        if (user == null || !user.IsInitialized)
                        {
                            if (IntroductionRetries < IntroductionRetriesMax)
                            {
                                IntroductionRetries++;
                                return;
                            }
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


        private static string Transform(string html)
        {
            if (!System.Diagnostics.Debugger.IsAttached) { return html; }
            var config = AppBuilder.ServiceProvider?.GetService<IConfiguration>();
            if (config == null) { return html; }
            var targets = new Dictionary<string, string?>
            {
                { "//*[@id='username']", config["debug.user:name"] },
                { "//*[@id='login-password']", config["debug.user:code"] }
            };
            var finders = targets.Keys.ToList();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            finders.ForEach(x =>
            {
                var node = doc.DocumentNode.SelectSingleNode(x);
                var attr = node?.Attributes.FirstOrDefault(a => a.Name.Equals("value"));
                if (node != null && !string.IsNullOrEmpty(targets[x]))
                {
                    if (attr == null)
                    {
                        attr = doc.CreateAttribute("value");
                        node.Attributes.Add(attr);
                    }
                    attr.Value = targets[x];
                }
            });
            return doc.DocumentNode.OuterHtml;
        }

        private const int IntervalHomePage = 200;
        private const int IntervalBlankPage = 1000;
        private const int IntervalIntroductionPage = 2500;
    }
}
