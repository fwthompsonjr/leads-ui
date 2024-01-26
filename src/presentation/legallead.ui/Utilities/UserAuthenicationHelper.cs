using legallead.desktop.entities;
using legallead.desktop.utilities;
using legallead.ui.Models;

namespace legallead.ui.Utilities
{
    internal static class UserAuthenicationHelper
    {
        public static void AuthenicationCompleted(string token, string username = "")
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
            if (!string.IsNullOrEmpty(username))
            {
                user.UserName = username;
            }
            user.Token = new AccessTokenBo { AccessToken = token, RefreshToken = token, Expires = GetExpirationDate() };
            model.IsMyAccountVisible = true;
            model.IsMySearchVisible = true;
            main.BindableToolbars?.ToList().ForEach(toolbar =>
            {
                toolbar.IsEnabled = true;
                toolbar.StyleId = "EnabledLink";
            });
        }

        public static void LogoutRequested()
        {
            var main = MainPageFinder.GetMain();
            if (main == null) return;
            var provider = AppBuilder.ServiceProvider;
            var user = provider?.GetService<UserBo>();
            var model = GetModel(main);
            if (provider == null || user == null || model == null) return;
            user.Token = null;
            model.IsMyAccountVisible = false;
            model.IsMySearchVisible = false;
            main.BindableToolbars?.ToList().ForEach(toolbar =>
            {
                toolbar.IsEnabled = false;
                toolbar.StyleId = "DisabledLink";
            });
        }

        private static MainWindowViewModel? GetModel(MainPage main)
        {
            if (main.BindingContext is MainWindowViewModel viewModel) return viewModel;
            return null;
        }

        private static DateTime GetExpirationDate()
        {
            return DateTime.UtcNow.AddMinutes(ExpirationInterval());
        }

        private static int ExpirationInterval()
        {
            const int fallback = 30;
            if (sessionTimeout.HasValue) return sessionTimeout.Value;
            var configuration = AppBuilder.Configuration;
            if (configuration == null)
            {
                sessionTimeout = fallback;
                return fallback;
            }

            var timeoutString = configuration["user:session_timeout"];
            if (!int.TryParse(timeoutString, out var timeout))
            {
                sessionTimeout = fallback;
                return fallback;
            }
            sessionTimeout = timeout;
            return timeout;
        }
        private static int? sessionTimeout;
    }
}
