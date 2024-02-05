using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;

namespace legallead.ui.Utilities
{
    internal class MyAccountContentLoadHandler() : ContentLoadBase
    {
        public void SetHome()
        {
            var homepage = Task.Run(async () =>
            {
                var content = GetHTML("myaccount");
                var builder = new MyAccountSettings(content);
                await builder.Build();
                return builder.Content;
            }).Result;
            SetView(homepage);
        }

        protected class MyAccountSettings(string content)
        {
            public async Task Build()
            {
                await MapProfileResponse();
                await MapPermissionsResponse();
            }

            public string Content { get; private set; } = content;


            private async Task MapProfileResponse()
            {
                string response = Content;
                var provider = AppBuilder.ServiceProvider;
                var user = provider?.GetService<UserBo>();
                var api = provider?.GetService<IPermissionApi>();
                var service = provider?.GetService<IUserProfileMapper>();
                if (api == null || user == null || service == null) return;
                try
                {
                    var resp = await service.Map(api, user, response);
                    Content = resp;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            private async Task MapPermissionsResponse()
            {
                string response = Content;
                var provider = AppBuilder.ServiceProvider;
                var user = provider?.GetService<UserBo>();
                var api = provider?.GetService<IPermissionApi>();
                var service = provider?.GetService<IUserPermissionsMapper>();
                if (api == null || user == null || service == null) return;
                try
                {
                    var resp = await service.Map(api, user, response);
                    Content = resp;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

    }
}
