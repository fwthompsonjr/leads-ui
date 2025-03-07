﻿using legallead.jdbc.interfaces;
using legallead.permissions.api.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace legallead.permissions.api.Health
{
    [ExcludeFromCodeCoverage(Justification = "This class is tested through postman integration tests.")]
    public class PricingHealthCheck(IInternalServiceProvider provider) : BaseServiceHealthCheck(provider, ClassContextName), IHealthCheck
    {
        private const string ClassContextName = "Pricing Synchronization Service";
        private readonly IPricingRepository? _pricingInfrastructure = provider.ServiceProvider.GetService<IPricingRepository>();

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                if (_pricingInfrastructure == null)
                {
                    return HealthCheckResult.Unhealthy(description: $"{ClassContextName} : IPricingRepository is null");
                }
                var items = await _pricingInfrastructure.GetPricingTemplates();
                if (items == null || items.Count == 0) return HealthCheckResult.Degraded(description: $"{ClassContextName} : GetPricingTemplates returns no items");
                var count = items.Count(x => x.IsActive.GetValueOrDefault());
                var mapped = PricingLookupService.PricingCodes.Count;
                return HealthCheckResult.Healthy(description: $"{ClassContextName} : GetPricingTemplates items OK. Expected {count}, Actual {mapped}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    description: ClassContextName,
                    exception: ex);
            }
        }
    }
}
