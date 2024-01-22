using legallead.ui.interfaces;
using System.Windows.Input;

namespace legallead.ui.implementations
{
    internal class MenuItemClickedCommand : ICommand
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
            service?.Click(null, null);
        }

        private static ServiceProvider MenuProvider => menuServices ??= GetServices();
        private static ServiceProvider? menuServices;
        private static ServiceProvider GetServices()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<IMenuClickHandler, ExitMenuClicked>("menuExit");
            return services.BuildServiceProvider();
        }
    }
}
