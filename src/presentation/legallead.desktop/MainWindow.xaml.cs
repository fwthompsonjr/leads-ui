using CefSharp;
using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.js;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isBlankLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBrowserContent();
            InitializeErrorContent();
            ContentRendered += MainWindow_ContentRendered;
            mnuExit.Click += MnuExit_Click;
        }

        private void InitializeBrowserContent()
        {
            string[] names = new[] { "home", "blank" };
            var target = isBlankLoaded ? names[0] : names[1];
            var blankContent = ContentHandler.GetLocalContent(target);
            if (blankContent != null)
            {
                var blankHtml = ContentHandler.GetAddressBase64(blankContent);
                var browser = new ChromiumWebBrowser()
                {
                    Address = blankHtml
                };
                var handler = target == names[1] ? new JsHandler(browser) : new HomeJsHandler(browser);
                browser.JavascriptObjectRepository.Register("jsHandler", handler);
                content1.Content = browser;
            }
        }

        private void InitializeErrorContent()
        {
            SetErrorContent(404);
        }

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

        private void MainWindow_ContentRendered(object? sender, System.EventArgs e)
        {
            if (isBlankLoaded) { return; }
            try
            {
                Task.Run(() =>
                {
                    Thread.Sleep(1500);
                    var initialPage = AppBuilder.InitialViewName ?? "introduction";
                    var helper = GetHelper();
                    helper.Load(initialPage, Dispatcher, content1);
                    helper.Handler?.OnPageLoaded();
                }).ConfigureAwait(false);
            }
            finally
            {
                isBlankLoaded = true;
            }
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MnuAccount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem mnu) return;
            if (mnu.Tag is not string mnuCommand) return;
            if (string.IsNullOrEmpty(mnuCommand)) return;
            NavigateChild(mnuCommand);
        }

        private void MnuHome_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem _) return;
            tabHome.IsSelected = true;
        }

        private void MnuError_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem item) return;
            var menuId = Convert.ToInt32(item.Name.Replace("mnu", ""));
            if (menuId == 0) return;
            tabError.IsSelected = true;
            SetErrorContent(menuId);
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

        private BrowserHelper GetHelper()
        {
            var window = (Window)this;
            return new BrowserHelper(window);
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

        internal void NavigateChild(string destination)
        {
            var sublanding = SubLandings.Find(x => x.Equals(destination, StringComparison.OrdinalIgnoreCase));
            if (sublanding == null) return;
            var directions = sublanding.Split('-');
            var parentName = directions[0];
            var parentView = NavigateTo(parentName);
            if (string.IsNullOrEmpty(parentView)) return;

            var targetWindow = GetBrowserTarget(parentView);
            if (targetWindow is not ChromiumWebBrowser web) return;
            var script = $"setDisplay( '{directions[1]}' );";
            var replacements = new Dictionary<string, string>()
            {
                { "let clientScriptActivated = false;", "let clientScriptActivated = true;" },
                { "/* user injected block */", script }
            };
            var html = new StringBuilder(web.GetHTML(Dispatcher));
            foreach (var replace in replacements)
            {
                html.Replace(replace.Key, replace.Value);
            }
            web.SetHTML(Dispatcher, html.ToString());
        }

        internal string? NavigateTo(string destination, int errorCode = 0)
        {
            var landing = Landings.Find(x => x.Equals(destination, StringComparison.OrdinalIgnoreCase));
            if (landing == null) return null;
            switch (landing)
            {
                case "home":
                    Dispatcher.Invoke(() =>
                    {
                        InitializeBrowserContent();
                        tabHome.IsSelected = true;
                    });
                    break;

                case "error":
                    Dispatcher.Invoke(() =>
                    {
                        SetErrorContent(errorCode);
                        tabError.IsSelected = true;
                    });
                    break;

                case "myaccount":
                    var user = AppBuilder.ServiceProvider?.GetRequiredService<UserBo>();
                    if (user == null || !user.IsAuthenicated)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            mnuMyAccount.Visibility = Visibility.Hidden;
                            SetErrorContent(401);
                            tabError.IsSelected = true;
                        });
                        return null;
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

                    break;

                case "exit":
                    Environment.Exit(0);
                    break;
            }
            return landing;
        }

        private object? GetBrowserTarget(string name)
        {
            return Dispatcher.Invoke(() =>
            {
                return name.Equals("home") ? content1.Content : contentMyAccount.Content;
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

        private static readonly List<string> Landings = new()
        {
            "home",
            "myaccount",
            "error",
            "exit"
        };

        private static readonly List<string> SubLandings = new()
        {
            "home-home",
            "home-login",
            "home-register",
            "myaccount-home",
            "myaccount-profile",
            "myaccount-permissions"
        };
    }
}