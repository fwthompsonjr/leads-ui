﻿using legallead.permissions.api.Models;
using Stripe;

namespace legallead.permissions.api.Interfaces
{
    public interface IStripeInfrastructure
    {
        object SessionStatus(string sessionId);
        Task<object?> CreatePaymentAsync(
            PaymentCreateModel model,
            List<SearchInvoiceBo> data);
        Task<object> FetchClientSecretAsync(LevelRequestBo session);
        Tuple<bool, string, Invoice> VerifySubscription(string sessionId);
        Task<string> FetchClientSecretValueAsync(LevelRequestBo session);
    }
}
