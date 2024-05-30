using CefSharp.Wpf;
using legallead.desktop.js;
using legallead.desktop.models;
using System;
using System.Collections.Generic;
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
            var landingName = paymentLandings.FirstOrDefault(x => x.Equals(landing, StringComparison.OrdinalIgnoreCase));
            if (landingName == null) { return; }
            if (web == null)
            {
                changeHandler.Reload(targetName);
                return;
            }
            ChangeHandler(response, landingName, changeHandler, web);
        }

        private static void ChangeHandler(string response, string landingName, JsPermissionChange changeHandler, ChromiumWebBrowser web)
        {
            var levelChange = ObjectExtensions.TryGet<SubscriptionChangeResponse>(response);
            var navigateTo = GetInvoiceUri(levelChange, landingName);
            if (navigateTo.Equals("NONE"))
            {
                changeHandler.Reload(targetName);
                return;
            }
            web.LoadUrl(navigateTo);
        }

        private static string GetInvoiceUri(SubscriptionChangeResponse? response, string landingName)
        {
            
            const string guest = "guest";
            if (response == null || response.Dto == null) return none;
            var levelName = response.Dto.LevelName ?? string.Empty;
            var navigateTo = response.Dto.InvoiceUri ?? none;
            if (!navigateTo.Equals(none, StringComparison.OrdinalIgnoreCase)) { return navigateTo; }
            if (!levelName.Equals(guest, StringComparison.OrdinalIgnoreCase)) { return navigateTo; }
            return BuildUri(landingName, response.Dto);
        }

        private static string BuildUri(string landing, SubscriptionChangeResponseDto dto)
        {
            var landingName = paymentLandings.Find(x => x.Equals(landing, StringComparison.OrdinalIgnoreCase));
            if (landingName == null) { return none; }
            if (string.IsNullOrWhiteSpace(dto.ExternalId)) return none;
            if (string.IsNullOrWhiteSpace(dto.SessionId)) return none;
            var id = paymentLandings.IndexOf(landingName);
            var hostname = AppBuilder.PermissionApiBase;
            if (string.IsNullOrEmpty(hostname)) return none;
            var url = string.Concat(paymentUrls[id], "?id={1}&sessionid={2}");
            return string.Format(url, hostname, dto.ExternalId, dto.SessionId);
        }

        private const string targetName = "myaccount-permissions";
        private static readonly List<string> paymentLandings = new[] { "Discounts", "Subscription" }.ToList();
        private static readonly string[] paymentUrls = new[] { "{0}discount-checkout", "{0}subscription-checkout" };
        private const string none = "NONE";
    }
}
