using legallead.permissions.api.Controllers;
using legallead.Profiles.api.Controllers;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace legallead.permissions.api.Health
{
    [ExcludeFromCodeCoverage(Justification = "This class is tested through postman integration tests.")]
    public class ControllerHealthCheck(IInternalServiceProvider provider) : BaseServiceHealthCheck(provider, ClassContextName), IHealthCheck
    {
        private const string ClassContextName = "Create Controller Context";

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
            typeof(ApplicationController),
            typeof(EventsController),
            typeof(HomeController),
            typeof(ListsController),
            typeof(PaymentController),
            typeof(PermissionsController),
            typeof(ProfilesController),
            typeof(SearchController),
            typeof(SignonController),
        };
    }
}