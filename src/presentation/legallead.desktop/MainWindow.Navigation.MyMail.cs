using legallead.desktop.entities;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {


        internal void NavigateToMyMailBox()
        {
            var user = AppBuilder.ServiceProvider?.GetRequiredService<UserBo>();
            if (user == null || !user.IsAuthenicated)
            {
                Dispatcher.Invoke(() =>
                {
                    SetErrorContent(401);
                    tabError.IsSelected = true;
                });
                return;
            }
            Dispatcher.Invoke(() =>
            {
                tabMySearch.IsSelected = true;
            });
        }
    }
}