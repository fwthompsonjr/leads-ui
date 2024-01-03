using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace legallead.desktop.helpers
{
    internal static class PermissionsHelper
    {
        public static async Task<string> GetContent(string html)
        {
            var provider = AppBuilder.ServiceProvider;
            var user = provider?.GetService<UserBo>();
            var api = provider?.GetService<IPermissionApi>();
            var service = provider?.GetService<IUserPermissionsMapper>();
            if (api == null || user == null || service == null) return html;
            try
            {
                var resp = await service.Map(api, user, html);
                return resp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return html;
            }
        }
    }
}