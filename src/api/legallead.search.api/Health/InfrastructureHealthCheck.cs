using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.logging.interfaces;
using legallead.search.api.Controllers;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace legallead.search.api.Health
{
    [ExcludeFromCodeCoverage]
    public class InfrastructureHealthCheck : BaseServiceHealthCheck, IHealthCheck
    {
        private const string ClassContextName = "Create Infrastructure Context";

        public InfrastructureHealthCheck(IServiceProvider provider) : base(provider, ClassContextName)
        {
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await CanCreate(types);
                return response;
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    description: ClassContextName,
                    exception: ex);
            }
        }

        private static readonly List<Type> types = new()
        {
            typeof(DataContext),
            typeof(IDapperCommand),
            typeof(ApiController),
            typeof(ILogConfiguration),
            typeof(ILoggingService),
            typeof(ILoggingRepository),
            typeof(ISearchQueueRepository),
            typeof(IBgComponentRepository),
            typeof(IBackgroundServiceSettings),
            typeof(IExcelGenerator)
        };
    }
}