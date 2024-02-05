using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;
using legallead.Profiles.api.Controllers;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace legallead.permissions.api.Health
{
    [ExcludeFromCodeCoverage]
    public class ControllerHealthCheck : BaseServiceHealthCheck, IHealthCheck
    {
        private const string ClassContextName = "Create Controller Context";

        public ControllerHealthCheck(IInternalServiceProvider provider) : base(provider, ClassContextName)
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
            typeof(HomeController),
            typeof(ApplicationController),
            typeof(ListsController),
            typeof(PermissionsController),
            typeof(ProfilesController),
            typeof(SignonController),
            typeof(SearchController),
        };
    }
}