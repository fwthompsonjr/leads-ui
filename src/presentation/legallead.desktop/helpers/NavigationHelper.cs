using CefSharp.DevTools.Profiler;
using CefSharp.Wpf;
using legallead.desktop.utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace legallead.desktop.helpers
{
    internal static class NavigationHelper
    {
        public static object? GetBrowserTarget(string name)
        {
            var pointer = new MainWindowPointer();
            return pointer.GetBrowser(name);
        }

        public static async Task PopulateMyAccount()
        {
            var pointer = new MainWindowPointer();
            await pointer.MapMyAccount();
        }

        public static readonly List<string> Landings = new()
        {
            "home",
            "myaccount",
            "mysearch",
            "error",
            "exit"
        };

        public static readonly List<string> SubLandings = new()
        {
            "home-home",
            "home-login",
            "home-register",
            "myaccount-home",
            "myaccount-profile",
            "myaccount-permissions",
            "myaccount-password",
            "mysearch-home",
            "mysearch-history",
            "mysearch-purchases",
        };

        private sealed class MainWindowPointer
        {
            private readonly MainWindow? window;

            public MainWindowPointer()
            {
                var wnd = Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    return Application.Current.MainWindow;
                });
                if (wnd is not MainWindow main) return;
                window = main;
            }

            public object? GetBrowser(string name)
            {
                if (window == null) return null;
                if (string.IsNullOrEmpty(name)) return null;

                var find = name.Trim().ToLower();
                if (find.Equals("exit")) return null;

                var dispatcher = window.Dispatcher;
                return dispatcher.Invoke(() =>
                {
                    return find switch
                    {
                        "home" => window.content1.Content,
                        "myaccount" => window.contentMyAccount.Content,
                        "search" => window.contentMySearch.Content,
                        "error" => window.contentError.Content,
                        _ => window.content1.Content,
                    };
                });
            }

            public async Task MapMyAccount()
            {
                if (window == null) return;
                string content = string.Empty;
                var list = new List<Task<string>>
                {
                    new(() =>
                    {
                        var dispatcher = window.Dispatcher;
                        content = dispatcher.Invoke(() =>
                        {
                            var container = window.contentMyAccount.Content;
                            if (container is not ChromiumWebBrowser web) return string.Empty;
                            return web.GetHTML(dispatcher);
                        });
                        return content;
                    }),
                    new(() =>
                    {
                        if (string.IsNullOrEmpty(content)) return content ?? string.Empty;
                        var profile = ProfileHelper.GetContent(content).Result;
                        if (string.IsNullOrEmpty(profile)) return content;
                        content = profile;
                        return content;
                    }),
                    new(() =>
                    {
                        if (string.IsNullOrEmpty(content)) return content ?? string.Empty;
                        var permissions = PermissionsHelper.GetContent(content).Result;
                        if (string.IsNullOrEmpty(permissions)) return content;
                        content = permissions;
                        return content;
                    }),
                    new(() =>
                    {
                        if (string.IsNullOrEmpty(content)) return content ?? string.Empty;
                        var dispatcher = window.Dispatcher;
                        dispatcher.Invoke(() =>
                        {
                            var container = window.contentMyAccount.Content;
                            if (container is not ChromiumWebBrowser web) return;
                            web.SetHTML(dispatcher, content);
                        });
                        return content;
                    })
                };
                foreach (var t in list) { await t; }
            }
        }
    }
}