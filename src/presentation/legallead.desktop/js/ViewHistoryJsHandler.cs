using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.handlers;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace legallead.desktop.js
{
    internal class ViewHistoryJsHandler : MailboxJsHandler
    {
        public ViewHistoryJsHandler(ChromiumWebBrowser? browser) : base(browser) { }
        public override void Fetch(string id)
        {
            var provider = AppBuilder.ServiceProvider;
            var permissionApi = provider?.GetService<IPermissionApi>();
            if (!HasBrowser || web == null || user == null || !user.IsAuthenicated || permissionApi == null) return;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _)) return;
            var model = new GenerateInvoiceModel { Id = id };
            var response = permissionApi.Post("search-get-invoice", model, user).Result;
            if (response == null || response.StatusCode != 200) return;
            var clspreview = new JsPreviewCompleted(web, response.Message);
            clspreview.Invoice();
        }
    }
}
