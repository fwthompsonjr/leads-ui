using legallead.jdbc.entities;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Enumerations;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using legallead.Profiles.api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Contollers
{
    public class ProfileAddressChangeTest
    {

        private static readonly Faker<ChangeContactAddressRequest> faker =
            new Faker<ChangeContactAddressRequest>()
                .RuleFor(x => x.AddressType, y => y.PickRandom<AddressTypeNames>().ToString())
                .RuleFor(x => x.Address, y => y.Person.Email);
        private static readonly Faker<User> fakeUser = new Faker<User>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        [Fact]
        public async Task SutCanUpdateAddressAsync()
        {
            var provider = GetProvider();
            var db = provider.GetRequiredService<Mock<IProfileInfrastructure>>();
            var verifications = provider.GetRequiredService<Mock<IProfileRequestVerification>>();
            var user = fakeUser.Generate();
            var response = new ActionUserResponse { User = user };
            var contactResponse = new KeyValuePair<bool, string>(true, string.Empty);
            var payload = faker.Generate(10).ToArray();
            verifications.Setup(s => s.VerifyRequestAsync(
                It.IsAny<HttpRequest>(),
                It.IsAny<object[]>())).ReturnsAsync(response);
            db.Setup(s => s.ChangeContactAddressAsync(
                It.IsAny<User>(),
                It.IsAny<ChangeContactAddressRequest[]>())).ReturnsAsync(contactResponse);
            var service = provider.GetRequiredService<ProfilesController>();
            var result = await service.ChangeContactAddressAsync(payload);
            Assert.NotNull(result);
            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [Fact]
        public async Task SutVerifiesUpdateAddressAsync()
        {
            var provider = GetProvider();
            var db = provider.GetRequiredService<Mock<IProfileInfrastructure>>();
            var verifications = provider.GetRequiredService<Mock<IProfileRequestVerification>>();
            var user = fakeUser.Generate();
            var response = new ActionUserResponse { User = user, Result = new BadRequestObjectResult("Test response") };
            var contactResponse = new KeyValuePair<bool, string>(true, string.Empty);
            var payload = faker.Generate(10).ToArray();
            verifications.Setup(s => s.VerifyRequestAsync(
                It.IsAny<HttpRequest>(),
                It.IsAny<object[]>())).ReturnsAsync(response);
            db.Setup(s => s.ChangeContactAddressAsync(
                It.IsAny<User>(),
                It.IsAny<ChangeContactAddressRequest[]>())).ReturnsAsync(contactResponse);
            var service = provider.GetRequiredService<ProfilesController>();
            var result = await service.ChangeContactAddressAsync(payload);
            Assert.NotNull(result);
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task SutCanConflictUpdateAddressAsync()
        {
            var provider = GetProvider();
            var db = provider.GetRequiredService<Mock<IProfileInfrastructure>>();
            var verifications = provider.GetRequiredService<Mock<IProfileRequestVerification>>();
            var user = fakeUser.Generate();
            var response = new ActionUserResponse { User = user };
            var contactResponse = new KeyValuePair<bool, string>(false, string.Empty);
            var payload = faker.Generate(10).ToArray();
            verifications.Setup(s => s.VerifyRequestAsync(
                It.IsAny<HttpRequest>(),
                It.IsAny<object[]>())).ReturnsAsync(response);
            db.Setup(s => s.ChangeContactAddressAsync(
                It.IsAny<User>(),
                It.IsAny<ChangeContactAddressRequest[]>())).ReturnsAsync(contactResponse);
            var service = provider.GetRequiredService<ProfilesController>();
            var result = await service.ChangeContactAddressAsync(payload);
            Assert.NotNull(result);
            Assert.IsAssignableFrom<ConflictObjectResult>(result);
        }

        private static IServiceProvider GetProvider()
        {
            var service = new ServiceCollection();
            var mqInfrastructure = new Mock<IProfileInfrastructure>();
            var mqVerification = new Mock<IProfileRequestVerification>();
            //Arrange
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

            service.AddSingleton(mqInfrastructure);
            service.AddSingleton(mqInfrastructure.Object);
            service.AddSingleton(mqVerification);
            service.AddSingleton(mqVerification.Object);
            service.AddSingleton(request);
            service.AddSingleton(request.Object);
            service.AddSingleton(m =>
            {
                var db = m.GetRequiredService<IProfileInfrastructure>();
                var verifications = m.GetRequiredService<IProfileRequestVerification>();
                var controller = new ProfilesController(db)
                {
                    ControllerContext = controllerContext,
                    GetVerification = verifications
                };
                return controller;
            });
            return service.BuildServiceProvider();
        }
    }
}