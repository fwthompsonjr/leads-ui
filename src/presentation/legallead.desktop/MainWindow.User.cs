using CefSharp.DevTools.Network;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private void InitializeUserChanged()
        {
            const string dash = "-";
            var user = AppBuilder.ServiceProvider?.GetRequiredService<UserBo>();
            if (user == null) return;
            user.AuthenicatedChanged = () =>
            {
                var menus = Dispatcher.Invoke(() => { return new[] { mnuMyAccount, mnuMySearch, mnuMyMailbox }.ToList(); });
                if (user.IsAuthenicated)
                {
                    Dispatcher.Invoke(() =>
                    {
                        menus.ForEach(m => { m.Visibility = Visibility.Visible; });
                    });
                    Dispatcher.InvokeAsync(async () =>
                    {
                        var api = AppBuilder.ServiceProvider?.GetService<IPermissionApi>();
                        if (api == null) return;
                        var permissions = await api.Get("user-permissions-list", user);
                        if (permissions == null || permissions.StatusCode != 200)
                        {
                            sbUserLevelText.Text = dash;
                            return;
                        }
                        var mapped = ObjectExtensions.TryGet<List<ContactPermissionResponse>>(permissions.Message);
                        var current = mapped.Find(x => x.KeyName.Equals("Account.Permission.Level"));
                        var level = current?.KeyValue?.ToUpper() ?? dash;
                        sbUserLevelText.Text = level;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        // navigate to home
                        NavigateChild("home-login");
                        menus.ForEach(m => { m.Visibility = Visibility.Hidden; });
                        sbUserLevelText.Text = dash;
                    });
                }
            };
        }
    }
}