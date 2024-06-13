using CefSharp;
using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.models;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace legallead.desktop.js
{
    internal class MailboxJsHandler : JsHandler
    {
        private readonly IPermissionApi? permissionApi;
        private readonly IMailPersistence? mailSvc;
        private readonly UserBo? user;

        private bool HasBrowser => permissionApi != null &&
            user != null &&
            web != null &&
            web.CanExecuteJavascriptInMainFrame;

        public MailboxJsHandler(ChromiumWebBrowser? browser) : base(browser)
        {
            var provider = AppBuilder.ServiceProvider;
            permissionApi = provider?.GetService<IPermissionApi>();
            user = provider?.GetService<UserBo>();
            mailSvc = provider?.GetService<IMailPersistence>();
        }

        public void Repopulate()
        {
            bool found = false;
            try
            {
                if (!HasBrowser || web == null || mailSvc == null || user == null || !user.IsAuthenicated) return;
                var messages = mailSvc.Fetch();
                if (string.IsNullOrEmpty(messages)) return;
                var data = ObjectExtensions.TryGet<List<MailStorageItem>>(messages);
                if (data.Count == 0) return;
                found = true;
                web.ExecuteScriptAsync(mailboxScriptNames[1], messages);
            }
            finally
            {
                if (!found) { web?.ExecuteScriptAsync(mailboxScriptNames[0]); }
            }
        }

        public string Fetch(string id)
        {
            var response = string.Empty;
            if (!HasBrowser || web == null || mailSvc == null || user == null || !user.IsAuthenicated) return response;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _)) return response;
            var content = mailSvc.Fetch(id);
            if (string.IsNullOrEmpty(content)) return response;
            web.ExecuteScriptAsync(mailboxScriptNames[2], content);
            return "ok";
        }

        private static readonly List<string> mailboxScriptNames = new()
            {
                "clearContent",
                "appendMailCollection",
                "viewSelectedItem",
                "loadMailBox"
            };
    }
}
