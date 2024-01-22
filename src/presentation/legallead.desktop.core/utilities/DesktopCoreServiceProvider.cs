using legallead.desktop.entities;
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
            builder.AddSingleton<IErrorContentProvider, ErrorContentProvider>();
            builder.AddScoped<IContentParser, ContentParser>();
            builder.AddSingleton<IInternetStatus, InternetStatus>();
            builder.AddSingleton<IUserProfileMapper, UserProfileMapper>();
            builder.AddSingleton<IUserPermissionsMapper, UserPermissionsMapper>();
            builder.AddSingleton<ICopyrightBuilder>(new CopyrightBuilder());
            builder.AddSingleton(new CommonMessageList());
            var menucontent = Properties.Resources.contextmenu;
            if (string.IsNullOrEmpty(menucontent)) return builder.BuildServiceProvider();
            builder.AddSingleton(s =>
            {
                return ObjectExtensions.TryGet<MenuConfiguration>(menucontent);
            });
            return builder.BuildServiceProvider();
        }
    }
}