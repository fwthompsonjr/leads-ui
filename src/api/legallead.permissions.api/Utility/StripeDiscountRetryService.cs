using legallead.jdbc.interfaces;
using legallead.permissions.api.Custom;
using legallead.permissions.api.Models;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Polly;
using Stripe;
using Stripe.Checkout;

namespace legallead.permissions.api.Utility
{
    internal static class StripeDiscountRetryService
    {
        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party service")]
        public static async Task<object?> CreatePaymentAsync(
            DiscountRequestBo requested,
            string paymentType = "Monthly",
            ICustomerRepository? customerDb = null,
            IUserRepository? userDb = null,
            IUserSearchRepository? searchDb = null
            )
        {
            var description = "Discount Modification Request";
            var externalId = requested.ExternalId ?? string.Empty;
            var data = customerDb == null ? [] : (await customerDb.GetDiscountRequestPaymentAmount(externalId) ?? []);
            var amount = data.Sum(x => x.Price.GetValueOrDefault(0));
            const decimal minAmount = 0.50m;
            if (amount <= minAmount)
            {
                // close invoice as paid
                // Set Discount Purchase Date by externalId
                var nocost = new
                {
                    Id = Guid.Empty.ToString("D"),
                    PaymentIntentId = Guid.Empty.ToString("D"),
                    clientSecret = string.Empty,
                    externalId,
                    description,
                    data
                };
                return nocost;
            }
            var user = GetUserOrDefault(requested.UserId ?? string.Empty, userDb);
            var intent = CreatePaymentIntent(amount);
            var successPg = (requested.InvoiceUri ?? string.Empty).Replace("discount-checkout", $"discount-result?id={externalId}&sts=success");
            var failurePg = successPg.Replace("success", "cancel");
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes =
                [
                    "card"
                ],
                LineItems = [],
                Metadata = new Dictionary<string, string>
                {
                    { "external-id", externalId },
                    { "product-type", "Discount" }
                },
                Mode = "payment",
                AutomaticTax = new SessionAutomaticTaxOptions { Enabled = false },
                SuccessUrl = successPg,
                CancelUrl = failurePg,
            };
            data.ForEach(d =>
            {
                var item = PricingConverter.ConvertFrom(d, user);
                options.LineItems.Add(item);
            });
            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            session.PaymentIntent = intent;
            var response = new 
            {
                ExternalId = externalId,
                Description = description,
                SuccessUrl = successPg,
                Data = data
            };
            var js = JsonConvert.SerializeObject(response);
            var payment = new PaymentSessionDto
            {
                Id = Guid.NewGuid().ToString("D"),
                UserId = user.Id,
                SessionId = session.Id,
                SessionType = "Discount",
                IntentId = intent.Id,
                ClientId = intent.ClientSecret,
                ExternalId = response.ExternalId,
                JsText = js
            };
            if (searchDb == null) return response;
            var isadded = await searchDb.AppendPaymentSession(payment);
            if (!isadded) return null;
            return response;
        }
        public async static Task<Tuple<bool, string, Invoice>> VerifySubscription(
            DiscountRequestBo requested,
            ICustomerRepository? customerDb = null,
            string paymentType = "Monthly")
        {
            var failure = new Tuple<bool, string, Invoice>(true, dash, new() { Total = 0 });
            try
            {
                var sessionId = requested.SessionId;
                var expected = customerDb == null ? [] : (await customerDb.GetDiscountRequestPaymentAmount(requested.ExternalId ?? string.Empty) ?? []);
                var paymentData = expected.FindAll(x => x.PriceType == paymentType);
                var invoiceData = FetchLatestInvoice(paymentData, sessionId);
                return invoiceData;
            }
            catch
            {
                return failure;
            }
        }

        private static Tuple<bool, string, Invoice> FetchLatestInvoice(List<DiscountPaymentBo>? list, string? sessionId)
        {
            const double wait = 300f;
            var intervals = new[] {
                TimeSpan.FromMilliseconds(wait),
                TimeSpan.FromMilliseconds(wait * 2f),
                TimeSpan.FromMilliseconds(wait * 4f),
                TimeSpan.FromMilliseconds(wait * 6f)
            };
            if (string.IsNullOrEmpty(sessionId)) { throw new ArgumentOutOfRangeException(nameof(sessionId)); }
            var payment = list?.FirstOrDefault();
            if (list != null && payment != null) { payment.Price = list.Sum(x => x.Price); }
            var response = Policy.Handle<Exception>()
                .WaitAndRetry(intervals)
                .Execute(() =>
                {
                    return GetInvoiceData(payment, sessionId);
                });
            return response;
        }

        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party. Integration testing only")]
        private static Tuple<bool, string, Invoice> GetInvoiceData(DiscountPaymentBo? payment, string sessionId)
        {
            var service = new SubscriptionService();
            var subscription = service.Get(sessionId) ?? throw new SubscriptionNotFoundException();
            var invoiceId = subscription.LatestInvoiceId;
            var invoiceSvc = new InvoiceService();
            var invoice = invoiceSvc.Get(invoiceId) ?? throw new InvoiceNotFoundException();
            _ = subscription.Metadata.TryGetValue("SuccessUrl", out string? successUrl);
            successUrl ??= dash;
            var amount = payment?.Price.GetValueOrDefault(0) ?? 0;
            var actual = invoice.Total * 0.01m;
            var data = new Tuple<bool, string, Invoice>(true, successUrl, invoice);
            if (payment == null || amount == 0) return data;
            if (amount != actual)
            {
                var pmt = Convert.ToInt64(amount * 100m);
                return new Tuple<bool, string, Invoice>(true, successUrl, new() { Total = pmt });
            }
            return data;
        }

        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party service")]
        private static PaymentIntent CreatePaymentIntent(decimal amount, string customerId = "")
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = Convert.ToInt64(amount * 100),
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };
            if (!string.IsNullOrEmpty(customerId)) { options.Customer = customerId; }
            var service = new PaymentIntentService();
            var intent = service.Create(options);
            return intent;
        }

        private static User GetUserOrDefault(string userId, IUserRepository? userDb)
        {
            var fallback = new User { Id = userId };
            if (userDb == null) return fallback;
            return userDb.GetById(userId).GetAwaiter().GetResult() ?? fallback;
        }

        private const string dash = " - ";
    }
}