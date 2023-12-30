using CefSharp.Wpf;
using legallead.desktop.entities;
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
        public JsProfileChange(ChromiumWebBrowser? browser) : base(browser)
        {
        }

        public override void Submit(string formName, string json)
        {
            var name = ProfileForms.Find(p => p.Equals(formName, StringComparison.OrdinalIgnoreCase));
            if (name == null) { return; }
            var isMapped = AddressMap.ContainsKey(name);
            if (!isMapped) { return; }
            var provider = AppBuilder.ServiceProvider;
            var api = provider?.GetService<IPermissionApi>();
            var user = provider?.GetService<UserBo>();
            if (api == null || user == null) { return; }
            var response = api.Post(AddressMap[name], JsonConvert.DeserializeObject(json) ?? new(), user).Result;
            if (response == null || response.StatusCode != 200)
            {
                Console.WriteLine("Unexpected err.");
                return;
            }
            Console.WriteLine(response.Message);
        }

        private static readonly Dictionary<string, string> AddressMap = new Dictionary<string, string>()
        {
            { "frm-profile-personal", "profile-edit-contact-name" },
            { "frm-profile-address", "profile-edit-contact-address" },
            { "frm-profile-phone", "profile-edit-contact-phone" },
            { "frm-profile-email", "profile-edit-contact-email" }
        };
    }
}