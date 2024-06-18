using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        internal string ShowFolderBrowserDialog()
        {
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Please select a folder for file download.",
                UseDescriptionForTitle = true // This applies to the Vista style dialog only, not the old dialog.
            };

            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                return string.Empty;
            }
            var isselected = dialog.ShowDialog(this);
            if (isselected.GetValueOrDefault())
            {
                return dialog.SelectedPath;
            }
            return string.Empty;
        }

        private object? GetBrowserTarget(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var find = name.Trim().ToLower();
            if (find.Equals("exit")) return null;

            return Dispatcher.Invoke(() =>
            {
                return find switch
                {
                    "home" => content1.Content,
                    "myaccount" => contentMyAccount.Content,
                    "search" => contentMySearch.Content,
                    "mysearch" => contentMySearch.Content,
                    "mailbox" => contentMyMailbox.Content,
                    "error" => contentError.Content,
                    _ => content1.Content,
                };
            });
        }

        internal void NavigateChild(string destination)
        {
            const string homepages = "home-";
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var sublanding = SubLandings.Find(x => x.Equals(destination, oic));
            if (sublanding == null) return;
            if (!sublanding.StartsWith(homepages) && IsSessionTimeOut())
            {
                NavigateChild("home-login");
                return;
            }
            if (sublanding.Equals("mysearch-actives"))
            {
                NavigateToMyActiveSearches();
                return;
            }
            if (sublanding.Equals("mysearch-history"))
            {
                NavigateToMyHistorySearches();
                return;
            }
            var directions = sublanding.Split('-');
            var parentName = directions[0];
            var parentView = NavigateTo(parentName);

            if (string.IsNullOrEmpty(parentView)) return;

            var targetWindow = GetBrowserTarget(parentView);
            if (targetWindow is not ChromiumWebBrowser web) return;

            var jscript = string.Concat(
                "if ( typeof setDisplay == 'function' ) { ",
                string.Format(JsScriptTokenFormat, directions[1]),
                "}");
            var replacements = new Dictionary<string, string>()
            {
                { UserScriptActivationFalse, UserScriptActivationTrue },
                { UserScriptInjectionToken, jscript }
            };
            var html = new StringBuilder(web.GetHTML(Dispatcher));
            var lookup = replacements.Keys.ToList();
            lookup.ForEach(x => { html.Replace(x, replacements[x]); });
            var substitute = MySearchSubstitute(directions, html.ToString());
            web.SetHTML(Dispatcher, substitute);
        }

        private static bool IsSessionTimeOut()
        {
            var user = AppBuilder.ServiceProvider?.GetService<UserBo>();
            if (user == null) return false;
            return user.IsSessionTimeout();
        }

        private static string MySearchSubstitute(string[] directions, string html)
        {
            if (directions.Length == 0) return html;
            if (!directions[0].Equals("mysearch")) return html;
            var searched = SearchPageContentHelper.Transform(html, directions[1]);
            return searched;
        }

        private static readonly List<string> Landings = new()
        {
            "home",
            "myaccount",
            "mysearch",
            "mailbox",
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
            "myaccount-permissions",
            "myaccount-password",
            "mysearch-home",
            "mysearch-history",
            "mysearch-purchases",
            "mysearch-actives",
            "mailbox-home"
        };

        private const string JsScriptTokenFormat = "setDisplay( '{0}' );";
        private const string UserScriptActivationFalse = "let clientScriptActivated = false;";
        private const string UserScriptActivationTrue = "let clientScriptActivated = true;";
        private const string UserScriptInjectionToken = "/* user injected block */";
    }
}