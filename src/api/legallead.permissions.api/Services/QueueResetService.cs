using legallead.jdbc.interfaces;

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

        private IUserSearchRepository QueueDb { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is running.");
            await BackgroundProcessing(stoppingToken);
        }

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

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }

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
