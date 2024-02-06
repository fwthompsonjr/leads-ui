using legallead.permissions.api.Interfaces;

namespace legallead.permissions.api
{
    public class InternalServiceProvider : IInternalServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public InternalServiceProvider(IServiceCollection collection)
        {
            _serviceProvider = collection.BuildServiceProvider();
        }

        public IServiceProvider ServiceProvider => _serviceProvider;
    }
}