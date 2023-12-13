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
            builder.AddScoped<ContentDbContext>();
            _serviceProvider = builder.BuildServiceProvider();
        }

        public IServiceProvider Provider => _serviceProvider;
    }
}