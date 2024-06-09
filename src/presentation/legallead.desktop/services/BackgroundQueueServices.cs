using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace legallead.desktop.services
{
    internal class BackgroundQueueServices : IHostedService
    {
        private readonly IQueueStarter? _queueStarter;
        private readonly IQueueStopper? _queueStopper;
        private readonly IQueueFilter? _filterService;
        private Timer? _timer = null;
        public BackgroundQueueServices()
        {
            var provider = DesktopCoreServiceProvider.Provider;
            if (provider == null) return;
            _queueStarter = provider.GetService<IQueueStarter>();
            _queueStopper = provider.GetService<IQueueStopper>();
            _filterService = provider.GetService<IQueueFilter>();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _filterService?.Clear();
            _timer = new Timer(OnTimer, null,
                TimeSpan.FromSeconds(15d),
                TimeSpan.FromMinutes(3d));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _timer?.Change(Timeout.Infinite, 0);
            _queueStopper?.Stop();
            return Task.CompletedTask;
        }

        private void OnTimer(object? state)
        {
            if (_queueStarter == null) return;
            _queueStarter.Start();
        }
    }
}
