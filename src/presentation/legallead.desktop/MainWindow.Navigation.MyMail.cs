using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.js;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private void InitializeMyMailBox()
        {
            var blankContent = ContentHandler.GetLocalContent("mailbox");
            var mapper = AppBuilder.ServiceProvider?.GetService<IUserMailboxMapper>();
            var persistence = AppBuilder.ServiceProvider?.GetService<IMailPersistence>();
            if (blankContent != null && mapper != null)
            {
                blankContent.Content = mapper.Substitute(persistence, blankContent.Content);
            }
            if (blankContent != null)
            {
                var blankHtml = ContentHandler.GetAddressBase64(blankContent);
                var browser = new ChromiumWebBrowser()
                {
                    Address = blankHtml
                };
                browser.JavascriptObjectRepository.Register("jsHandler", new MailboxJsHandler(browser));
                contentMyMailbox.Content = browser;
            }
        }

        internal void NavigateToMyMailBox()
        {
            var user = AppBuilder.ServiceProvider?.GetService<UserBo>();
            if (user == null || !user.IsAuthenicated)
            {
                Dispatcher.Invoke(() =>
                {
                    SetErrorContent(401);
                    tabError.IsSelected = true;
                });
                return;
            }
            Dispatcher.Invoke(() =>
            {
                InitializeMyMailBox();
                tabMyMailbox.IsSelected = true;
            });
        }
    }
}