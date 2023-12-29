using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api;
using legallead.permissions.api.Utility;
using legallead.Profiles.api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Contollers
{
    public class ProfilesControllerTests
    {
        private static readonly Faker<User> userFaker =
            new Faker<User>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.UserName, y => y.Person.FullName)
            .RuleFor(x => x.PasswordHash, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.PasswordSalt, y => y.Random.AlphaNumeric(25));

        private static readonly Faker<UserProfileView> profileFaker =
            new Faker<UserProfileView>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ProfileMapId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.OrderId, y => y.IndexFaker)
            .RuleFor(x => x.KeyValue, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.KeyName, y => y.Random.AlphaNumeric(25));

        [Fact]
        public void ControllerCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var provider = GetTestArtifacts();
                _ = provider.GetRequiredService<ProfilesController>();
            });
            Assert.Null(exception);
        }

        private static UserProfileView[] GetProfile()
        {
            var fieldNames = new[]
            {
                "First Name",
                "Last Name",
                "Company Name",
                "Phone 1",
                "Phone 2",
                "Phone 3",
                "Email 1",
                "Email 2",
                "Email 3",
                "Address 1 - Address Line 1",
                "Address 1 - Address Line 2",
                "Address 1 - Address Line 3",
                "Address 1 - Address Line 4",
                "Address 2 - Address Line 1",
                "Address 2 - Address Line 2",
                "Address 2 - Address Line 3",
                "Address 2 - Address Line 4",
            };
            var list = profileFaker.Generate(fieldNames.Length);
            list.ForEach(a => a.KeyName = fieldNames[list.IndexOf(a)]);
            return list.ToArray();
        }

        private static IServiceProvider GetTestArtifacts()
        {
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));

            var httpContext = Mock.Of<HttpContext>(_ =>
                _.Request == request.Object
            );
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var collection = new ServiceCollection();
            collection.AddSingleton(m => new Mock<IDataProvider>());
            collection.AddSingleton(m => new Mock<IUserProfileRepository>());
            collection.AddSingleton(m => new Mock<IUserProfileViewRepository>());
            collection.AddSingleton(m => new Mock<IUserProfileHistoryRepository>());
            collection.AddSingleton(m => m.GetRequiredService<Mock<IDataProvider>>().Object);
            collection.AddSingleton(m => m.GetRequiredService<Mock<IUserProfileRepository>>().Object);
            collection.AddSingleton(m => m.GetRequiredService<Mock<IUserProfileViewRepository>>().Object);
            collection.AddSingleton(m => m.GetRequiredService<Mock<IUserProfileHistoryRepository>>().Object);
            collection.AddSingleton(m => GetProfile());
            collection.AddSingleton(m => userFaker.Generate());
            collection.AddSingleton<IProfileInfrastructure>(m =>
            {
                var db = m.GetRequiredService<IDataProvider>();
                return new ProfileInfrastructure(db);
            });
            collection.AddSingleton(m =>
            {
                var db = m.GetRequiredService<IProfileInfrastructure>();
                return new ProfilesController(db)
                {
                    ControllerContext = controllerContext
                };
            });
            var provider = collection.BuildServiceProvider();
            var dataProvider = provider.GetRequiredService<Mock<IDataProvider>>();
            var userProfileVw = provider.GetRequiredService<IUserProfileViewRepository>();
            var profileHistoryDb = provider.GetRequiredService<IUserProfileHistoryRepository>();
            var userProfileDb = provider.GetRequiredService<IUserProfileRepository>();
            var userProfileVwMock = provider.GetRequiredService<Mock<IUserProfileViewRepository>>();
            var profile = provider.GetRequiredService<UserProfileView[]>();
            userProfileVwMock.Setup(m => m.GetAll(It.IsAny<User>())).ReturnsAsync(profile);
            dataProvider.SetupGet(m => m.UserProfileVw).Returns(userProfileVw);
            dataProvider.SetupGet(m => m.ProfileHistoryDb).Returns(profileHistoryDb);
            dataProvider.SetupGet(m => m.UserProfileDb).Returns(userProfileDb);

            return provider;
        }
    }
}