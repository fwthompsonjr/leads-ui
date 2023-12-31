using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.utilities;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace legallead.desktop.handlers
{
    internal abstract class JsCompletedHandler
    {
        protected ChromiumWebBrowser? Web { get; private set; }

        protected JsCompletedHandler(ChromiumWebBrowser? browser)
        {
            Web = browser;
            AppBuilder.Build();
        }

        public abstract void Complete(Window window, Dispatcher dispatcher, ContentControl control, string? customData = null);

        public abstract void Submit(string formName, string json);

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

        internal static string ConvertHTML(ApiResponse? response)
        {
            const int statusOk = 200;
            response ??= new ApiResponse { StatusCode = 500, Message = "An unexpected error has occurred" };
            var code = response.StatusCode;
            var statusClass = code == statusOk ? "text-success" : "text-danger";
            var borderCode = code == statusOk ? "border-secordary" : "border-danger";
            var builder = new StringBuilder($"<div class='border {borderCode} m-2 p-2 fs-5cd'>");
            var message = string.IsNullOrWhiteSpace(response.Message) ? "No data provided" : response.Message;
            builder.AppendLine();
            builder.AppendLine($"\t<span name='status-code' class='{statusClass}'> {code:D3} Status </span> <br/>");
            builder.AppendLine($"\t<span name='status-message' class='text-secondary'> {message} </span> <br/>");
            builder.AppendLine("</div>");
            return builder.ToString();
        }
    }
}