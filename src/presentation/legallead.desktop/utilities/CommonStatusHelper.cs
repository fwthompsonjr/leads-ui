using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace legallead.desktop.utilities
{
    internal class CommonStatusHelper
    {
        private readonly List<CommonMessage> messages;
        public CommonStatusHelper(CommonMessageList messageList)
        {
            messages = messageList.Messages;
        }

        public void SetStatus(CommonStatusTypes status)
        {
            try
            {
                SetStatus((int)status);
            }
            catch (Exception)
            {
                // no action to take here
            }
        }

        public void SetVersion()
        {
            try
            {
                var dispatcher = Application.Current.Dispatcher;
                Window mainWindow = dispatcher.Invoke(() => { return Application.Current.MainWindow; });
                if (mainWindow is not MainWindow main) return;
                var currentVersion = dispatcher.Invoke(() => { return main.sbVersionNumberText.Text; });
                if (currentVersion.Equals(AppProductVersion)) return;
                dispatcher = main.Dispatcher;
                dispatcher.Invoke(() =>
                {
                    main.sbVersionNumberText.Text = AppProductVersion;
                });
            }
            catch (Exception)
            {
                // no action to take
            }
        }
        private void SetStatus(int index)
        {
            var status = messages.Find(x => x.Id == index);
            if (status == null) return;
            var dispatcher = Application.Current.Dispatcher;
            Window mainWindow = dispatcher.Invoke(() => { return Application.Current.MainWindow; });
            if (mainWindow is not MainWindow main) return;
            dispatcher = main.Dispatcher;
            dispatcher.Invoke(() =>
            {
                var icon = main.sbStatusApplicationIcon;
                var appstatus = main.sbStatusApplicationText;
                var appmessage = main.sbComment;
                var appconnection = main.sbConnectionStatusText;
                if (icon == null || appstatus == null) return;
                var connection = index switch
                {
                    1 => "Unknown",
                    10 => "Connected",
                    20 => "Offline/Error",
                    30 => "Submitting",
                    40 => "Disconnected",
                    50 => "Connected",
                    _ => appconnection.Text
                };
                appconnection.Text = connection;
                var color = GetColorFromString(status.Color);
                icon.Fill = color;
                appstatus.Text = status.Name;
                if (string.IsNullOrEmpty(status.Message))
                {
                    appmessage.Visibility = Visibility.Collapsed;
                    appmessage.Text = string.Empty;
                    return;
                }
                appmessage.Visibility = Visibility.Collapsed;
                appmessage.Text = status.Message;
                appmessage.Visibility = Visibility.Visible;
            });
        }

        private static SolidColorBrush GetColorFromString(string colorString)
        {
            var fallback = Brushes.Black;
            try
            {
                ColorConverter converter = new();
                var obj = converter.ConvertFromInvariantString(colorString);
                if (obj is not Color color) return fallback;
                return new SolidColorBrush(color);
            }
            catch (Exception)
            {
                return fallback;
            }
        }
        private static string AppProductVersion => _appProductVersion ??= GetAppProductVersion();
        private static string? _appProductVersion;
        private static string GetAppProductVersion()
        {
            const char dash = '-';
            const char plus = '+';
            var mode = GetAppPaymentMode();
            string fallback = string.Concat("3.2.10", mode);
            var process = Process.GetCurrentProcess();
            if (process == null) return fallback;
            string processExe = process.MainModule?.ModuleName ?? string.Empty;
            if (string.IsNullOrEmpty(processExe) || !File.Exists(processExe)) return fallback;
            var versionInfo = FileVersionInfo.GetVersionInfo(processExe);
            if (versionInfo == null) return fallback;
            var product = versionInfo.ProductVersion;
            if (string.IsNullOrEmpty(product) || !product.Contains(dash)) return fallback;
            var parsed = product.Split(dash);
            if (parsed.Length < 2) return fallback;
            var item = parsed[1];
            if (!item.Contains(plus)) return fallback;
            var revised = item.Split(plus)[0];
            return string.Concat(revised, mode);
        }
        private static string GetAppPaymentMode()
        {
            const string test = " (TEST)";
            var api = AppBuilder.ServiceProvider?.GetService<IPermissionApi>();
            if (api == null) return test;
            var getpayment = api.Get("payment-process-type").GetAwaiter().GetResult();
            if (getpayment == null || getpayment.StatusCode != 200) return test;
            var model = JsonConvert.DeserializeObject<PaymentProcessModel>(getpayment.Message);
            if (model == null) return test;
            return model.IsLive ? string.Empty : test;
        }
    }
}
