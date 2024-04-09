using legallead.desktop.entities;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
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
            var user = AppBuilder.ServiceProvider?.GetRequiredService<UserBo>();
            if (user == null) return;
            user.AuthenicatedChanged = () =>
            {
                var menus = Dispatcher.Invoke(() => { return new[] { mnuMyAccount, mnuMySearch }.ToList(); });
                if (user.IsAuthenicated)
                {
                    Dispatcher.Invoke(() =>
                    {
                        menus.ForEach(m => { m.Visibility = Visibility.Visible; });
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        // navigate to home
                        NavigateChild("home-login");
                        menus.ForEach(m => { m.Visibility = Visibility.Hidden; });
                    });
                }
            };
        }
    }
}