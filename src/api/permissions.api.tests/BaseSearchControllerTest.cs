using legallead.jdbc.entities;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests
{
    public abstract class BaseSearchControllerTest
    {
        protected static readonly Faker<ApplicationModel> modelfaker
            = new Faker<ApplicationModel>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        protected static readonly Faker<UserSearchBeginResponse> fakerSearchBegin
            = new Faker<UserSearchBeginResponse>()
            .RuleFor(x => x.RequestId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Request, y => fakerSearchRequest?.Generate() ?? new());


        protected static readonly Faker<User> userfaker = new Faker<User>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        protected static readonly Faker<UserSearchRequest> fakerSearchRequest
            = new Faker<UserSearchRequest>()
            .RuleFor(x => x.County, y => new())
            .RuleFor(x => x.Details, y => new())
            .RuleFor(x => x.EndDate, y => y.Random.Long(2000000, 3000000))
            .RuleFor(x => x.StartDate, y => y.Random.Long(1000000, 1999999));

        protected static readonly Faker<PurchasedSearchBo> purchaseFaker = new Faker<PurchasedSearchBo>()
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .RuleFor(x => x.ReferenceId, y => y.Random.Long().ToString());

        protected static IServiceProvider GetServiceProvider()
        {
            var service = new ServiceCollection();
            var validator = new Mock<IUserSearchValidator>();
            var infra = new Mock<ISearchInfrastructure>();
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
            service.AddSingleton(validator);
            service.AddSingleton(validator.Object);
            service.AddSingleton(request);
            service.AddSingleton(request.Object);
            service.AddSingleton(m =>
            {
                var data = m.GetRequiredService<ISearchInfrastructure>();
                var validator = m.GetRequiredService<IUserSearchValidator>();
                var locking = m.GetRequiredService<ICustomerLockInfrastructure>();
                var controller = new SearchController(validator, data, locking)
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            return service.BuildServiceProvider();
        }
    }
}
