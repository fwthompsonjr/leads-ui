using legallead.desktop.utilities;
using Newtonsoft.Json;
using System;
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

        public abstract string Submit(string formName, string json);

        protected static T? TryDeserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}