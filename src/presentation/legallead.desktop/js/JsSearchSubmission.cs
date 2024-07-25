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
using System.Windows;

namespace legallead.desktop.js
{
    internal class JsSearchSubmission : JsHandler
    {
        private readonly IPermissionApi? permissionApi;
        private readonly UserBo? user;

        private bool HasBrowser => permissionApi != null &&
            user != null &&
            web != null &&
            web.CanExecuteJavascriptInMainFrame;

        public JsSearchSubmission(ChromiumWebBrowser? browser) : base(browser)
        {
            var provider = AppBuilder.ServiceProvider;
            permissionApi = provider?.GetService<IPermissionApi>();
            user = provider?.GetService<UserBo>();
        }

        public override void Submit(string formName, string json)
        {
            if (!HasBrowser || user == null) return;
            var name = SearchForms.Find(p => p.Equals(formName, StringComparison.OrdinalIgnoreCase));
            if (name == null) { return; }
            var isMapped = AddressMap.ContainsKey(name);
            if (!isMapped) { return; }
            try
            {
                var js = JsSearchSubmissionHelper.Refine(MapPayload(formName, json));
                var response = permissionApi?.Post(AddressMap[name], js, user).Result;
                var htm = JsCompletedHandler.ConvertHTML(response);
                SetMessage(htm);
                if ((response == null || response.StatusCode != 200) && SearchForms[4] == formName)
                {
                    var mn = GetMain();
                    if (mn == null) return;
                    mn.SetInvoiceError();
                    return;
                }
                if (response == null || response.StatusCode != 200) return;
                if (SearchForms[3] == formName)
                {
                    var clspreview = new JsPreviewCompleted(web, response.Message);
                    clspreview.Complete();
                    return;
                }
                if (SearchForms[4] == formName)
                {
                    var clspreview = new JsPreviewCompleted(web, response.Message);
                    clspreview.Invoice();
                    return;
                }
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
            var queueStart = GetQueueStarter();
            queueStart?.Start();
            web.ExecuteScriptAsync(scriptNames[2]);
        }
        private static IQueueStarter? GetQueueStarter()
        {
            var provider = AppBuilder.ServiceProvider;
            var starter = provider?.GetService<IQueueStarter>();
            return starter;
        }
        private static object MapPayload(string formName, object payload)
        {
            var js = Convert.ToString(payload);
            if (js == null) return new();
            var typeMap = PayloadMap[formName];
            var mapped = JsonConvert.DeserializeObject(js, typeMap);
            return mapped ?? new();
        }

        private static readonly Dictionary<string, string> AddressMap = new()
        {
            { "frm-search", "search-begin" },
            { "frm-search-history", "profile-edit-contact-address" },
            { "frm-search-purchases", "profile-edit-contact-phone" },
            { "frm-search-preview", "search-get-preview" },
            { "frm-search-invoice", "search-get-invoice" }
        };

        private static readonly Dictionary<string, Type> PayloadMap = new()
        {
            { "frm-search", typeof(BeginSearchModel) },
            { "frm-search-history", typeof(ContactAddress[]) },
            { "frm-search-purchases", typeof(ContactPhone[]) },
            { "frm-search-preview", typeof(SearchPreviewModel) },
            { "frm-search-invoice", typeof(GenerateInvoiceModel) }
        };



        private static MainWindow? GetMain()
        {
            var dispatcher = Application.Current.Dispatcher;
            Window mainWindow = dispatcher.Invoke(() => { return Application.Current.MainWindow; });
            if (mainWindow is not MainWindow main) return null;
            return main;
        }

        private static readonly List<string> scriptNames = new()
            {
                "setProfileIconState",
                "setProfileStatusMessage",
                "profileActionCompleted",
                "setSuccessAlert"
            };
    }
}