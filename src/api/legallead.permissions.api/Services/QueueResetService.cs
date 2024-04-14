using legallead.jdbc.interfaces;
using System.Diagnostics.CodeAnalysis;

namespace legallead.permissions.api.Services
{
    public class QueueResetService : BackgroundService
    {
        private readonly ILogger<QueueResetService> _logger;

        public QueueResetService(IUserSearchRepository db,
            ILogger<QueueResetService> logger)
        {
            _logger = logger;
            QueueDb = db;
        }

        protected IUserSearchRepository QueueDb { get; }

        [ExcludeFromCodeCoverage(Justification = "This process directly interacts with data services and is for integration testing only.")]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is running.");
            await BackgroundProcessing(stoppingToken);
        }

        [ExcludeFromCodeCoverage(Justification = "This process directly interacts with data services and is for integration testing only.")]
        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ResetItems(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing ResetItems");
                }
            }
        }

        [ExcludeFromCodeCoverage(Justification = "This process directly interacts with data services and is for integration testing only.")]
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(cancellationToken);
        }

        [ExcludeFromCodeCoverage(Justification = "This process directly interacts with data services and is for integration testing only.")]
        private async Task ResetItems(CancellationToken stoppingToken)
        {
            _ = await Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                var status = await QueueDb.RequeueSearches();
                return status;
            });
        }

    }
}
