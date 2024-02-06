using legallead.permissions.api.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace legallead.permissions.api.Health
{
    public abstract class BaseServiceHealthCheck
    {
        protected readonly IInternalServiceProvider _provider;
        protected readonly string _contextName;

        protected BaseServiceHealthCheck(
            IInternalServiceProvider provider,
            string contextName)
        {
            _provider = provider;
            _contextName = contextName;
        }

        protected async Task<HealthCheckResult> CanCreate(List<Type> types)
        {
            var response = await Task.Run(() =>
            {
                var result = new List<HealthCheckResult>();
                types.ForEach(type =>
                {
                    var obj = _provider.ServiceProvider.GetService(type);
                    if (obj != null)
                    {
                        result.Add(
                            HealthCheckResult.Healthy($"{type.Name} retrieved from service provider."));
                    }
                    else
                    {
                        var message = $"{type.Name} could not be created from service provider.";
                        result.Add(HealthCheckResult.Unhealthy(message));
                    }
                });
                return result;
            });
            return GetResult(response);
        }

        private HealthCheckResult GetResult(List<HealthCheckResult> response)
        {
            var title = _contextName;
            var unhealthy = response.Exists(x => x.Status == HealthStatus.Unhealthy);
            var data = response.Select((s, id) => new { id, description = s.Description ?? string.Empty }).ToList();
            var list = new Dictionary<string, object>();
            data.ForEach(d => list.Add(d.id.ToString(), d.description));
            IReadOnlyDictionary<string, object> dictionary =
                new Dictionary<string, object>(list);

            return unhealthy ?
                HealthCheckResult.Unhealthy(title, data: dictionary) :
                HealthCheckResult.Healthy(title, dictionary);
        }
    }
}