using legallead.desktop.services;
using legallead.desktop.utilities;
using System.Threading;

namespace legallead.desktop
{
    public partial class App : System.Windows.Application
    {
        private readonly BackgroundQueueServices backgroundQueue;
        private readonly BackgroundMailService mailService;

        public App()
        {
            mailService = new BackgroundMailService();
            using var source = new CancellationTokenSource();
            backgroundQueue = new BackgroundQueueServices();
            backgroundQueue.StartAsync(source.Token);
            AppBuilder.QueueService = backgroundQueue;
            AppBuilder.MailService = mailService;
        }
    }
}
