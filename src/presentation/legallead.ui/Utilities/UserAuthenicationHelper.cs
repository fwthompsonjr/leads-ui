using legallead.desktop.entities;
using legallead.desktop.utilities;
using legallead.ui.Models;

namespace legallead.ui.Utilities
{
    internal static class UserAuthenicationHelper
    {
        public static void AuthenicationCompleted(string token)
        {
            var main = MainPageFinder.GetMain();
            if (main == null) return;
            if (string.IsNullOrEmpty(token)) { return; }
            var provider = AppBuilder.ServiceProvider;
            var user = provider?.GetService<UserBo>();
            var model = GetModel(main);
            if (provider == null) return;
            if (string.IsNullOrEmpty(token) ||
                user == null ||
                model == null) return;
            user.Token = new AccessTokenBo { AccessToken = token, RefreshToken = token, Expires = DateTime.UtcNow.AddMinutes(60) };
            model.IsMyAccountVisible = true;
            model.IsMySearchVisible = true;
            
        }

        private static MainWindowViewModel? GetModel(MainPage main)
        {
            if (main.BindingContext is MainWindowViewModel viewModel) return viewModel;
            return null;
        }
    }
}
