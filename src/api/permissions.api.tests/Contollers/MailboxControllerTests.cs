using legallead.jdbc.entities;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Contollers
{
    public class MailboxControllerTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ControllerCanGetCountAsync(bool hasUser = true)
        {
            var provider = GetProvider();
            var user = hasUser ? provider.GetRequiredService<User>() : null;
            var search = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var mailbox = provider.GetRequiredService<Mock<IUserMailbox>>();
            var payload = provider.GetRequiredService<MailboxRequest>();
            var bo = new EmailCountBo();
            mailbox.Setup(m => m.GetCountAsync(It.IsAny<MailboxRequest>())).ReturnsAsync(bo);
            search.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            var controller = provider.GetRequiredService<MailboxController>();
            var problems = await Record.ExceptionAsync(async () =>
            {
                _ = await controller.GetCountAsync(payload);
            });
            Assert.Null(problems);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ControllerCanGetBodyAsync(bool hasUser = true)
        {
            var provider = GetProvider();
            var user = hasUser ? provider.GetRequiredService<User>() : null;
            var search = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var mailbox = provider.GetRequiredService<Mock<IUserMailbox>>();
            var payload = provider.GetRequiredService<MailboxRequest>();
            var bo = new EmailBodyBo();
            mailbox.Setup(m => m.GetBodyAsync(It.IsAny<MailboxRequest>())).ReturnsAsync(bo);
            search.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            var controller = provider.GetRequiredService<MailboxController>();
            var problems = await Record.ExceptionAsync(async () =>
            {
                _ = await controller.GetBodyAsync(payload);
            });
            Assert.Null(problems);
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ControllerCanGetMailMessagesAsync(bool hasUser = true)
        {
            var provider = GetProvider();
            var user = hasUser ? provider.GetRequiredService<User>() : null;
            var search = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var mailbox = provider.GetRequiredService<Mock<IUserMailbox>>();
            var payload = provider.GetRequiredService<MailboxRequest>();
            var bo = new List<EmailListBo>();
            mailbox.Setup(m => m.GetMailMessagesAsync(It.IsAny<MailboxRequest>())).ReturnsAsync(bo);
            search.Setup(m => m.GetUserAsync(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            var controller = provider.GetRequiredService<MailboxController>();
            var problems = await Record.ExceptionAsync(async () =>
            {
                _ = await controller.GetMailMessagesAsync(payload);
            });
            Assert.Null(problems);
        }
        private static IServiceProvider GetProvider()
        {
            var service = new ServiceCollection();
            var mqSearch = new Mock<ISearchInfrastructure>();
            var mqMail = new Mock<IUserMailbox>();
            var payload = faker1.Generate();
            var user = new User { Id = payload.UserId };
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

            service.AddSingleton(user);
            service.AddSingleton(payload);
            service.AddSingleton(mqSearch);
            service.AddSingleton(mqSearch.Object);
            service.AddSingleton(mqMail);
            service.AddSingleton(mqMail.Object);
            service.AddSingleton(request);
            service.AddSingleton(request.Object);
            service.AddSingleton(m =>
            {
                var search = m.GetRequiredService<ISearchInfrastructure>();
                var mail = m.GetRequiredService<IUserMailbox>();
                var controller = new MailboxController(search, mail)
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            return service.BuildServiceProvider();
        }



        private static readonly Faker<MailboxRequest> faker1 =
            new Faker<MailboxRequest>()
            .RuleFor(x => x.RequestType, y => y.PickRandom(_requests))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.MessageId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.LastUpdate, y => y.Date.Recent(60));

        private static readonly List<string> _requests =
            ["body", "count", "messages"];
    }
}