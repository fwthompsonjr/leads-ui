using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace permissions.api.tests.Contollers
{
    public abstract class BaseControllerTest
    {
        private static IServiceProvider? _serviceProvider;

        protected static readonly Faker<UserLoginModel> faker =
            new Faker<UserLoginModel>()
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Password, y => y.Random.AlphaNumeric(22));

        protected static readonly Faker<Tokens> tokenFaker =
            new Faker<Tokens>()
            .RuleFor(x => x.RefreshToken, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.AccessToken, y => y.Random.AlphaNumeric(22));

        protected static readonly Faker<UserChangePasswordModel> changeFaker =
            new Faker<UserChangePasswordModel>()
            .RuleFor(x => x.UserName, y => y.Random.AlphaNumeric(22))
            .RuleFor(x => x.OldPassword, y => y.Random.AlphaNumeric(30))
            .RuleFor(x => x.NewPassword, y => y.Random.AlphaNumeric(30))
            .FinishWith((a, b) => b.ConfirmPassword = b.NewPassword);

        protected static IServiceProvider GetProvider()
        {
            if (_serviceProvider != null) { return _serviceProvider; }

            //Arrange
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));

            var httpContext = Mock.Of<HttpContext>(_ =>
                _.Request == request.Object
            );

            //Controller needs a controller context
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var refreshMock = new Mock<IRefreshTokenValidator>();
            var jwtMock = new Mock<IJwtManagerRepository>();
            var compMk = new Mock<IComponentRepository>();
            var permissionMk = new Mock<IPermissionMapRepository>();
            var profileMk = new Mock<IProfileMapRepository>();
            var userPermissionMk = new Mock<IUserPermissionRepository>();
            var userProfileMk = new Mock<IUserProfileRepository>();
            var userTokenMk = new Mock<IUserTokenRepository>();
            var userPermissionVwMk = new Mock<IUserPermissionViewRepository>();
            var userProfileVwMk = new Mock<IUserProfileViewRepository>();
            var permissionGroupMk = new Mock<IPermissionGroupRepository>();
            var permissionHistoryDb = new Mock<IUserPermissionHistoryRepository>();
            var profileHistoryDb = new Mock<IUserProfileHistoryRepository>();
            var userMk = new Mock<IUserRepository>();
            var stateMock = new Mock<IStateSearchProvider>();
            var collection = new ServiceCollection();
            collection.AddScoped(s => request);
            collection.AddScoped(s => userMk);
            collection.AddScoped(s => permissionMk);
            collection.AddScoped(s => profileMk);
            collection.AddScoped(s => userPermissionMk);
            collection.AddScoped(s => userProfileMk);
            collection.AddScoped(s => userTokenMk);
            collection.AddScoped(s => compMk);
            collection.AddScoped(s => jwtMock);
            collection.AddScoped(s => refreshMock);
            collection.AddScoped(s => userPermissionVwMk);
            collection.AddScoped(s => userProfileVwMk);
            collection.AddScoped(s => permissionGroupMk);
            collection.AddScoped(s => permissionHistoryDb);
            collection.AddScoped(s => userMk.Object);
            collection.AddScoped(s => permissionMk.Object);
            collection.AddScoped(s => profileMk.Object);
            collection.AddScoped(s => userPermissionMk.Object);
            collection.AddScoped(s => userProfileMk.Object);
            collection.AddScoped(s => userTokenMk.Object);
            collection.AddScoped(s => compMk.Object);
            collection.AddScoped(s => jwtMock.Object);
            collection.AddScoped(s => refreshMock.Object);
            collection.AddScoped(s => userPermissionVwMk.Object);
            collection.AddScoped(s => userProfileVwMk.Object);
            collection.AddScoped(s => permissionGroupMk.Object);
            collection.AddScoped(s => permissionHistoryDb.Object);
            collection.AddScoped(s => profileHistoryDb);
            collection.AddScoped(s => profileHistoryDb.Object);
            collection.AddScoped(s => stateMock);
            collection.AddScoped(s => stateMock.Object);
            collection.AddScoped(p =>
            {
                var a = p.GetRequiredService<IComponentRepository>();
                var b = p.GetRequiredService<IPermissionMapRepository>();
                var c = p.GetRequiredService<IProfileMapRepository>();
                var d = p.GetRequiredService<IUserPermissionRepository>();
                var e = p.GetRequiredService<IUserProfileRepository>();
                var f = p.GetRequiredService<IUserTokenRepository>();
                var g = p.GetRequiredService<IUserPermissionViewRepository>();
                var h = p.GetRequiredService<IUserProfileViewRepository>();
                var i = p.GetRequiredService<IPermissionGroupRepository>();
                var j = p.GetRequiredService<IUserRepository>();
                var k = p.GetRequiredService<IUserPermissionHistoryRepository>();
                var l = p.GetRequiredService<IUserProfileHistoryRepository>();
                return new DataProvider(a, b, c, d, e, f, g, h, i, j, k, l);
            });
            collection.AddScoped<IDataProvider>(p =>
            {
                var a = p.GetRequiredService<DataProvider>();
                return a;
            });
            collection.AddScoped(a =>
            {
                var db = a.GetRequiredService<DataProvider>();
                var sprovider = a.GetRequiredService<IStateSearchProvider>();
                return new ApplicationController(db, sprovider)
                {
                    ControllerContext = controllerContext
                };
            });
            collection.AddScoped(a =>
            {
                var db = a.GetRequiredService<DataProvider>();
                var jwt = a.GetRequiredService<IJwtManagerRepository>();
                var refresh = a.GetRequiredService<IRefreshTokenValidator>();
                var log = a.GetRequiredService<ILoggingInfrastructure>();
                return new SignonController(db, jwt, refresh, log)
                {
                    ControllerContext = controllerContext
                };
            });
            collection.AddScoped(a =>
            {
                var db = a.GetRequiredService<DataProvider>();
                return new ListsController(db)
                {
                    ControllerContext = controllerContext
                };
            });
            collection.AddScoped(s => new Mock<ILoggingInfrastructure>().Object);
            _serviceProvider = collection.BuildServiceProvider();
            return _serviceProvider;
        }

        protected static AppHeader GetApplicationHeader()
        {
            var localFaker = new Faker();
            var applist = ApplicationModel.GetApplicationsFallback();
            const string headerName = "APP_IDENTITY";
            var obj = new ApplicationRequestModel
            {
                Id = Guid.NewGuid(),
                Name = localFaker.PickRandom(applist).Name
            };
            var app = new Component { Id = obj.Id.GetValueOrDefault().ToString("D"), Name = obj.Name };
            var serialObj = JsonConvert.SerializeObject(obj);
            var headers = new HeaderDictionary
            {
                { "Content-Type", "application/json" },
                { headerName, serialObj }
            };
            return new AppHeader
            {
                App = app,
                AppHeading = obj,
                Headers = headers
            };
        }

        protected sealed class AppHeader
        {
            public Component App { get; set; } = new();
            public ApplicationRequestModel AppHeading { get; set; } = new();
            public HeaderDictionary Headers { get; set; } = new();
        }
    }
}