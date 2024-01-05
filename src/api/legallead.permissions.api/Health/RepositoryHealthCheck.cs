using legallead.jdbc.interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace legallead.permissions.api.Health
{
    [ExcludeFromCodeCoverage]
    public class RepositoryHealthCheck : BaseServiceHealthCheck, IHealthCheck
    {
        private const string ClassContextName = "Create Repository Context";

        public RepositoryHealthCheck(IInternalServiceProvider provider) : base(provider, ClassContextName)
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