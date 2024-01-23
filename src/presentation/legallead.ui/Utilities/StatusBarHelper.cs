using legallead.desktop.utilities;
using legallead.ui.Models;
using Microsoft.Maui.Graphics.Converters;
using Mmg = Microsoft.Maui.Graphics;

namespace legallead.ui.Utilities
{
    internal static class StatusBarHelper
    {

        public static void SetStatus(int statusId)
        {
            var model = Provider?.GetService<MainWindowViewModel>();
            var messages = Provider?.GetService<CommonMessageList>()?.Messages;
            var mainPage = MainPageFinder.GetMain();
            if (mainPage == null || model == null || messages == null) return;
            var status = messages.Find(x => x.Id == statusId);
            if (status == null) return;
            mainPage.Dispatcher?.Dispatch(() =>
            {
                mainPage.StatusIcon.Background = GetColorFromString(status.Color);
                mainPage.StatusText.Text = status.Name;
                mainPage.StatusMessage.Text = status.Message;
                var current = mainPage.StatusConnection.Text;
                mainPage.StatusConnection.Text = status.Id switch
                {
                    1 => "Offline",
                    10 => "Connected",
                    _ => current
                };
            });
        }
        private static IServiceProvider? Provider => AppBuilder.ServiceProvider;
        private static SolidColorBrush GetColorFromString(string colorString)
        {
            var fallback = Brush.Black;
            try
            {
                ColorTypeConverter converter = new();
                var obj = converter.ConvertFromInvariantString(colorString);
                if (obj is not Mmg.Color color) return fallback;
                return new SolidColorBrush(color);
            }
            catch (Exception)
            {
                return fallback;
            }
        }
    }
}
