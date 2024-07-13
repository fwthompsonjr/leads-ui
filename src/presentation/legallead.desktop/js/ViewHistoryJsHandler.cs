using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.handlers;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Windows;

namespace legallead.desktop.js
{
    internal class ViewHistoryJsHandler : MailboxJsHandler
    {
        public ViewHistoryJsHandler(ChromiumWebBrowser? browser) : base(browser) { }


        public void Filter(string id)
        {
            var provider = AppBuilder.ServiceProvider;
            var permissionApi = provider?.GetService<IPermissionApi>();
            var persistence = provider?.GetService<IHistoryPersistence>();
            if (!HasBrowser || 
                web == null || 
                user == null || 
                !user.IsAuthenicated || 
                permissionApi == null ||
                persistence == null) return;
            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out var index)) return;
            var mn = GetMain();
            if (mn == null) return;

            var obj = new UserSearchFilterBo { Index = index };
            var data = persistence.Filter() ?? JsonConvert.SerializeObject(obj);
            var mapped = ObjectExtensions.TryGet<UserSearchFilterBo>(data) ?? obj;
            mapped.Index = index;
            var serialized = JsonConvert.SerializeObject(mapped);
            persistence.SaveFilter(serialized);
            mn.NavigateToMyHistorySearches();

        }

        public override void Fetch(string id)
        {
                var provider = AppBuilder.ServiceProvider;
                var permissionApi = provider?.GetService<IPermissionApi>();
                if (!HasBrowser || web == null || user == null || !user.IsAuthenicated || permissionApi == null) return;
                if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _)) return;
                var model = new GenerateInvoiceModel { Id = id };
                var response = permissionApi.Post("search-get-invoice", model, user).Result;
                if (response == null || response.StatusCode != 200)
                {
                    var mn = GetMain();
                    if (mn == null) return;
                    mn.SetInvoiceError();
                    return;
                }
                var clspreview = new JsPreviewCompleted(web, response?.Message);
                clspreview.Invoice();
        }


        private static MainWindow? GetMain()
        {
            var dispatcher = Application.Current.Dispatcher;
            Window mainWindow = dispatcher.Invoke(() => { return Application.Current.MainWindow; });
            if (mainWindow is not MainWindow main) return null;
            return main;
        }
    }
}
