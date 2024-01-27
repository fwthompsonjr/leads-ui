using legallead.ui.implementations;
using legallead.ui.interfaces;

namespace legallead.ui
{
    public partial class MainPage
    {
        private static readonly object lockobj = new();

        private void MainWebViewer_Navigating(object? sender, WebNavigatingEventArgs e)
        {
            lock (lockobj)
            {
                var url = e.Url;
                if (string.IsNullOrEmpty(url)) { return; }
                if (url.EndsWith('#'))
                {
                    e.Cancel = true;
                    return;
                }
                if (!url.Contains(InternalDomain)) return;
                e.Cancel = true;
                var pieces = url.Split('/');
                var componentName = $"{pieces[3]}-{pieces[4]}";
                var navigator = NavProvider.GetKeyedService<IPageNavigator>(componentName);
                navigator?.Submit(url);
            }
        }




        private static ServiceProvider NavProvider => navServices ??= GetNavigations();
        private static ServiceProvider? navServices;
        private static ServiceProvider GetNavigations()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<IPageNavigator, PageNavigatorHome>("home-form-login-submit");
            services.AddKeyedSingleton<IPageNavigator, PageNavigatorRegistration>("home-form-register-submit");
            services.AddKeyedSingleton<IPageNavigator, PageNavigatorLogout>("user-logout");
            services.AddKeyedSingleton<IPageNavigator, PageNavigatorLogout>("myaccount-logout");
            services.AddKeyedSingleton<IPageNavigator, PageNavigatorGetSession>("user-session");
            services.AddKeyedSingleton<IPageNavigator, ReAuthorizeNavigator>("user-re-authorize");
            return services.BuildServiceProvider();
        }
    }
}
