using legallead.desktop.entities;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace legallead.desktop.handlers
{
    internal class InitializationCompletedHandler : JsCompletedHandler
    {
        public InitializationCompletedHandler() : base()
        {
        }

        public override void Complete(Window window, Dispatcher dispatcher, ContentControl control, string? customData = null)
        {
            var provider = AppBuilder.ServiceProvider;
            if (provider == null) return;
            var user = provider.GetRequiredService<UserBo>();
            if (user == null || !user.IsInitialized) return;
            const string target = "home";
            ContentHandler.LoadLocal(target, dispatcher, control);
        }
    }
}