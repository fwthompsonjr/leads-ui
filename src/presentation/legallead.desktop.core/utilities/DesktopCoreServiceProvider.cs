using legallead.desktop.implementations;
using legallead.desktop.interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.desktop.utilities
{
    public static class DesktopCoreServiceProvider
    {
        public static IServiceProvider Provider => _serviceProvider ??= GetProvider();
        private static IServiceProvider? _serviceProvider;

        private static IServiceProvider GetProvider()
        {
            var builder = new ServiceCollection();
            builder.AddSingleton<IContentHtmlNames, ContentHtmlNames>();
            return builder.BuildServiceProvider();
        }
    }
}