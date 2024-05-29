using Polly;
using Stripe;

namespace legallead.permissions.api.Utility
{
    [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party. Integration testing only")]
    internal static class StripeRetryService
    {
        public static async Task<string> FetchClientSecret(LevelRequestBo session)
        {
            const double wait = 150f;
            var intervals = new[] {
                TimeSpan.FromMilliseconds(wait),
                TimeSpan.FromMilliseconds(wait * 2f),
                TimeSpan.FromMilliseconds(wait * 4f),
                TimeSpan.FromMilliseconds(wait * 6f)
            };
            string response = await Policy.Handle<Exception>()
                .WaitAndRetryAsync(intervals)
                .ExecuteAsync(async () =>
                {
                    var secret = await FetchClientSecretValue(session);
                    return secret;
                });
            return response;
        }

        private static async Task<string> FetchClientSecretValue(LevelRequestBo session)
        {
            var nodata = Guid.Empty.ToString("D");
            var service = new SubscriptionService();
            var subscription = await service.GetAsync(session.SessionId);
            if (subscription == null) return nodata;
            var invoiceId = subscription.LatestInvoiceId;
            var invoiceSvc = new InvoiceService();
            var invoice = await invoiceSvc.GetAsync(invoiceId);
            if (invoice == null) return nodata;
            var intentSvc = new PaymentIntentService();
            var intent = await intentSvc.GetAsync(invoice.PaymentIntentId);
            return intent.ClientSecret;
        }
    }
}
