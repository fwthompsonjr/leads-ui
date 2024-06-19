using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isBlankLoaded = false;
        private CommonStatusHelper? StatusHelper;
        public MainWindow()
        {
            InitializeComponent();
            SetupStatus();
            InitializeBrowserContent();
            InitializeErrorContent();
            InitializeUserChanged();
            ContentRendered += MainWindow_ContentRendered;
            Closing += MainWindow_Closing;
            mnuExit.Click += MnuExit_Click;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var provider = AppBuilder.ServiceProvider;
            var manager = provider?.GetService<IMailPersistence>();
            var mailService = AppBuilder.MailService;
            var historyService = AppBuilder.HistoryService;
            var backgroundQueue = AppBuilder.QueueService;
            mailService?.Dispose();
            historyService?.Dispose();
            manager?.Clear();
            if (backgroundQueue == null) { return; }
            using var source = new CancellationTokenSource();
            backgroundQueue.StopAsync(source.Token);
        }

        private BrowserHelper GetHelper()
        {
            var window = (Window)this;
            return new BrowserHelper(window);
        }

        private void MainWindow_ContentRendered(object? sender, System.EventArgs e)
        {
            if (isBlankLoaded) { return; }
            try
            {
                Task.Run(() =>
                {
                    Thread.Sleep(500);
                    var initialPage = AppBuilder.InitialViewName ?? "introduction";
                    var helper = GetHelper();
                    helper.Load(initialPage, Dispatcher, content1);
                    helper.Handler?.OnPageLoaded();
                }).ConfigureAwait(false);
            }
            finally
            {
                isBlankLoaded = true;
            }
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow_Closing(sender, new());
            Environment.Exit(0);
        }

        private void MnuDefault_Click(object sender, RoutedEventArgs e)
        {
            const string logoutCommand = "myaccount-logout";
            if (sender is not MenuItem mnu) return;
            if (mnu.Tag is not string mnuCommand) return;
            if (string.IsNullOrEmpty(mnuCommand)) return;
            if (mnuCommand.Equals(logoutCommand))
            {
                var user = AppBuilder.ServiceProvider?.GetRequiredService<UserBo>();
                if (user == null) return;
                user.Token = null;
                return;
            }
            NavigateChild(mnuCommand);
        }

        private void MnuHome_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem _) return;
            tabHome.IsSelected = true;
        }

        private void MnuError_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem item) return;
            var menuId = Convert.ToInt32(item.Name.Replace("mnu", ""));
            if (menuId == 0) return;
            tabError.IsSelected = true;
            SetErrorContent(menuId);
        }

        internal string? NavigateTo(string destination, int errorCode = 0)
        {
            var landing = Landings.Find(x => x.Equals(destination, StringComparison.OrdinalIgnoreCase));
            if (landing == null) return null;
            switch (landing)
            {
                case "home":
                    Dispatcher.Invoke(() =>
                    {
                        InitializeBrowserContent();
                        tabHome.IsSelected = true;
                    });
                    break;

                case "error":
                    Dispatcher.Invoke(() =>
                    {
                        SetErrorContent(errorCode);
                        tabError.IsSelected = true;
                    });
                    break;

                case "myaccount":
                    NavigateToMyAccount();
                    break;

                case "mysearch":
                    NavigateToMySearch();
                    break;
                case "mailbox":
                    NavigateToMyMailBox();
                    return string.Empty;

                case "exit":
                    Environment.Exit(0);
                    break;
            }
            return landing;
        }
    }
}