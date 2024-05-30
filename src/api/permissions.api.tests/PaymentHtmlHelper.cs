using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using legallead.permissions.api.Utility;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Stripe;

namespace permissions.api.tests
{
    internal static class PaymentHtmlHelper
    {

        public static IServiceProvider GetProvider()
        {
            var services = new ServiceCollection();
            var repo = new Mock<IUserSearchRepository>();
            var subscriptionDb = new Mock<ISubscriptionInfrastructure>();
            var custDb = new Mock<ICustomerInfrastructure>();
            var userDb = new Mock<IUserRepository>();
            var stripeConfig = new Mock<StripeKeyEntity>();
            var stripe = new Mock<IStripeInfrastructure>();
            var subscription = new Mock<SubscriptionService>();
            var paymentKey = KeyFaker.Generate();
            var custRepo = new Mock<ICustomerRepository>();
            stripeConfig.Setup(x => x.GetActiveName()).Returns(paymentKey.PaymentKey);
            services.AddSingleton(repo);
            services.AddSingleton(stripe);
            services.AddSingleton(subscriptionDb);
            services.AddSingleton(subscription);
            services.AddSingleton(custDb);
            services.AddSingleton(userDb);
            services.AddSingleton(custRepo);
            services.AddSingleton(stripeConfig.Object);
            services.AddSingleton(stripe.Object);
            services.AddSingleton(repo.Object);
            services.AddSingleton(subscriptionDb.Object);
            services.AddSingleton(subscription.Object);
            services.AddSingleton(custDb.Object);
            services.AddSingleton(userDb.Object);
            services.AddSingleton(custRepo.Object);
            services.AddSingleton(x =>
            {
                var repo = x.GetRequiredService<IUserSearchRepository>();
                var subscriptionDb = x.GetRequiredService<ISubscriptionInfrastructure>();
                var custDb = x.GetRequiredService<ICustomerInfrastructure>();
                var userDb = x.GetRequiredService<IUserRepository>();
                var stripeConfig = x.GetRequiredService<StripeKeyEntity>();
                var stripe = x.GetRequiredService<IStripeInfrastructure>();
                var custRpo = x.GetRequiredService<ICustomerRepository>();
                var translator = new PaymentHtmlTranslator(repo, userDb, custDb, subscriptionDb, stripe, stripeConfig, custRpo);
                var subscription = x.GetRequiredService<Mock<SubscriptionService>>();
                translator.SetupSubscriptionService(subscription.Object);
                return translator;
            });
            return services.BuildServiceProvider();
        }

        public static SearchIsPaidDto? GenerateSearchIsPaid(bool? isPaid, bool? isDownloaded)
        {
            if (isPaid == null) return null;
            var guid = LevelFaker.Generate().Id ?? Guid.NewGuid().ToString();
            return new()
            {
                Id = guid,
                IsDownloaded = isDownloaded,
                IsPaid = isPaid
            };
        }

        private static readonly Faker<PaymentKeyWrapper> KeyFaker
            = new Faker<PaymentKeyWrapper>()
            .RuleFor(x => x.PaymentKey, y => y.Random.AlphaNumeric(8));


        public static readonly Faker<SearchInvoiceBo> InvoiceFaker
            = new Faker<SearchInvoiceBo>()
            .RuleFor(x => x.LineId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemType, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 5))
            .RuleFor(x => x.UnitPrice, y => y.Random.Decimal(1, 10))
            .RuleFor(x => x.ReferenceId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .FinishWith((a, b) =>
            {
                b.Price = b.UnitPrice.GetValueOrDefault() * b.ItemCount.GetValueOrDefault();
            });


        public static readonly Faker<PaymentSessionJs> PaymentFaker = new Faker<PaymentSessionJs>()
            .RuleFor(x => x.Description, y => y.Person.UserName)
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.SuccessUrl, y => y.Random.Guid().ToString())
            .FinishWith((a, b) =>
            {
                var invoice = InvoiceFaker.Generate(a.Random.Int(2, 6));
                b.Data = invoice;
            });
        public static readonly Faker<PaymentSessionDto> SessionFaker
            = new Faker<PaymentSessionDto>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.UserId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.InvoiceId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.SessionType, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.SessionId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.IntentId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.ClientId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.ExternalId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.JsText, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .FinishWith((a, b) =>
            {
                var data = PaymentFaker.Generate();
                b.JsText = JsonConvert.SerializeObject(data);
            });

        public static readonly Faker<LevelRequestBo> LevelFaker
            = new Faker<LevelRequestBo>()
            .RuleFor(x => x.Id, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.UserId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.InvoiceUri, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.LevelName, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.SessionId, y => y.Random.AlphaNumeric(8))
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        public static readonly Faker<DownloadResetRequest> DownloadRequestFaker
            = new Faker<DownloadResetRequest>()
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ExternalId, y => y.Random.AlphaNumeric(8));

        public static readonly Faker<SearchFinalBo> SearchFinalFaker = new Faker<SearchFinalBo>()
            .RuleFor(x => x.Plantiff, y => y.Person.FullName)
            .RuleFor(x => x.CaseNumber, y => y.Random.AlphaNumeric(15))
            .RuleFor(x => x.DateFiled, y => y.Date.Recent().ToString("MM/dd/yyyy"))
            .RuleFor(x => x.Address1, y => y.Person.Address.Street)
            .RuleFor(x => x.Address2, y =>
            {
                var isEmpty = y.Random.Bool();
                if (isEmpty) return string.Empty;
                return y.Person.Address.Suite;
            })
            .RuleFor(x => x.Address3, y =>
            {
                var address = y.Person.Address;
                return $"{address.City}, {address.State} {address.ZipCode}";
            });


        public static readonly Faker<DiscountRequestBo> DiscountBoFaker =
            new Faker<DiscountRequestBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.InvoiceUri, y => y.Hacker.Phrase())
            .RuleFor(x => x.LevelName, y => y.Hacker.Phrase())
            .RuleFor(x => x.SessionId, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        public static readonly Faker<PurchaseSummaryDto> PurchaseSummaryFaker =
            new Faker<PurchaseSummaryDto>()
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Price, y => y.Random.Int(1, 25))
            .RuleFor(x => x.ItemType, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .RuleFor(x => x.UserName, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.Email, y => y.Random.Guid().ToString("D"));

        public static readonly Faker<User> UserFaker = new Faker<User>()
            .RuleFor(x => x.UserName, y => y.Person.UserName)
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.Email, y => y.Person.Email)
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        public static readonly string[] requestNames = ["success", "cancel"];
        public static readonly string[] PermissionNames = ["guest", "gold", "silver", "platinum"];
    }
}
