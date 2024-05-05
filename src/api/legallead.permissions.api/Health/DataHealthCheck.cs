using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace legallead.permissions.api.Health
{
    [ExcludeFromCodeCoverage(Justification = "This class is tested through postman integration tests.")]
    public class DataHealthCheck : BaseServiceHealthCheck, IHealthCheck
    {
        private const string ClassContextName = "Create Data Context";

        public DataHealthCheck(IInternalServiceProvider provider) : base(provider, ClassContextName)
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
            typeof(IDataInitializer),
            typeof(IDapperCommand),
            typeof(DataContext),
            typeof(DataProvider),
        };
    }
}