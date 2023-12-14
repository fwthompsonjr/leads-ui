using legallead.content.helpers;
using legallead.content.implementations;
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
            builder.AddScoped<IWebContentLineRepository, WebContentLineRepository>();
            builder.AddScoped<IWebContentRepository, WebContentRepository>();
            _serviceProvider = builder.BuildServiceProvider();
        }

        public IServiceProvider Provider => _serviceProvider;
    }
}