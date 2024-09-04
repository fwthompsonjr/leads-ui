using legallead.jdbc.entities;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;

namespace permissions.api.tests.Contollers
{
    public class DiscountCheckoutTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void SutCanGetSecret(int testIndex)
        {
            var response = new Faker().Random.AlphaNumeric(12);
            var guid = testIndex == 2 ? Guid.Empty.ToString("D") : response;
            var controller = new DiscountTestController();
            var request = testIndex == 0 ? null : controller.GetLevelRequest();
            controller.MqStripeSvc.Setup(x => x.FetchClientSecretValueAsync(It.IsAny<LevelRequestBo>())).ReturnsAsync(guid);
            controller.MqSecretSvc.Setup(x => x.GetDiscountSecret(It.IsAny<DiscountRequestBo>(), It.IsAny<string>())).Returns(response);
            var uid = controller.GetSecret(request);
            Assert.NotNull(uid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task SutCanDiscountCheckoutAsync(int testIndex, string? sts = "success", string? id = "012345")
        {
            var errored = await Record.ExceptionAsync(async () =>
            {
                var response = new Faker().Random.AlphaNumeric(12);
                var guid = testIndex == 2 ? Guid.Empty.ToString("D") : response;
                var controller = new DiscountTestController();
                var request = testIndex == 0 ? null : controller.GetLevelRequest();
                var ispaid = testIndex != 4;
                controller.MqHtml.Setup(x => x.IsDiscountLevelAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                    )).ReturnsAsync(true);
                controller.MqHtml.Setup(x => x.IsDiscountValidAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                    )).ReturnsAsync(request);
                controller.MqHtml.Setup(x => x.IsDiscountPaidAsync(
                    It.IsAny<LevelRequestBo>()
                    )).ReturnsAsync(ispaid);
                controller.MqHtml.Setup(x => x.Transform(
                    It.IsAny<DiscountRequestBo>(),
                    It.IsAny<string>()
                    )).Returns(response);
                controller.MqHtml.Setup(x => x.TransformForDiscountsAsync(
                    It.IsAny<ISubscriptionInfrastructure>(),
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                    )).ReturnsAsync(response);
                controller.MqStripeSvc.Setup(x => x.FetchClientSecretValueAsync(It.IsAny<LevelRequestBo>())).ReturnsAsync(guid);
                controller.MqSecretSvc.Setup(x => x.GetDiscountSecret(
                    It.IsAny<DiscountRequestBo>(),
                    It.IsAny<string>())).Returns(response);
                _ = await controller.DiscountCheckoutAsync(sts, id);
            });
            Assert.Null(errored);
        }

        private sealed class DiscountTestController : HomeController
        {
            public DiscountTestController() : base(
                    html.Object,
                    infrastructure.Object,
                    subscription.Object,
                    lockdb.Object,
                    stripeSvcs.Object,
                    secretSvc.Object)
            {
                lock (locker)
                {
                    html.Reset();
                    infrastructure.Reset();
                    subscription.Reset();
                    lockdb.Reset();
                    stripeSvcs.Reset();
                    secretSvc.Reset();
                }
            }

            public string GetSecret(LevelRequestBo? session)
            {
                return GetDiscountSecret(session);
            }
            public LevelRequestBo GetLevelRequest()
            {
                return levelBofaker.Generate();
            }

            public Mock<IPaymentHtmlTranslator> MqHtml => html;
            public Mock<ISearchInfrastructure> MqInfrastructure => infrastructure;
            public Mock<ISubscriptionInfrastructure> MqSubscription => subscription;
            public Mock<ICustomerLockInfrastructure> MqLockDb => lockdb;
            public Mock<IStripeInfrastructure> MqStripeSvc => stripeSvcs;
            public Mock<IClientSecretService> MqSecretSvc => secretSvc;

            private static readonly object locker = new();
            private static readonly Mock<IPaymentHtmlTranslator> html = new();
            private static readonly Mock<ISearchInfrastructure> infrastructure = new();
            private static readonly Mock<ISubscriptionInfrastructure> subscription = new();
            private static readonly Mock<ICustomerLockInfrastructure> lockdb = new();
            private static readonly Mock<IStripeInfrastructure> stripeSvcs = new();
            private static readonly Mock<IClientSecretService> secretSvc = new();

            private static readonly Faker<LevelRequestBo> levelBofaker =
                new Faker<LevelRequestBo>()
                .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
                .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
                .RuleFor(x => x.CustomerId, y => y.Random.Guid().ToString("D"))
                .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
                .RuleFor(x => x.InvoiceUri, y => y.Hacker.Phrase())
                .RuleFor(x => x.LevelName, y => y.Hacker.Phrase())
                .RuleFor(x => x.SessionId, y => y.Hacker.Phrase())
                .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
                .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
                .RuleFor(x => x.CreateDate, y => y.Date.Recent());
        }
    }
}
