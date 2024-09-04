using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace legallead.permissions.api.Health
{
    [ExcludeFromCodeCoverage(Justification = "This class is tested through postman integration tests.")]
    public class InfrastructureHealthCheck(IInternalServiceProvider provider) : BaseServiceHealthCheck(provider, ClassContextName), IHealthCheck
    {
        private const string ClassContextName = "Create Infrastructure Context";

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await CanCreateAsync(types);
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
            typeof(ICustomerInfrastructure),
            typeof(IJwtManagerRepository),
            typeof(ILoggingInfrastructure),
            typeof(IPaymentHtmlTranslator),
            typeof(IProfileInfrastructure),
            typeof(IRequestedUser),
            typeof(ISearchInfrastructure),
            typeof(IStateSearchProvider),
            typeof(IStripeInfrastructure),
            typeof(ISubscriptionInfrastructure),
            typeof(IUserSearchValidator)
        };
    }
}