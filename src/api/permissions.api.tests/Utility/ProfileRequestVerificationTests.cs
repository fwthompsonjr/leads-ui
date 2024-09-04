using legallead.jdbc.entities;
using legallead.permissions.api.Enumerations;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using legallead.permissions.api.Utility;
using legallead.Profiles.api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Utility
{
    public class ProfileRequestVerificationTests
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
        public void UserCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = fakeUser.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void RequestCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate(10);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ServiceCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                var provider = GetProvider();
                var service = provider.GetRequiredService<IProfileRequestVerification>();
                Assert.NotNull(service);
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public async Task ServiceCanVerifyAsync(bool hasUser, bool hasError)
        {
            var provider = GetProvider();
            var http = provider.GetRequiredService<HttpRequest>();
            var db = provider.GetRequiredService<Mock<IProfileInfrastructure>>();
            User? user = hasUser ? fakeUser.Generate() : null;
            db.Setup(s => s.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            var payload = faker.Generate(10).ToArray();
            if (hasError)
            {
                var invalid = new Faker().PickRandom(payload);
                invalid.PhoneType = "Invalid";
                invalid.Phone = string.Empty;
            }
            var service = provider.GetRequiredService<IProfileRequestVerification>();
            var response = await service.VerifyRequestAsync(http, payload);
            Assert.NotNull(response);
            if (hasUser && hasError)
            {
                Assert.IsType<BadRequestObjectResult>(response.Result);
            }
            if (!hasUser)
            {
                Assert.IsType<UnauthorizedObjectResult>(response.Result);
            }
        }

        private static IServiceProvider GetProvider()
        {
            var service = new ServiceCollection();
            var mqInfrastructure = new Mock<IProfileInfrastructure>();
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
            service.AddSingleton(request);
            service.AddSingleton(request.Object);
            service.AddSingleton<IProfileRequestVerification, ProfileRequestVerification>();
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
