using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.logging;
using legallead.logging.interfaces;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Model;
using legallead.permissions.api.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace legallead.permissions.api
{
    public static class ObjectExtensions
    {
        public static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var keyconfig = configuration["JWT:Key"] ?? string.Empty;
                var Key = Encoding.UTF8.GetBytes(keyconfig);
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static void RegisterDataServices(this IServiceCollection services)
        {
            services.AddSingleton<IJwtManagerRepository, JwtManagerRepository>();
            services.AddSingleton<IRefreshTokenValidator, RefreshTokenValidator>();
            services.AddSingleton<IDataInitializer, DataInitializer>();
            services.AddScoped<IDapperCommand, DapperExecutor>();
            services.AddScoped(d =>
            {
                var command = d.GetRequiredService<IDapperCommand>();
                var dbint = d.GetRequiredService<IDataInitializer>();
                return new DataContext(command, dbint);
            });
            services.AddScoped<ISubscriptionInfrastructure, SubscriptionInfrastructure>();
            services.AddScoped<IComponentRepository, ComponentRepository>();
            services.AddScoped<IPermissionMapRepository, PermissionMapRepository>();
            services.AddScoped<IProfileMapRepository, ProfileMapRepository>();
            services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IUserTokenRepository, UserTokenRepository>();
            services.AddScoped<IUserPermissionViewRepository, UserPermissionViewRepository>();
            services.AddScoped<IUserProfileViewRepository, UserProfileViewRepository>();
            services.AddScoped<IPermissionGroupRepository, PermissionGroupRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserPermissionHistoryRepository, UserPermissionHistoryRepository>();
            services.AddScoped<IUserProfileHistoryRepository, UserProfileHistoryRepository>();
            services.AddScoped(d =>
            {
                var components = d.GetRequiredService<IComponentRepository>();
                var permissionDb = d.GetRequiredService<IPermissionMapRepository>();
                var profileDb = d.GetRequiredService<IProfileMapRepository>();
                var userPermissionDb = d.GetRequiredService<IUserPermissionRepository>();
                var userProfileDb = d.GetRequiredService<IUserProfileRepository>();
                var userTokenDb = d.GetRequiredService<IUserTokenRepository>();
                var userPermissionVw = d.GetRequiredService<IUserPermissionViewRepository>();
                var userProfileVw = d.GetRequiredService<IUserProfileViewRepository>();
                var permissionGroupDb = d.GetRequiredService<IPermissionGroupRepository>();
                var users = d.GetRequiredService<IUserRepository>();
                var permissionHistoryDb = d.GetRequiredService<IUserPermissionHistoryRepository>();
                var profileHistoryDb = d.GetRequiredService<IUserProfileHistoryRepository>();
                return new DataProvider(
                    components,
                    permissionDb,
                    profileDb,
                    userPermissionDb,
                    userProfileDb,
                    userTokenDb,
                    userPermissionVw,
                    userProfileVw,
                    permissionGroupDb,
                    users,
                    permissionHistoryDb,
                    profileHistoryDb);
            });
            services.AddScoped<IDataProvider>(p =>
            {
                return p.GetRequiredService<DataProvider>();
            });
            services.AddScoped<IProfileInfrastructure>(p =>
            {
                var provider = p.GetRequiredService<IDataProvider>();
                return new ProfileInfrastructure(provider);
            });
            services.AddScoped<SignonController>();
            services.AddScoped<ApplicationController>();
            services.AddScoped(p =>
            {
                var data = p.GetRequiredService<DataProvider>();
                return new ListsController(data);
            });
            services.AddScoped<PermissionsController>();
            services.AddSingleton<IStartupTask, JsonInitStartupTask>();
            services.AddSingleton<IStartupTask, JdbcInitStartUpTask>();
            services.AddSingleton<LoggingDbServiceProvider>();

            // logging
            services.AddScoped(p =>
            {
                var logprovider = p.GetRequiredService<LoggingDbServiceProvider>().Provider;
                return logprovider.GetRequiredService<ILogConfiguration>();
            });
            // logging
            services.AddScoped(p =>
            {
                var logprovider = p.GetRequiredService<LoggingDbServiceProvider>().Provider;
                return logprovider.GetRequiredService<ILoggingService>();
            });
            services.AddScoped<ILoggingInfrastructure>(p =>
            {
                var lg = p.GetRequiredService<ILoggingService>();
                return new LoggingInfrastructure(lg);
            });
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

        public static string IfNull(this string? s, string fallback)
        {
            if (s == null) return fallback;
            return s;
        }

        internal static KeyValuePair<bool, string> Validate(this HttpRequest request, string response)
        {
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
            return SimpleNameValidation(application.Name);
        }

        internal static async Task<User?> GetUser(this HttpRequest request, IDataProvider db)
        {
            var identity = request.HttpContext.User.Identity;
            if (identity == null) return null;
            var user = await db.UserDb.GetByEmail(identity.Name ?? string.Empty);
            return user;
        }

        internal static async Task<string?> GetUserLevel(this HttpRequest request, IDataProvider db)
        {
            const string fallback = "None";
            var user = await request.GetUser(db);
            if (user == null) return fallback;
            var userlevel = (await db.UserPermissionVw.GetAll(user)) ?? Array.Empty<UserPermissionView>();
            var level = userlevel.FirstOrDefault(x => x.KeyName == "Account.Permission.Level");
            string levelName = level?.KeyValue ?? fallback;
            return levelName;
        }

        internal static async Task<bool> IsAdminUser(this HttpRequest request, IDataProvider db)
        {
            var level = await request.GetUserLevel(db);
            if (level == null) return false;
            return level.Equals("admin", StringComparison.OrdinalIgnoreCase);
        }

        private static KeyValuePair<bool, string> SimpleNameValidation(string name)
        {
            var names = ApplicationModel.GetApplicationsFallback();
            var appNames = names.Select(x => x.Name).ToList();
            var matched = appNames.Exists(x => x.Equals(name, StringComparison.OrdinalIgnoreCase));
            var message = matched ? "application is valid" : "Target application is not found or mismatched.";
            return new KeyValuePair<bool, string>(matched, message);
        }
    }
}