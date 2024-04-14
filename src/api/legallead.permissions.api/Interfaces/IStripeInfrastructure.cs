using legallead.jdbc.entities;
using legallead.permissions.api.Models;
using Stripe;

namespace legallead.permissions.api.Interfaces
{
    public interface IStripeInfrastructure
    {
        object SessionStatus(string sessionId);
        Task<object?> CreatePaymentAsync(
            PaymentCreateModel model,
            List<SearchInvoiceBo> data);
        Task<object> FetchClientSecret(LevelRequestBo session);
        Tuple<bool, string, Invoice> VerifySubscription(string sessionId);
    }
}
