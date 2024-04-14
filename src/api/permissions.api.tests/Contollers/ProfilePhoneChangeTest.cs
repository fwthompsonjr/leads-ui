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
    public class ProfilePhoneChangeTest
    {
        private static readonly string[] fakerNumbers = new string[]
        {
            "(799) 846-8465",
            "(355) 976-9927",
            "(688) 671-3533",
            "(731) 708-8757",
            "(587) 863-9416",
            "(795) 499-0471",
            "(714) 682-7575",
            "(939) 792-5046",
            "(842) 691-3765",
            "(907) 785-9086",
            "(226) 349-3936",
            "(871) 758-6644",
            "608-943-3822",
            "572-620-8386",
            "575-439-9534",
            "401-869-3467",
            "803-353-1652",
            "364-371-8470",
            "229-591-5606",
            "469-496-2948",
        };

        private static readonly Faker<ChangeContactPhoneRequest> faker =
            new Faker<ChangeContactPhoneRequest>()
                .RuleFor(x => x.PhoneType, y => y.PickRandom<PhoneTypeNames>().ToString())
                .RuleFor(x => x.Phone, y => y.PickRandom(fakerNumbers));
        private static readonly Faker<User> fakeUser = new Faker<User>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        [Fact]
        public async Task SutCanUpdatePhoneNumber()
        {
            var provider = GetProvider();
            var db = provider.GetRequiredService<Mock<IProfileInfrastructure>>();
            var verifications = provider.GetRequiredService<Mock<IProfileRequestVerification>>();
            var user = fakeUser.Generate();
            var response = new ActionUserResponse { User = user };
            var contactResponse = new KeyValuePair<bool, string>(true, string.Empty);
            var payload = faker.Generate(10).ToArray();
            verifications.Setup(s => s.VerifyRequest(
                It.IsAny<HttpRequest>(),
                It.IsAny<object[]>())).ReturnsAsync(response);
            db.Setup(s => s.ChangeContactPhone(
                It.IsAny<User>(),
                It.IsAny<ChangeContactPhoneRequest[]>())).ReturnsAsync(contactResponse);
            var service = provider.GetRequiredService<ProfilesController>();
            var result = await service.ChangeContactPhone(payload);
            Assert.NotNull(result);
            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [Fact]
        public async Task SutVerifiesUpdatePhoneNumber()
        {
            var provider = GetProvider();
            var db = provider.GetRequiredService<Mock<IProfileInfrastructure>>();
            var verifications = provider.GetRequiredService<Mock<IProfileRequestVerification>>();
            var user = fakeUser.Generate();
            var response = new ActionUserResponse { User = user, Result = new BadRequestObjectResult("Test response") };
            var contactResponse = new KeyValuePair<bool, string>(true, string.Empty);
            var payload = faker.Generate(10).ToArray();
            verifications.Setup(s => s.VerifyRequest(
                It.IsAny<HttpRequest>(),
                It.IsAny<object[]>())).ReturnsAsync(response);
            db.Setup(s => s.ChangeContactPhone(
                It.IsAny<User>(),
                It.IsAny<ChangeContactPhoneRequest[]>())).ReturnsAsync(contactResponse);
            var service = provider.GetRequiredService<ProfilesController>();
            var result = await service.ChangeContactPhone(payload);
            Assert.NotNull(result);
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task SutCanConflictUpdatePhoneNumber()
        {
            var provider = GetProvider();
            var db = provider.GetRequiredService<Mock<IProfileInfrastructure>>();
            var verifications = provider.GetRequiredService<Mock<IProfileRequestVerification>>();
            var user = fakeUser.Generate();
            var response = new ActionUserResponse { User = user };
            var contactResponse = new KeyValuePair<bool, string>(false, string.Empty);
            var payload = faker.Generate(10).ToArray();
            verifications.Setup(s => s.VerifyRequest(
                It.IsAny<HttpRequest>(),
                It.IsAny<object[]>())).ReturnsAsync(response);
            db.Setup(s => s.ChangeContactPhone(
                It.IsAny<User>(),
                It.IsAny<ChangeContactPhoneRequest[]>())).ReturnsAsync(contactResponse);
            var service = provider.GetRequiredService<ProfilesController>();
            var result = await service.ChangeContactPhone(payload);
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
