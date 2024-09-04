using legallead.jdbc.interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace legallead.permissions.api.Health
{
    [ExcludeFromCodeCoverage(Justification = "This class is tested through postman integration tests.")]
    public class RepositoryHealthCheck(IInternalServiceProvider provider) : BaseServiceHealthCheck(provider, ClassContextName), IHealthCheck
    {
        private const string ClassContextName = "Create Repository Context";

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
            typeof(IJwtManagerRepository),
            typeof(IComponentRepository),
            typeof(IPermissionMapRepository),
            typeof(IProfileMapRepository),
            typeof(IUserPermissionRepository),
            typeof(IUserProfileRepository),
            typeof(IUserTokenRepository),
            typeof(IUserProfileViewRepository),
            typeof(IUserPermissionViewRepository),
            typeof(IPermissionGroupRepository),
            typeof(IUserRepository),
            typeof(IUserPermissionHistoryRepository),
        };
    }
}