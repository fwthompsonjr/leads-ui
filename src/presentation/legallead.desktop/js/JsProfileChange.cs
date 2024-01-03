using CefSharp;
using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.handlers;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace legallead.desktop.js
{
    internal class JsProfileChange : JsHandler
    {
        private readonly IPermissionApi? permissionApi;
        private readonly UserBo? user;

        private bool HasBrowser => permissionApi != null &&
            user != null &&
            web != null &&
            web.CanExecuteJavascriptInMainFrame;

        public JsProfileChange(ChromiumWebBrowser? browser) : base(browser)
        {
            var provider = AppBuilder.ServiceProvider;
            permissionApi = provider?.GetService<IPermissionApi>();
            user = provider?.GetService<UserBo>();
        }

        public override void Submit(string formName, string json)
        {
            if (!HasBrowser || user == null) return;
            var name = ProfileForms.Find(p => p.Equals(formName, StringComparison.OrdinalIgnoreCase));
            if (name == null) { return; }
            var isMapped = AddressMap.ContainsKey(name);
            if (!isMapped) { return; }
            try
            {
                var js = MapPayload(formName, json);
                var response = permissionApi?.Post(AddressMap[name], js, user).Result;
                var htm = JsCompletedHandler.ConvertHTML(response);
                SetMessage(htm);
                if (response == null || response.StatusCode != 200) return;
                SubmitCompleted();
            }
            catch (Exception ex)
            {
                var exresponse = new ApiResponse { StatusCode = 500, Message = ex.Message };
                var exmsg = JsCompletedHandler.ConvertHTML(exresponse);
                SetMessage(exmsg);
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

        private static object MapPayload(string formName, object payload)
        {
            var js = Convert.ToString(payload);
            var typeMap = PayloadMap[formName];
            var mapped = JsonConvert.DeserializeObject(js, typeMap);
            return mapped ?? new();
        }

        private static readonly Dictionary<string, string> AddressMap = new()
        {
            { "frm-profile-personal", "profile-edit-contact-name" },
            { "frm-profile-address", "profile-edit-contact-address" },
            { "frm-profile-phone", "profile-edit-contact-phone" },
            { "frm-profile-email", "profile-edit-contact-email" }
        };

        private static readonly Dictionary<string, Type> PayloadMap = new()
        {
            { "frm-profile-personal", typeof(ContactName[]) },
            { "frm-profile-address", typeof(ContactAddress[]) },
            { "frm-profile-phone", typeof(ContactPhone[]) },
            { "frm-profile-email", typeof(ContactEmail[]) }
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