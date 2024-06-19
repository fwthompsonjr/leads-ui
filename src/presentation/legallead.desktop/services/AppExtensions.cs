using legallead.desktop.services;
using legallead.desktop.utilities;
using System.Threading;

namespace legallead.desktop
{
    public partial class App : System.Windows.Application
    {
        private readonly BackgroundQueueServices backgroundQueue;
        private readonly BackgroundMailService mailService;
        private readonly BackgroundHistoryService historyService;
        public App()
        {
            mailService = new BackgroundMailService();
            historyService = new();
            using var source = new CancellationTokenSource();
            backgroundQueue = new BackgroundQueueServices();
            backgroundQueue.StartAsync(source.Token);
            AppBuilder.QueueService = backgroundQueue;
            AppBuilder.MailService = mailService;
            AppBuilder.HistoryService = historyService;
        }
    }
}
