﻿using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace legallead.desktop.js
{
    internal class JsHandler
    {
        protected readonly ChromiumWebBrowser? web;

        public JsHandler(ChromiumWebBrowser? browser)
        {
            web = browser;
            AppBuilder.Build();
        }

        public virtual void Initialize()
        {
        }

        public virtual void Submit(string formName, string json)
        {
        }

        public virtual Action<object?>? OnInitCompleted { get; set; }

        protected static void Init()
        {
            var provider = AppBuilder.ServiceProvider;
            if (provider == null) return;

            var api = provider.GetRequiredService<PermissionApi>();
            var user = provider.GetRequiredService<UserBo>();
            if (api == null || user == null || user.IsInitialized) return;
            var list = api.Get("list").Result;
            if (list == null || list.StatusCode != 200 || string.IsNullOrEmpty(list.Message)) return;
            var applications = TryDeserialize<ApiContext[]>(list.Message);
            user.Applications = applications;
        }

        protected static T? TryDeserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}