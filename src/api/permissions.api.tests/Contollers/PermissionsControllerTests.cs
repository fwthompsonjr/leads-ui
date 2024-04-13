using legallead.jdbc.entities;
using legallead.json.db.entity;
using legallead.models;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace permissions.api.tests.Contollers
{
    using LookupList = legallead.json.db.UsStateCountyList;
    using UsStateCounty = legallead.json.db.entity.UsStateCounty;
    public class PermissionsControllerTests
    {
        public PermissionsControllerTests()
        {
            if (Counties == null)
            {
                UsStateCounty.Initialize();
                Counties = LookupList.All;
            }
        }
        private static readonly List<string> LevelTypes = "Platinum,Gold,Silver,Guest".Split(',').ToList();
        private static List<UsStateCounty>? Counties { get; set; }
        private static readonly Faker<DiscountChoice> dchoicefaker =
            new Faker<DiscountChoice>()
                .RuleFor(x => x.IsSelected, y => y.Random.Bool())
            .FinishWith((a,b) =>
            {
                var item = a.PickRandom(Counties);
                b.StateName = item.StateCode ?? string.Empty;
                b.CountyName = item.Name ?? string.Empty;
            });

        private static readonly Faker<ChangeDiscountRequest> drequestfaker =
            new Faker<ChangeDiscountRequest>()
                .RuleFor(x => x.Choices, y => {
                    var n = y.Random.Int(1, 5);
                    return dchoicefaker.Generate(n);
                });
        private static readonly Faker<LevelRequestBo> levelBoFaker =
            new Faker<LevelRequestBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.InvoiceUri, y => y.Random.Guid().ToString())
            .RuleFor(x => x.LevelName, y => y.Random.Guid().ToString())
            .RuleFor(x => x.SessionId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        private static readonly Faker<User> userfaker = new Faker<User>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        private static readonly Faker<UserLevelRequest> levelRqfaker = new Faker<UserLevelRequest>()
            .RuleFor(x => x.Level, y => y.PickRandom(LevelTypes));

        [Fact]
        public void FakerCanGenerateDiscountChoices()
        {
            var exception = Record.Exception(() =>
            {
                _ = drequestfaker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void FakerCanGenerateLevelRequest()
        {
            var exception = Record.Exception(() =>
            {
                _ = levelBoFaker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void FakerCanGenerateUser()
        {
            var exception = Record.Exception(() =>
            {
                _ = userfaker.Generate();
            });
            Assert.Null(exception);
        }

        [Theory]
        [InlineData(true, false, false, false)]
        [InlineData(true, true, false, false)]
        [InlineData(true, false, true, false)]
        [InlineData(true, false, false, true)]
        [InlineData(false, false, false, false)]
        public async Task ContollerCanSetDiscount(bool hasUser, bool isLocked, bool isAdmin, bool isPaid)
        {
            var provider = GetProvider();
            var request = drequestfaker.Generate();
            var discount = levelBoFaker.Generate();
            var infra = provider.GetRequiredService<Mock<ISubscriptionInfrastructure>>();
            var lockDb = provider.GetRequiredService<Mock<ICustomerLockInfrastructure>>();
            var controller = provider.GetRequiredService<PermissionsController>();
            
            discount.IsPaymentSuccess = isPaid;
            User? user = hasUser ? userfaker.Generate() : null;
            infra.Setup(s => s.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infra.Setup(s => s.IsAdminUser(It.IsAny<HttpRequest>())).ReturnsAsync(isAdmin);
            infra.Setup(s => s.GenerateDiscountSession(
                It.IsAny<HttpRequest>(),
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<string>())).ReturnsAsync(discount);
            lockDb.Setup(s => s.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isLocked);
            var result = await controller.SetDiscount(request);
            Assert.NotNull(result);
            if (user == null) Assert.IsAssignableFrom<UnauthorizedObjectResult>(result);
            if (user != null && isLocked) Assert.IsAssignableFrom<ForbidResult>(result);
        }
        [Theory]
        [InlineData(true, false, false, false)]
        [InlineData(true, true, false, false)]
        [InlineData(true, false, true, false)]
        [InlineData(true, false, false, true)]
        [InlineData(false, false, false, false)]
        [InlineData(true, false, false, false, "admin")]
        [InlineData(true, false, true, false, "admin")]
        [InlineData(true, false, false, false, "invalid-level")]
        public async Task ContollerCanSetPermissionLevel(
            bool hasUser, 
            bool isLocked, 
            bool isAdmin, 
            bool isPaid,
            string permissionReqeuested = "")
        {
            var provider = GetProvider();
            var request = levelRqfaker.Generate();
            var discount = levelBoFaker.Generate();
            var infra = provider.GetRequiredService<Mock<ISubscriptionInfrastructure>>();
            var lockDb = provider.GetRequiredService<Mock<ICustomerLockInfrastructure>>();
            var controller = provider.GetRequiredService<PermissionsController>();
            discount.IsPaymentSuccess = isPaid;
            if (!string.IsNullOrEmpty(permissionReqeuested)) { request.Level = permissionReqeuested; }
            User? user = hasUser ? userfaker.Generate() : null;
            infra.Setup(s => s.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infra.Setup(s => s.IsAdminUser(It.IsAny<HttpRequest>())).ReturnsAsync(isAdmin);
            infra.Setup(s => s.GeneratePermissionSession(
                It.IsAny<HttpRequest>(),
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(discount);
            lockDb.Setup(s => s.IsAccountLocked(It.IsAny<string>())).ReturnsAsync(isLocked);
            var result = await controller.SetPermissionLevel(request);
            Assert.NotNull(result);
            if (user == null) Assert.IsAssignableFrom<UnauthorizedObjectResult>(result);
            if (user != null && isLocked) Assert.IsAssignableFrom<ForbidResult>(result);
        }
        private static IServiceProvider GetProvider()
        {
            var service = new ServiceCollection();
            var infra = new Mock<ISubscriptionInfrastructure>();
            var lockDb = new Mock<ICustomerLockInfrastructure>();
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

            service.AddSingleton(infra);
            service.AddSingleton(infra.Object);
            service.AddSingleton(lockDb);
            service.AddSingleton(lockDb.Object);
            service.AddSingleton(request);
            service.AddSingleton(request.Object);
            service.AddSingleton(m =>
            {
                var data = m.GetRequiredService<ISubscriptionInfrastructure>();
                var locking = m.GetRequiredService<ICustomerLockInfrastructure>();
                var controller = new PermissionsController(data, locking)
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            return service.BuildServiceProvider();
        }
    }
}
