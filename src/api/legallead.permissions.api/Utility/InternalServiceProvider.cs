namespace legallead.permissions.api
{
    public class InternalServiceProvider(IServiceCollection collection) : IInternalServiceProvider
    {
        private readonly IServiceProvider _serviceProvider = collection.BuildServiceProvider();

        public IServiceProvider ServiceProvider => _serviceProvider;
    }
}