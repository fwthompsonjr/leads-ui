using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api
{
    public static class ObjectExtensions
    {
        public static void RegisterDataServices(this IServiceCollection services)
        {
            services.AddScoped<IDapperCommand, DapperExecutor>();
            services.AddScoped<DataContext>();
            services.AddScoped<IComponentRepository, ComponentRepository>();
            services.AddScoped<IPermissionMapRepository, PermissionMapRepository>();
            services.AddScoped<IProfileMapRepository, ProfileMapRepository>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped(d =>
            {
                var components = d.GetRequiredService<IComponentRepository>();
                var permissionDb = d.GetRequiredService<IPermissionMapRepository>();
                var profileDb = d.GetRequiredService<IProfileMapRepository>();
                var userPermissionDb = d.GetRequiredService<IUserPermissionRepository>();
                var userProfileDb = d.GetRequiredService<IUserProfileRepository>();
                var users = d.GetRequiredService<IUserRepository>();
                return new DataProvider(
                    components,
                    permissionDb,
                    profileDb,
                    userPermissionDb,
                    userProfileDb,
                    users);
            });
            services.AddScoped<ApplicationController>();
        }

        public static T? GetObjectFromHeader<T>(this HttpRequest request, string headerName) where T : class
        {
            try
            {
                if (string.IsNullOrEmpty(headerName)) return default;
                if (!request.Headers.TryGetValue(headerName, out var strings)) return default;
                var source = strings.ToString();
                if (string.IsNullOrEmpty(source)) return default;
                var response = JsonConvert.DeserializeObject<T>(source);
                return response;
            }
            catch
            {
                return default;
            }
        }

        public static List<ValidationResult> Validate<T>(this T source, out bool isValid) where T : class
        {
            var context = new ValidationContext(source, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(source, context, validationResults, true);
            return validationResults;
        }

        internal static KeyValuePair<bool, string> Validate(this HttpRequest request, DataProvider db, string response)
        {
            var pair = new KeyValuePair<bool, string>(true, "application is valid");

            var application = request.GetObjectFromHeader<ApplicationRequestModel>("APP_IDENTITY");
            if (application == null)
            {
                return new KeyValuePair<bool, string>(false, response);
            }
            var apperrors = application.Validate(out bool isAppValid);
            if (!application.Id.HasValue || !isAppValid)
            {
                response = string.Join(';', apperrors.Select(m => m.ErrorMessage));
                return new KeyValuePair<bool, string>(false, response);
            }
            var matched = db.Find(application).GetAwaiter().GetResult();
            if (matched == null || !(matched.Name ?? "").Equals(application.Name))
            {
                response = "Target application is not found or mismatched.";
                return new KeyValuePair<bool, string>(false, response);
            }
            return pair;
        }

        private static async Task<Component?> Find(this DataProvider db, ApplicationRequestModel request)
        {
            if (request.Id == null) { return null; }
            return await db.ComponentDb.GetById(request.Id.GetValueOrDefault().ToString("D"));
        }
    }
}