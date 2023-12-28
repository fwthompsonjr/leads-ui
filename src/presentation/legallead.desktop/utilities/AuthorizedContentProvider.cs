using legallead.desktop.entities;
using legallead.desktop.extensions;
using legallead.desktop.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace legallead.desktop.utilities
{
    internal static class AuthorizedContentProvider
    {
        public static async Task<string> GetContent(string pageName)
        {
            var provider = DesktopCoreServiceProvider.Provider;
            var user = provider.GetService<UserBo>();
            if (user == null || !user.IsAuthenicated) { return GetErrorContent(401); }
            var api = provider.GetService<IPermissionApi>();
            if (api == null) { return GetErrorContent(503); }
            await user.ExtendToken(api);
            var response = await api.Get(pageName);
            if (response == null || response.StatusCode != 200)
            {
                int code = response?.StatusCode ?? 500;
                return GetErrorContent(code);
            }
            return response.Message;
        }

        private static string GetErrorContent(int statusCode)
        {
            var provider = DesktopCoreServiceProvider.Provider;
            var errorProvider = provider?.GetService<IErrorContentProvider>();
            var content = errorProvider?.GetContent(statusCode)?.Content;
            content ??= "<html><body>An unexpected error has occured.</body></html>";
            return content;
        }
    }
}