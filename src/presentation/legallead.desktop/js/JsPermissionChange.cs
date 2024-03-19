using AngleSharp.Text;
using CefSharp;
using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.handlers;
using legallead.desktop.interfaces;
using legallead.desktop.models;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace legallead.desktop.js
{
    internal class JsPermissionChange : JsHandler
    {
        private readonly IPermissionApi? permissionApi;
        private readonly UserBo? user;

        private bool HasBrowser => permissionApi != null &&
            user != null &&
            web != null &&
            web.CanExecuteJavascriptInMainFrame;

        public JsPermissionChange(ChromiumWebBrowser? browser) : base(browser)
        {
            var provider = AppBuilder.ServiceProvider;
            permissionApi = provider?.GetService<IPermissionApi>();
            user = provider?.GetService<UserBo>();
        }

        public override void Submit(string formName, string json)
        {
            if (!HasBrowser || user == null) return;
            var name = PermissionForms.Find(p => p.Equals(formName, StringComparison.OrdinalIgnoreCase));
            if (name == null) { return; }
            var submission = ObjectExtensions.TryGet<UserPermissionChangeRequest>(json);
            if (submission == null || !submission.CanSubmit) return;
            var js = MapPayload(submission);
            var response = permissionApi?.Post(AddressMap[submission.SubmissionName], js, user).Result;
            var htm = JsCompletedHandler.ConvertHTML(response);
            if (response == null || response.StatusCode != 200)
            {
                SetMessage(htm);
                return;
            }
            SubmitCompleted();
            var paymentLandings = new[] { "Discounts", "Subscription" };
            if (paymentLandings.Contains(submission.SubmissionName, StringComparison.OrdinalIgnoreCase))
            {
                var levelChange = ObjectExtensions.TryGet<SubscriptionChangeResponse>(response.Message);
                var navigateTo = levelChange?.InvoiceUri ?? "NONE";
                if (navigateTo.Equals("NONE"))
                {
                    Reload("myaccount-permissions");
                    return;
                }
                web?.LoadUrl(navigateTo);
            }
        }

        /// <summary>
        /// Echo a message to UI in the submission status div
        /// </summary>
        /// <param name="htm"></param>
        private void SetMessage(string htm)
        {
            if (!HasBrowser || web == null) return;
            web.ExecuteScriptAsync(scriptNames[1], htm, true);
        }

        /// <summary>
        /// Resets UI to proper post submission completed state
        /// </summary>
        private void SubmitCompleted()
        {
            if (!HasBrowser || web == null) return;
            web.ExecuteScriptAsync(scriptNames[2]);
        }

        private static object MapPayload(UserPermissionChangeRequest request)
        {
            try
            {
                var changeType = request.SubmissionName;
                switch (changeType)
                {
                    case "Subscription":
                        return JsonConvert.DeserializeObject<ContactLevel>(request.Subscription) ?? new();

                    case "Discounts":
                        var selections = JsonConvert.DeserializeObject<DiscountChoice[]>(request.Discounts);
                        if (selections == null) return new();
                        var discountRequest = new { Choices = selections };
                        return discountRequest;

                    case "Changes":
                        return JsonConvert.DeserializeObject<ContactChangePassword>(request.Changes) ?? new();

                    default:
                        return new();
                }
            }
            catch (Exception)
            {
                return new();
            }
        }

        private static readonly Dictionary<string, string> AddressMap = new()
        {
            { "Changes", "permissions-change-password" },
            { "Discounts", "permissions-set-discount" },
            { "Subscription", "permissions-set-permission" }
        };

        private static readonly List<string> scriptNames = new()
            {
                "setProfileIconState",
                "setProfileStatusMessage",
                "profileActionCompleted",
                "setSuccessAlert"
            };
    }
}