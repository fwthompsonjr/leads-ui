using legallead.desktop.entities;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace legallead.desktop.js
{
    internal class JsHandler
    {
        public JsHandler()
        {
            AppBuilder.Build();
        }

        public virtual void Initialize()
        {
        }

        public virtual string Submit(string formName, string json)
        {
            return string.Empty;
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