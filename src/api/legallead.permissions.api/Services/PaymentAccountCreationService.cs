namespace legallead.permissions.api.Services
{
    public class PaymentAccountCreationService(ILogger<PaymentAccountCreationService> log, ICustomerInfrastructure infrastructure, bool isTest = false) : BackgroundService
    {
        private readonly ICustomerInfrastructure _customerInfrastructure = infrastructure;
        private readonly ILogger<PaymentAccountCreationService> logger = log;
        private readonly bool _isTestMode = isTest;

        [ExcludeFromCodeCoverage(Justification = "This process directly interacts with data services and is for integration testing only.")]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Queued Hosted Service is running.");
            await BackgroundProcessing(stoppingToken);
        }

        [ExcludeFromCodeCoverage(Justification = "This process directly interacts with data services and is for integration testing only.")]
        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                if (stoppingToken.IsCancellationRequested) break;
                await _customerInfrastructure.MapCustomers();
                if (!_isTestMode) await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
