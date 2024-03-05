using legallead.permissions.api.Interfaces;

namespace legallead.permissions.api
{
    public class PaymentAccountCreationService : BackgroundService
    {
        private readonly ICustomerInfrastructure _customerInfrastructure;
        private readonly ILogger<PaymentAccountCreationService> logger;
        private readonly bool _isTestMode;
        public PaymentAccountCreationService(ILogger<PaymentAccountCreationService> log, ICustomerInfrastructure infrastructure, bool isTest = false)
        {
            _customerInfrastructure = infrastructure;
            logger = log;
            _isTestMode = isTest;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Queued Hosted Service is running.");
            await BackgroundProcessing(stoppingToken);
        }

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
