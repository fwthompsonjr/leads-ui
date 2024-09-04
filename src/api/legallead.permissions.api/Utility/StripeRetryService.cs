using legallead.permissions.api.Custom;
using Polly;
using Stripe;

namespace legallead.permissions.api.Utility
{
    [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party. Integration testing only")]
    internal static class StripeRetryService
    {
        public static async Task<string> FetchClientSecretAsync(LevelRequestBo session)
        {
            const double wait = 400f;
            var nodata = Guid.Empty.ToString("D");
            try
            {
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
                        var secret = await FetchClientSecretValueAsync(session);
                        return secret;
                    });
                return response;
            }
            catch (Exception)
            {
                return nodata;
            }
        }

        private static async Task<string> FetchClientSecretValueAsync(LevelRequestBo session)
        {
            var service = new SubscriptionService();
            var subscription = await service.GetAsync(session.SessionId) ?? throw new SubscriptionNotFoundException();
            var invoiceId = subscription.LatestInvoiceId;
            var invoiceSvc = new InvoiceService();
            var invoice = await invoiceSvc.GetAsync(invoiceId) ?? throw new InvoiceNotFoundException();
            var intentSvc = new PaymentIntentService();
            if (invoice.PaymentIntentId == null) throw new PaymentIntentNotFoundException();
            var intent = await intentSvc.GetAsync(invoice.PaymentIntentId);
            return intent.ClientSecret;
        }
    }
}
