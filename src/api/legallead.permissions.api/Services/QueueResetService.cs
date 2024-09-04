using legallead.jdbc.interfaces;

namespace legallead.permissions.api.Services
{
    [ExcludeFromCodeCoverage(Justification = "This process directly interacts with data services and is for integration testing only.")]
    public class QueueResetService(IUserSearchRepository db,
        ILogger<QueueResetService> logger) : BackgroundService
    {
        private readonly ILogger<QueueResetService> _logger = logger;

        protected IUserSearchRepository QueueDb { get; } = db;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is running.");
            await BackgroundProcessingAsync(stoppingToken);
        }

        private async Task BackgroundProcessingAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ResetItemsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing ResetItems");
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(cancellationToken);
        }

        private async Task ResetItemsAsync(CancellationToken stoppingToken)
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
