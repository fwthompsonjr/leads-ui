﻿using legallead.jdbc.entities;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace permissions.api.tests.Contollers
{
    public class PaymentControllerTests
    {
        private static readonly List<string> codeNames = "admin,gold,guest,platinum,silver".Split(',').ToList();

        private static readonly Faker<PaymentCode> codefaker =
            new Faker<PaymentCode>()
                .RuleFor(x => x.Admin, y => y.Random.AlphaNumeric(10))
                .RuleFor(x => x.Gold, y => y.Random.AlphaNumeric(10))
                .RuleFor(x => x.Guest, y => y.Random.AlphaNumeric(10))
                .RuleFor(x => x.Platinum, y => y.Random.AlphaNumeric(10))
                .RuleFor(x => x.Silver, y => y.Random.AlphaNumeric(10));

        private static readonly Faker<PaymentStripeOption> optionfaker =
            new Faker<PaymentStripeOption>()
                .RuleFor(x => x.Key, y => y.PickRandom(codeNames))
                .RuleFor(x => x.Codes, y => codefaker.Generate());

        private static readonly Faker<User> userfaker = new Faker<User>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        private static readonly Faker<SearchPreviewBo> previewfaker =
            new Faker<SearchPreviewBo>()
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.FirstName, y => y.Person.FirstName)
            .RuleFor(x => x.LastName, y => y.Person.LastName);

        private static readonly Faker<SearchInvoiceBo> invoicefaker =
            new Faker<SearchInvoiceBo>()
            .RuleFor(x => x.LineId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemType, y => y.Person.FirstName);

        [Fact]
        public void SutCanGetProductCodes()
        {
            var provider = GetProvider();
            var service = provider.GetRequiredService<PaymentController>();
            var result = service.ProductCodes();
            Assert.NotNull(result);
            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [Fact]
        public void SutCanGetSessionStatus()
        {
            var provider = GetProvider();
            var infra = provider.GetRequiredService<Mock<IStripeInfrastructure>>();
            var codes = optionfaker.Generate();
            infra.Setup(s => s.SessionStatus(It.IsAny<string>())).Returns(codes);
            var service = provider.GetRequiredService<PaymentController>();
            var result = service.SessionStatus("123456");
            Assert.NotNull(result);
            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [Theory]
        [InlineData(true, true, true, true, true, true)]
        [InlineData(false, true, true, true, true, true)]
        [InlineData(true, false, true, true, true, true)]
        [InlineData(true, true, false, true, true, true)]
        [InlineData(true, true, true, false, true, true)]
        [InlineData(true, true, true, true, true, false)]
        public async Task SutCanCreatePaymentSession(
            bool hasUser,
            bool hasGuid,
            bool hasPreview,
            bool canCreateInvoice,
            bool hasInvoices,
            bool canCreatePayment)
        {
            var provider = GetProvider();
            var request = new PaymentCreateRequest { Id = Guid.NewGuid().ToString(), ProductType = "Search" };
            User? user = hasUser ? userfaker.Generate() : null;
            List<SearchPreviewBo>? preview = hasPreview ? previewfaker.Generate(3) : null;
            List<SearchInvoiceBo>? invoices = canCreateInvoice ? invoicefaker.Generate(3) : null;
            object? payment = canCreatePayment ? new { PaymentId = "123456789" } : null;
            if (!hasGuid) request.Id = "abc";
            if (canCreateInvoice && !hasInvoices) invoices?.Clear();

            var infra = provider.GetRequiredService<Mock<ISearchInfrastructure>>();
            var stripe = provider.GetRequiredService<Mock<IStripeInfrastructure>>();
            infra.Setup(s => s.GetUser(It.IsAny<HttpRequest>())).ReturnsAsync(user);
            infra.Setup(s => s.GetPreview(It.IsAny<HttpRequest>(), It.IsAny<string>())).ReturnsAsync(preview);
            infra.Setup(s => s.CreateInvoice(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(invoices);
            stripe.Setup(s => s.CreatePaymentAsync(It.IsAny<PaymentCreateModel>(), It.IsAny<List<SearchInvoiceBo>>())).ReturnsAsync(payment);
            var service = provider.GetRequiredService<PaymentController>();
            var result = await service.Create(request);
            Assert.NotNull(result);
            if (!hasUser || !hasGuid) Assert.IsAssignableFrom<UnauthorizedResult>(result);
            if (hasUser && hasGuid && !hasPreview) Assert.IsAssignableFrom<UnprocessableEntityObjectResult>(result);
            if (hasUser && hasGuid && hasPreview && !canCreateInvoice) Assert.IsAssignableFrom<UnprocessableEntityObjectResult>(result);
        }

        private static IServiceProvider GetProvider()
        {
            var service = new ServiceCollection();
            var mqSearch = new Mock<ISearchInfrastructure>();
            var mqStripe = new Mock<IStripeInfrastructure>();
            var option = optionfaker.Generate();
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

            service.AddSingleton(option);
            service.AddSingleton(mqSearch);
            service.AddSingleton(mqSearch.Object);
            service.AddSingleton(mqStripe);
            service.AddSingleton(mqStripe.Object);
            service.AddSingleton(request);
            service.AddSingleton(request.Object);
            service.AddSingleton(m =>
            {
                var opt = m.GetRequiredService<PaymentStripeOption>();
                var search = m.GetRequiredService<ISearchInfrastructure>();
                var stripe = m.GetRequiredService<IStripeInfrastructure>();
                var controller = new PaymentController(opt, search, stripe)
                {
                    ControllerContext = controllerContext
                };
                return controller;
            });
            return service.BuildServiceProvider();
        }
    }
}