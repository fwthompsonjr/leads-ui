using legallead.email.utility;
using legallead.jdbc.helpers;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.logging;
using legallead.logging.helpers;
using legallead.logging.implementations;
using legallead.logging.interfaces;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Health;
using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using legallead.permissions.api.Utility;
using legallead.Profiles.api.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Stripe;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace legallead.permissions.api
{
    public static class ObjectExtensions
    {
        public static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddSingleton<IUserSearchValidator>(a =>
            {
                var cfg = a.GetRequiredService<IConfiguration>();
                var mx = Convert.ToInt32(cfg["Search:MaxDays"]);
                var mn = Convert.ToInt64(cfg["Search:MinStartDate"]);
                return new UserSearchValidator { MinStartDate = mn, MaxDays = mx };
            });
            services.AddSingleton(a =>
            {
                var cfg = a.GetRequiredService<IConfiguration>();
                var mx = Convert.ToInt32(cfg["Search:MaxDays"]);
                var mn = Convert.ToInt64(cfg["Search:MinStartDate"]);
                return new UserSearchValidator { MinStartDate = mn, MaxDays = mx };
            });
            services.SetupJwt(configuration);
        }

        public static void RegisterDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            string environ = GetConfigOrDefault(configuration, "DataEnvironment", "Test");
            var payment = MapOption(configuration);
            var stripeConfig = MapStripeKey(configuration);
            services.AddSingleton(payment);
            services.AddSingleton(stripeConfig);
            services.AddScoped<ICountyAuthorizationService, CountyAuthorizationService>();
            services.AddScoped<IAppAuthenicationService, AppAuthenicationService>();
            services.AddScoped<IStripeInfrastructure, StripeInfrastructure>();
            services.AddScoped<IPaymentHtmlTranslator, PaymentHtmlTranslator>();
            services.AddSingleton<IJwtManagerRepository, JwtManagerRepository>();
            services.AddSingleton<IRefreshTokenValidator, RefreshTokenValidator>();
            services.AddSingleton<IDataInitializer, DataInitializer>();
            services.AddScoped<ICustomerInfrastructure, CustomerInfrastructure>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IPricingRepository, PricingRepository>();
            services.AddScoped<IUserLockStatusRepository, UserLockStatusRepository>();
            services.AddScoped<ISearchQueueRepository, SearchQueueRepository>();
            services.AddScoped<IDapperCommand, DapperExecutor>();
            services.AddScoped(d =>
            {
                var command = d.GetRequiredService<IDapperCommand>();
                var dbint = d.GetRequiredService<IDataInitializer>();
                return new DataContext(command, dbint, environ);
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
            services.AddScoped<IUserSearchRepository, UserSearchRepository>();
            services.AddScoped<IAppSettingRepository, AppSettingRepository>();
            services.AddScoped<IQueueWorkRepository, QueueWorkRepository>();
            services.AddScoped<ICustomerLockInfrastructure, CustomerLockInfrastructure>();
            services.AddScoped<IClientSecretService, ClientSecretService>();
            services.AddScoped<IMailBoxRepository, MailBoxRepository>();
            services.AddScoped<IUserMailbox, UserMailboxService>();
            services.AddScoped<IAppSettingService, AppSettingService>();
            services.AddScoped<MailboxController>();
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
            services.AddScoped<IRequestedUser>(p =>
            {
                var provider = p.GetRequiredService<IDataProvider>();
                return new RequestedUserService(provider);
            });

            services.AddScoped<ISearchInfrastructure>(p =>
            {
                var provider = p.GetRequiredService<IDataProvider>();
                var repo = p.GetRequiredService<IUserSearchRepository>();
                var usr = p.GetRequiredService<IRequestedUser>();
                return new SearchInfrastructure(provider, repo, usr);
            });
            services.AddScoped<SignonController>();
            services.AddScoped<ApplicationController>();
            services.AddScoped(p =>
            {
                var data = p.GetRequiredService<DataProvider>();
                var locking = p.GetRequiredService<ICustomerLockInfrastructure>();
                return new ListsController(data, locking);
            });
            services.AddScoped<IStateSearchProvider, StateSearchProvider>();
            services.AddScoped<PermissionsController>();
            services.AddSingleton<IStartupTask, JsonInitStartupTask>();
            services.AddSingleton<IStartupTask, JdbcInitStartUpTask>();
            services.AddSingleton<LoggingDbServiceProvider>();
            services.AddScoped<SettingsController>();
            services.AddScoped<HomeController>();
            services.AddScoped<ProfilesController>();
            services.AddScoped<AppController>();
            services.AddScoped(p =>
            {
                return new PaymentController(payment,
                    p.GetRequiredService<ISearchInfrastructure>(),
                    p.GetRequiredService<IStripeInfrastructure>());
            });
            services.AddScoped(s =>
            {
                var db = s.GetRequiredService<ISearchInfrastructure>();
                var validator = s.GetRequiredService<UserSearchValidator>();
                var lockdb = s.GetRequiredService<ICustomerLockInfrastructure>();
                return new SearchController(validator, db, lockdb);
            });
            // logging
            services.AddSingleton<LoggingDbServiceProvider>();
            services.AddScoped<ILoggingDbCommand, LoggingDbExecutor>();
            services.AddScoped<ILoggingDbContext>(s =>
            {
                var command = s.GetRequiredService<ILoggingDbCommand>();
                return new LoggingDbContext(command, environ, "error");
            });
            // logging content repository
            services.AddScoped<ILogContentRepository>(s =>
            {
                var context = s.GetRequiredService<ILoggingDbContext>();
                return new LogContentRepository(context);
            });
            // logging configuration
            services.AddScoped(p =>
            {
                var logprovider = p.GetRequiredService<LoggingDbServiceProvider>().Provider;
                return logprovider.GetRequiredService<ILogConfiguration>();
            });

            services.AddScoped<EventsController>();
            // logging service
            services.AddScoped<ILoggingService>(p =>
            {
                var guid = Guid.NewGuid();
                var repo = p.GetRequiredService<ILogContentRepository>();
                var cfg = p.GetRequiredService<ILogConfiguration>();
                return new LoggingService(guid, repo, cfg);
            });
            // logging repository
            services.AddScoped<ILoggingInfrastructure>(p =>
            {
                var lg = p.GetRequiredService<ILoggingService>();
                return new LoggingInfrastructure(lg);
            });

            var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole());
            var queueLogger = loggerFactory.CreateLogger<QueueResetService>();
            var priceLogger = loggerFactory.CreateLogger<PricingSyncService>();
            var acctLogger = loggerFactory.CreateLogger<PaymentAccountCreationService>();
            var syncLogger = loggerFactory.CreateLogger<SubscriptionSyncService>();
            services.AddSingleton(s => queueLogger);
            services.AddSingleton(s => priceLogger);
            services.AddSingleton(s => acctLogger);
            services.AddSingleton(s => syncLogger);
            services.AddSingleton(s =>
            {
                var logger = s.GetRequiredService<ILogger<QueueResetService>>();
                var exec = new DapperExecutor();
                var context = new DataContext(exec);
                var db = new UserSearchRepository(context);
                return new QueueResetService(db, logger);
            });
            services.AddSingleton(s =>
            {
                var exec = new DapperExecutor();
                var context = new DataContext(exec);
                var userDb = new UserRepository(context);
                var custDb = new CustomerRepository(context);
                var custInfra = new CustomerInfrastructure(s.GetRequiredService<StripeKeyEntity>(), userDb, custDb);
                var logging = s.GetRequiredService<ILogger<PaymentAccountCreationService>>();
                return new PaymentAccountCreationService(logging, custInfra);
            });
            services.AddSingleton(s =>
            {
                var exec = new DapperExecutor();
                var context = new DataContext(exec);
                var repo = new PricingRepository(context);
                var logging = s.GetRequiredService<ILogger<PricingSyncService>>();
                return new PricingSyncService(logging, repo);
            });
            services.AddSingleton(s =>
            {
                var exec = new DapperExecutor();
                var context = new DataContext(exec);
                var repo = new CustomerRepository(context);
                var logging = s.GetRequiredService<ILogger<SubscriptionSyncService>>();
                return new SubscriptionSyncService(logging, repo);
            });
            services.AddHostedService(s => s.GetRequiredService<QueueResetService>());
            services.AddHostedService(s => s.GetRequiredService<PaymentAccountCreationService>());
            services.AddHostedService(s => s.GetRequiredService<PricingSyncService>());
            services.AddHostedService(s => s.GetRequiredService<SubscriptionSyncService>());
            
            loggerFactory.Dispose();
        }

        public static void RegisterHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<ControllerHealthCheck>("Contollers")
                .AddCheck<DataHealthCheck>("Data")
                .AddCheck<DbConnectionHealthCheck>("DBConnection")
                .AddCheck<InfrastructureHealthCheck>("Infrastructure")
                .AddCheck<RepositoryHealthCheck>("Repository")
                .AddCheck<PricingHealthCheck>("Pricing");
        }

        public static void RegisterEmailServices(this IServiceCollection services)
        {
            services.Initialize();
            services.AddScoped<IQueueNotificationService, QueueNotificationService>();
            services.AddScoped<ISearchStatusRepository, SearchStatusRepository>();
            services.AddScoped<IQueueStatusService>(s =>
            {
                var repo = s.GetRequiredService<IQueueWorkRepository>();
                var queue = s.GetRequiredService<ISearchQueueRepository>();
                var notification = s.GetRequiredService<IQueueNotificationService>();
                var sts = s.GetRequiredService<ISearchStatusRepository>();
                var usrdb = s.GetRequiredService<IUserSearchRepository>();
                var wrapper = new MailMessageWrapper(notification);
                return new QueueStatusService(repo, queue, sts, usrdb, wrapper);
            });
            services.AddScoped<QueueController>();
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

        public static bool IsRestricted(this SearchRestrictionDto dto)
        {
            if (!dto.MaxPerMonth.HasValue) return true;
            if (dto.IsLocked.GetValueOrDefault()) return true;
            if (dto.ThisMonth.GetValueOrDefault() >= dto.MaxPerMonth.GetValueOrDefault()) return true;
            if (dto.ThisYear.GetValueOrDefault() >= dto.MaxPerYear.GetValueOrDefault()) return true;
            return false;
        }

        public static UserSearchBeginResponse GetRestrictionResponse(this SearchRestrictionDto dto)
        {
            var response = new UserSearchBeginResponse();

            if (!dto.MaxPerMonth.HasValue)
            {
                response.Request.Details.Add(new() { Name = "No Value Returned", Text = "Unable to calculate usage" });
            }
            if (dto.IsLocked.GetValueOrDefault())
            {
                response.Request.Details.Add(new() { Name = "Account restriction", Text = dto.Reason ?? "" });
            }
            if (dto.ThisMonth.GetValueOrDefault() >= dto.MaxPerMonth.GetValueOrDefault())
            {
                var mtd = $"Month to date: {dto.ThisMonth.GetValueOrDefault()}. Limit: {dto.MaxPerMonth.GetValueOrDefault()}";
                response.Request.Details.Add(new() { Name = "Monthly Limit Exceeded", Text = mtd });
            }
            if (dto.ThisYear.GetValueOrDefault() >= dto.MaxPerYear.GetValueOrDefault())
            {
                var ytd = $"Year to date: {dto.ThisYear.GetValueOrDefault()}. Limit: {dto.MaxPerYear.GetValueOrDefault()}";
                response.Request.Details.Add(new() { Name = "Annual Limit Exceeded", Text = ytd });
            }
            return response;
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

        internal static async Task<User?> GetUserAsync(this HttpRequest request, IDataProvider db)
        {
            var identity = request.HttpContext.User.Identity;
            if (identity == null) return null;
            var user = await db.UserDb.GetByEmail(identity.Name ?? string.Empty);
            return user;
        }

        internal static async Task<string?> GetUserLevelAsync(this HttpRequest request, IDataProvider db)
        {
            const string fallback = "None";
            var user = await request.GetUserAsync(db);
            if (user == null) return fallback;
            var userlevel = (await db.UserPermissionVw.GetAll(user)) ?? Array.Empty<UserPermissionView>();
            var level = userlevel.FirstOrDefault(x => x.KeyName == "Account.Permission.Level");
            string levelName = level?.KeyValue ?? fallback;
            return levelName;
        }

        internal static async Task<bool> IsAdminUserAsync(this HttpRequest request, IDataProvider db)
        {
            var level = await request.GetUserLevelAsync(db);
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
        [ExcludeFromCodeCoverage(Justification = "Private method tested via public method.")]
        private static void SetupJwt(this IServiceCollection services, IConfiguration configuration)
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


        private static StripeKeyEntity? _stripeKeyEntity;
        private static PaymentStripeOption? _paymentOption;

        private static PaymentStripeOption MapOption(IConfiguration configuration)
        {
            if (_paymentOption != null) { return _paymentOption; }
            var key = configuration.GetValue<string>("Payment:key") ?? string.Empty;
            StripeConfiguration.ApiKey = key;
            var child = new PaymentCode
            {
                Admin = configuration.GetValue<string>("Payment:codes:admin") ?? string.Empty,
                Gold = configuration.GetValue<string>("Payment:codes:gold") ?? string.Empty,
                Guest = configuration.GetValue<string>("Payment:codes:guest") ?? string.Empty,
                Platinum = configuration.GetValue<string>("Payment:codes:platinum") ?? string.Empty,
                Silver = configuration.GetValue<string>("Payment:codes:silver") ?? string.Empty,
            };
            _paymentOption = new PaymentStripeOption { Key = key, Codes = child };
            return _paymentOption;
        }

        private static StripeKeyEntity MapStripeKey(IConfiguration configuration)
        {
            if (_stripeKeyEntity != null) { return _stripeKeyEntity; }
            var webid = configuration.GetValue<string>("Payment:keys:webhook");
            var keytype = configuration.GetValue<string>("Payment:keys:active") ?? string.Empty;
            var test = configuration.GetValue<string>("Payment:keys:values:test") ?? string.Empty;
            var prd = configuration.GetValue<string>("Payment:keys:values:prod") ?? string.Empty;
            var items = new List<StripeKeyItem> {
                new() { Name="test", Value= test },
                new() { Name="prod", Value= prd }
            };
            _stripeKeyEntity = new StripeKeyEntity
            {
                WebhookId = webid,
                ActiveName = keytype,
                Items = items
            };
            return _stripeKeyEntity;
        }

        [ExcludeFromCodeCoverage]
        private static string GetConfigOrDefault(IConfiguration? configuration, string key, string backup)
        {
            try
            {
                if (configuration == null) return backup;
                return configuration.GetValue<string>(key) ?? backup;
            }
            catch (Exception)
            {
                return backup;
            }
        }
    }
}