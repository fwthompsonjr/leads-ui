using CefSharp.Wpf;
using legallead.desktop.utilities;
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
                    "error" => contentError.Content,
                    _ => content1.Content,
                };
            });
        }

        internal void NavigateChild(string destination)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var sublanding = SubLandings.Find(x => x.Equals(destination, oic));
            if (sublanding == null) return;

            var directions = sublanding.Split('-');
            var parentName = directions[0];
            var parentView = NavigateTo(parentName);

            if (string.IsNullOrEmpty(parentView)) return;

            var targetWindow = GetBrowserTarget(parentView);
            if (targetWindow is not ChromiumWebBrowser web) return;

            var jscript = string.Format(JsScriptTokenFormat, directions[1]);
            var replacements = new Dictionary<string, string>()
            {
                { UserScriptActivationFalse, UserScriptActivationTrue },
                { UserScriptInjectionToken, jscript }
            };

            var html = new StringBuilder(web.GetHTML(Dispatcher));
            var lookup = replacements.Keys.ToList();
            lookup.ForEach(x => { html.Replace(x, replacements[x]); });

            web.SetHTML(Dispatcher, html.ToString());
        }

        private static readonly List<string> Landings = new()
        {
            "home",
            "myaccount",
            "mysearch",
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
        };

        private const string JsScriptTokenFormat = "setDisplay( '{0}' );";
        private const string UserScriptActivationFalse = "let clientScriptActivated = false;";
        private const string UserScriptActivationTrue = "let clientScriptActivated = true;";
        private const string UserScriptInjectionToken = "/* user injected block */";
    }
}