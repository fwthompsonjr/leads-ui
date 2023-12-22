using Microsoft.Extensions.DependencyInjection;

namespace legallead.logging
{
    internal static class InternalDbServiceProvider
    {
        public static T GetService<T>()
        {
            return (T)Provider.GetRequiredService(typeof(T));
        }

        private static IServiceProvider Provider => loggingDbServiceProvider.Provider;
        private static readonly LoggingDbServiceProvider loggingDbServiceProvider = new();
    }
}