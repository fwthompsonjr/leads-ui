using legallead.jdbc.entities;
using legallead.permissions.api.Controllers;
using legallead.permissions.api.Interfaces;

namespace permissions.api.tests.Contollers
{
    public class SubscriptionCheckoutTests
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
            var controller = new SubscriptionTestContoller();
            var request = testIndex == 0 ? null : controller.GetLevelRequest();
            controller.MqStripeSvc.Setup(x => x.FetchClientSecretValueAsync(It.IsAny<LevelRequestBo>())).ReturnsAsync(guid);
            controller.MqSecretSvc.Setup(x => x.GetSubscriptionSecret(It.IsAny<LevelRequestBo>(), It.IsAny<string>())).Returns(response);
            var uid = controller.GetSecret(request);
            Assert.NotNull(uid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task SutCanDiscountCheckoutAsync(int testIndex, string? sts = "success", string? id = "012345")
        {
            var errored = await Record.ExceptionAsync(async () =>
            {
                var response = new Faker().Random.AlphaNumeric(12);
                var guid = testIndex == 2 ? Guid.Empty.ToString("D") : response;
                var controller = new SubscriptionTestContoller();
                var request = testIndex == 0 ? null : controller.GetLevelRequest();
                var ispaid = testIndex != 4;
                controller.MqHtml.Setup(x => x.IsChangeUserLevelAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                    )).ReturnsAsync(true);
                controller.MqHtml.Setup(x => x.IsSubscriptionValidAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>()
                    )).ReturnsAsync(request);
                controller.MqHtml.Setup(x => x.IsRequestPaidAsync(
                    It.IsAny<LevelRequestBo>()
                    )).ReturnsAsync(ispaid);
                if (testIndex != 5)
                {
                    controller.MqHtml.Setup(x => x.Transform(
                        It.IsAny<LevelRequestBo>(),
                        It.IsAny<string>()
                        )).Returns(response);
                }
                controller.MqHtml.Setup(x => x.TransformForPermissionsAsync(
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                    )).ReturnsAsync(response);
                controller.MqStripeSvc.Setup(x => x.FetchClientSecretValueAsync(It.IsAny<LevelRequestBo>())).ReturnsAsync(guid);
                controller.MqSecretSvc.Setup(x => x.GetSubscriptionSecret(
                    It.IsAny<LevelRequestBo>(),
                    It.IsAny<string>())).Returns(response);
                _ = await controller.SubscriptionCheckoutAsync(sts, id);
            });
            Assert.Null(errored);
        }

        private sealed class SubscriptionTestContoller : HomeController
        {
            public SubscriptionTestContoller() : base(
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
                return GetSubscriptionSecret(session);
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