using CefSharp;
using CefSharp.Wpf;
using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.implementations;
using legallead.desktop.interfaces;
using legallead.desktop.models;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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

        public void Fetch(string id)
        {
            if (!HasBrowser || web == null || mailSvc == null || user == null || !user.IsAuthenicated) return;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _)) return;
            var content = mailSvc.Fetch(id);
            if (string.IsNullOrEmpty(content)) return;
            var dvhtml = GetEmailDiv(content);
            var dispatcher = Application.Current.Dispatcher;
            var html = web.GetHTML(dispatcher);
            if (string.IsNullOrEmpty(html)) return;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            SetItemIndex(doc, id);
            SetActiveMailItem(doc, id);
            var previewPane = doc.DocumentNode.SelectSingleNode("//*[@id='dv-mail-item-preview']");
            if (previewPane == null) return;
            previewPane.InnerHtml = MailboxMapper.RestyleBlue(dvhtml);
            web.SetHTML(dispatcher, doc.DocumentNode.OuterHtml);
        }

        private void SetItemIndex(HtmlDocument doc, string id)
        {
            const string subHeader = "//*[@id=\"mailbox-sub-header\"]";
            if (doc == null || mailSvc == null) return;
            var data = mailSvc.Fetch() ?? string.Empty;
            var collection = ObjectExtensions.TryGet<List<MailStorageItem>>(data);
            if (collection.Count == 0) return;
            var selected = collection.Find(item =>
            {
                if (string.IsNullOrEmpty(item.Id)) return false;
                return item.Id.Equals(id);
            });
            var mx = collection.Max(x => x.PositionId);
            var indx = selected == null ? 0 : selected.PositionId;
            var heading = selected == null ? "Correspondence" : $"Correspondence ( {indx} of {mx} )";
            var headingElement = doc.DocumentNode.SelectSingleNode(subHeader);
            if (headingElement != null) headingElement.InnerHtml = heading;
        }

        private static void SetActiveMailItem(HtmlDocument doc, string id)
        {
            if (doc == null) return;
            var collection = doc.DocumentNode.SelectNodes("//a[@name='link-mail-items-template']").ToList();
            if (collection.Count == 0) return;
            collection.ForEach(item =>
            {
                item.RemoveClass("active");
                var htm = item.InnerHtml.Trim();
                if (!string.IsNullOrEmpty(htm) && htm.Contains(id)) { item.AddClass("active"); }
            });
        }

        private static string GetEmailDiv(string original)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(original);
                var div = doc.DocumentNode.SelectSingleNode("//*[@id='email']");
                if (div == null) return original;
                return div.OuterHtml;
            }
            catch (Exception)
            {
                return original;
            }
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
