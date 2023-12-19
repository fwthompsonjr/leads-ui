using legallead.desktop.utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace legallead.desktop.handlers
{
    internal abstract class JsCompletedHandler
    {
        protected JsCompletedHandler()
        {
            AppBuilder.Build();
        }

        public abstract void Complete(Window window, Dispatcher dispatcher, ContentControl control, string? customData = null);
    }
}