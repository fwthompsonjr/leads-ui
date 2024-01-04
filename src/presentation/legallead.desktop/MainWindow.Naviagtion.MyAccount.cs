using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.js;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private void InitializeMyAccountContent()
        {
            var blankContent = ContentHandler.GetLocalContent("myaccount");
            if (blankContent != null)
            {
                var blankHtml = ContentHandler.GetAddressBase64(blankContent);
                var browser = new ChromiumWebBrowser()
                {
                    Address = blankHtml
                };
                browser.JavascriptObjectRepository.Register("jsHandler", new JsHandler(browser));
                contentMyAccount.Content = browser;
            }
        }

        private void NavigateToMyAccount()
        {
            var user = AppBuilder.ServiceProvider?.GetRequiredService<UserBo>();
            if (user == null || !user.IsAuthenicated)
            {
                Dispatcher.Invoke(() =>
                {
                    mnuMyAccount.Visibility = Visibility.Hidden;
                    SetErrorContent(401);
                    tabError.IsSelected = true;
                });
                return;
            }
            Dispatcher.Invoke(() =>
            {
                InitializeMyAccountContent();
                Task.Run(async () =>
                {
                    Thread.Sleep(500);
                    await MapMyAccountDetails();
                });
                mnuMyAccount.Visibility = Visibility.Visible;
                tabMyAccount.IsSelected = true;
            });
        }

        private async Task MapMyAccountDetails()
        {
            var content = Dispatcher.Invoke(() =>
            {
                var container = contentMyAccount.Content;
                if (container is not ChromiumWebBrowser web) return string.Empty;
                return web.GetHTML(Dispatcher);
            });
            if (string.IsNullOrEmpty(content)) return;
            var profile = await MapProfileResponse(content);
            if (string.IsNullOrEmpty(profile)) return;
            var permissions = await MapPermissionsResponse(profile);
            permissions ??= profile;
            Dispatcher.Invoke(() =>
            {
                var container = contentMyAccount.Content;
                if (container is not ChromiumWebBrowser web) return;
                web.SetHTML(Dispatcher, permissions);
            });
        }

        private static async Task<string> MapProfileResponse(string response)
        {
            var provider = AppBuilder.ServiceProvider;
            var user = provider?.GetService<UserBo>();
            var api = provider?.GetService<IPermissionApi>();
            var service = provider?.GetService<IUserProfileMapper>();
            if (api == null || user == null || service == null) return response;
            try
            {
                var resp = await service.Map(api, user, response);
                return resp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return response;
            }
        }

        private static async Task<string> MapPermissionsResponse(string response)
        {
            var provider = AppBuilder.ServiceProvider;
            var user = provider?.GetService<UserBo>();
            var api = provider?.GetService<IPermissionApi>();
            var service = provider?.GetService<IUserPermissionsMapper>();
            if (api == null || user == null || service == null) return response;
            try
            {
                var resp = await service.Map(api, user, response);
                return resp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return response;
            }
        }
    }
}