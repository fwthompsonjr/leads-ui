using legallead.jdbc.interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace legallead.permissions.api.Health
{
    [ExcludeFromCodeCoverage]
    public class RepositoryHealthCheck : IHealthCheck
    {
        private readonly IInternalServiceProvider _provider;

        public RepositoryHealthCheck(IInternalServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await Task.Run(() =>
                {
                    var result = new List<HealthCheckResult>();
                    types.ForEach(type =>
                    {
                        var obj = _provider.ServiceProvider.GetService(type);
                        if (obj != null)
                        {
                            result.Add(HealthCheckResult.Healthy());
                        }
                        else
                        {
                            var message = $"Unable to create repository type: {type.Name}";
                            result.Add(HealthCheckResult.Unhealthy(message));
                        }
                    });
                    return result;
                });
                var unhealthy = response.Exists(x => x.Status == HealthStatus.Unhealthy);
                if (unhealthy) return response.Find(x => x.Status == HealthStatus.Unhealthy);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    description: "Create Repository Context",
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