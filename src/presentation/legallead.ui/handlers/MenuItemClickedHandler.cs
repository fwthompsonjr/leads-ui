using legallead.ui.interfaces;
using System.Windows.Input;

namespace legallead.ui.implementations
{
    internal class MenuItemClickedHandler : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is not string menuItem) return;
            var service = MenuProvider.GetKeyedService<IMenuClickHandler>(menuItem);
            service?.Click();
        }

        private static ServiceProvider MenuProvider => menuServices ??= GetServices();
        private static ServiceProvider? menuServices;
        private static ServiceProvider GetServices()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<IMenuClickHandler, ExitMenuClicked>("menuExit");
            services.AddKeyedSingleton<IMenuClickHandler, HomeMenuClicked>("menuHome");
            services.AddKeyedSingleton<IMenuClickHandler, MyAccountMenuClicked>("myaccount-home");
            services.AddKeyedSingleton<IMenuClickHandler, MyAccountPermissionsMenuClicked>("myaccount-permissions");
            services.AddKeyedSingleton<IMenuClickHandler, MyAccountProfileMenuClicked>("myaccount-profile");
            services.AddKeyedSingleton<IMenuClickHandler, MyLogoutMenuClicked>("myaccount-logout");
            services.AddKeyedSingleton<IMenuClickHandler, MySearchMenuClicked>("mysearch-home");
            services.AddKeyedSingleton<IMenuClickHandler, MySearchHistoryMenuClicked>("mysearch-history");
            services.AddKeyedSingleton<IMenuClickHandler, MySearchPurchasesMenuClicked>("mysearch-purchases");
            return services.BuildServiceProvider();
        }
    }
}
