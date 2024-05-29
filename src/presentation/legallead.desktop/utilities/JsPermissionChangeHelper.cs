using CefSharp.Wpf;
using legallead.desktop.js;
using legallead.desktop.models;
using System;
using System.Linq;

namespace legallead.desktop.utilities
{
    internal static class JsPermissionChangeHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Code Analysis",
            "S6602:\"Find\" method should be used instead of the \"FirstOrDefault\" extension",
            Justification = "Find method not available for array")]
        public static void OnSubmissionCompleted(
            string landing,
            string response,
            JsPermissionChange changeHandler,
            ChromiumWebBrowser? web)
        {
            var paymentLandings = new[] { "Discounts", "Subscription" };
            var landingName = paymentLandings.FirstOrDefault(x => x.Equals(landing, StringComparison.OrdinalIgnoreCase));
            if (landingName == null) { return; }
            if (web == null)
            {
                changeHandler.Reload(targetName);
                return;
            }
            ChangeHandler(response, changeHandler, web);
        }

        private static void ChangeHandler(string response, JsPermissionChange changeHandler, ChromiumWebBrowser web)
        {
            var levelChange = ObjectExtensions.TryGet<SubscriptionChangeResponse>(response);
            var navigateTo = levelChange?.Dto?.InvoiceUri ?? "NONE";
            if (navigateTo.Equals("NONE"))
            {
                changeHandler.Reload(targetName);
                return;
            }
            web.LoadUrl(navigateTo);
        }
        private const string targetName = "myaccount-permissions";
    }
}
