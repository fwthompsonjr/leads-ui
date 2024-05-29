using legallead.jdbc.interfaces;
using legallead.permissions.api.Custom;
using Polly;
using Stripe;

namespace legallead.permissions.api.Utility
{
    internal static class StripeSubscriptionRetryService
    {
        public async static Task<Tuple<bool, string, Invoice>> VerifySubscription(
            LevelRequestBo requested, 
            ICustomerRepository? customerDb = null, 
            string paymentType = "Monthly")
        {
            var failure = new Tuple<bool, string, Invoice>(true, dash, new() { Total = 0 });
            try
            {
                var sessionId = requested.SessionId;
                var expected = customerDb == null ? [] : (await customerDb.GetLevelRequestPaymentAmount(requested.ExternalId ?? string.Empty) ?? []);
                var paymentData = expected.Find(x => x.PriceType == paymentType);
                var invoiceData = FetchLatestInvoice(paymentData, sessionId);
                return invoiceData;
            }
            catch
            {
                return failure;
            }
        }
        
        private static Tuple<bool, string, Invoice> FetchLatestInvoice(LevelPaymentBo? payment, string? sessionId)
        {
            const double wait = 300f;
            var intervals = new[] {
                TimeSpan.FromMilliseconds(wait),
                TimeSpan.FromMilliseconds(wait * 2f),
                TimeSpan.FromMilliseconds(wait * 4f),
                TimeSpan.FromMilliseconds(wait * 6f)
            };
            if (string.IsNullOrEmpty(sessionId)) { throw new ArgumentOutOfRangeException(nameof(sessionId)); }
            var response = Policy.Handle<Exception>()
                .WaitAndRetry(intervals)
                .Execute(() =>
                {
                    return GetInvoiceData(payment, sessionId);
                });
            return response;
        }

        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party. Integration testing only")]
        private static Tuple<bool, string, Invoice> GetInvoiceData(LevelPaymentBo? payment, string sessionId)
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
            if (amount != actual) throw new InvoiceAmountMismatchedException();
            return data;
        }

        private const string dash = " - ";
    }
}
