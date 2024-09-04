using legallead.jdbc.entities;

namespace permissions.api.tests.Utility
{
    using ApiResource = legallead.permissions.api.Properties.Resources;
    public class TransformForPermissionsTests
    {
        [Theory]
        [InlineData(true, "success", "12345")]
        [InlineData(false, "success", "12345")]
        [InlineData(true, "success", "12345", false)]
        [InlineData(true, "success", "12345", true, false)]
        public async Task SutCanExecuteTransformForPermissionsAsync(
            bool isvalid,
            string? status,
            string? id,
            bool hasDiscount = true,
            bool hasUser = true)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var builder = new PaymentHtmlTranslatorBuilder();
                LevelRequestBo? apiResponse = hasDiscount ? PaymentHtmlHelper.LevelFaker.Generate() : null;
                User? user = hasUser ? PaymentHtmlHelper.UserFaker.Generate() : null;
                var summary = PaymentHtmlHelper.PurchaseSummaryFaker.Generate();
                var permission = new KeyValuePair<bool, string>(true, "unit test response");
                var svc = builder.MockCustDb;
                var userDb = builder.MockUserDb;
                var searchDb = builder.MockRepo;
                var subscriptionDb = builder.MockSubscriptionDb;
                var service = builder.Translator;
                if (apiResponse != null)
                {
                    apiResponse.LevelName = new Faker().PickRandom(PaymentHtmlHelper.PermissionNames);
                }
                svc.Setup(m => m.GetLevelRequestByIdAsync(It.IsAny<string>())).ReturnsAsync(apiResponse);
                svc.Setup(m => m.CompleteLevelRequestAsync(It.IsAny<LevelRequestBo>())).ReturnsAsync(apiResponse);
                userDb.Setup(m => m.GetById(It.IsAny<string>())).ReturnsAsync(user);
                searchDb.Setup(m => m.GetPurchaseSummary(It.IsAny<string>())).ReturnsAsync(summary);
                subscriptionDb.Setup(m => m.SetPermissionGroupAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(permission);
                _ = await service.TransformForPermissionsAsync(isvalid, status, id, levelChangeHtml);
            });
            Assert.Null(error);
        }
        private static readonly string levelChangeHtml = ApiResource.page_invoice_subscription_html;
    }
}