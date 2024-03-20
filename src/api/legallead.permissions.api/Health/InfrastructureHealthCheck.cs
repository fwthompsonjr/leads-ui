using legallead.permissions.api.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace legallead.permissions.api.Health
{
    [ExcludeFromCodeCoverage]
    public class InfrastructureHealthCheck : BaseServiceHealthCheck, IHealthCheck
    {
        private const string ClassContextName = "Create Infrastructure Context";

        public InfrastructureHealthCheck(IInternalServiceProvider provider) : base(provider, ClassContextName)
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