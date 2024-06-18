using CefSharp.Wpf;
using System;

namespace legallead.desktop.js
{
    internal class ViewHistoryJsHandler : MailboxJsHandler
    {
        public ViewHistoryJsHandler(ChromiumWebBrowser? browser) : base(browser) { }
        public override void Fetch(string id)
        {
            if (!HasBrowser || web == null || user == null || !user.IsAuthenicated) return;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var _)) return;
            base.Fetch(id);
        }
    }
}
