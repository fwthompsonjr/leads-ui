using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace permissions.api.tests
{
    public class ValidateApplicationTests
    {
        private readonly Faker faker = new();

        [Fact]
        public void RequestValidationHappyPath()
        {
            const string headerName = "APP_IDENTITY";
            var obj = new ApplicationRequestModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Name.FullName()
            };
            var app = new Component { Id = obj.Id.GetValueOrDefault().ToString("D"), Name = obj.Name };
            var serialObj = JsonConvert.SerializeObject(obj);
            var headers = new HeaderDictionary
            {
                { "Content-Type", "application/json" },
                { headerName, serialObj }
            };
            var mock = GetMock();
            mock.SetupGet(x => x.Headers).Returns(headers);
            var db = new MockDataProvider();
            db.MqComponent.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(app);
            var request = mock.Object;
            var result = request.Validate(db, faker.Hacker.Phrase());
            Assert.True(result.Key);
            Assert.Equal("application is valid", result.Value);
        }

        [Fact]
        public void RequestWithOutHeaderIsNotValid()
        {
            string response = faker.Hacker.Phrase();
            var mock = GetMock();
            var db = new MockDataProvider();
            var request = mock.Object;
            var result = request.Validate(db, response);
            Assert.False(result.Key);
            Assert.Equal(response, result.Value);
        }

        private static Mock<HttpRequest> GetMock()
        {
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));
            return request;
        }

        private class MockDataProvider : DataProvider
        {
            private static Mock<IComponentRepository> ComponentMq { get; } = new();
            private static Mock<IPermissionMapRepository> PermissionMk { get; } = new();
            private static Mock<IProfileMapRepository> ProfileMk { get; } = new();
            private static Mock<IUserPermissionRepository> UserPermissionMk { get; } = new();
            private static Mock<IUserProfileRepository> UserProfileMk { get; } = new();
            private static Mock<IUserTokenRepository> UserTokenMk { get; } = new();
            private static Mock<IUserPermissionViewRepository> UserPermissionViewMk { get; } = new();
            private static Mock<IUserProfileViewRepository> UserProfileViewMk { get; } = new();
            private static Mock<IPermissionGroupRepository> ProfileGrpMk { get; } = new();
            private static Mock<IUserRepository> UserMq { get; } = new();

            public MockDataProvider() : base(
                ComponentMq.Object,
                PermissionMk.Object,
                ProfileMk.Object,
                UserPermissionMk.Object,
                UserProfileMk.Object,
                UserTokenMk.Object,
                UserPermissionViewMk.Object,
                UserProfileViewMk.Object,
                ProfileGrpMk.Object,
                UserMq.Object)
            {
            }

            public Mock<IComponentRepository> MqComponent => ComponentMq;
        }
    }
}