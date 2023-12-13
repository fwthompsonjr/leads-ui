using legallead.content.helpers;
using legallead.content.interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace legallead.content
{
    public class ContentDbServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ContentDbServiceProvider()
        {
            var builder = new ServiceCollection();
            builder.AddScoped<IContentDbCommand, ContentDbExecutor>();
            _serviceProvider = builder.BuildServiceProvider();
            // this class needs to use microsoft di package not the extensions package.
        }

        public IServiceProvider Provider => _serviceProvider;
    }
}