﻿using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Models;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;

namespace legallead.permissions.api.Utility
{
    [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party service")]
    public class StripeInfrastructure(
        IUserSearchRepository repo,
        StripeKeyEntity entity) : IStripeInfrastructure
    {
        private readonly IUserSearchRepository _repo = repo;
        private readonly StripeKeyEntity keyEntity = entity;

        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party service")]
        public async Task<object?> CreatePaymentAsync(
            PaymentCreateModel model, List<SearchInvoiceBo> data)
        {
            var existing = await GetPaymentSessionAsync(model);
            if (existing != null) return existing;
            var description = (await _repo.InvoiceDescription(model.SearchId)).ItemDescription;
            var externalId = data[0].ExternalId ?? model.SearchId;
            var amount = data.Sum(x => x.Price.GetValueOrDefault(0));
            const decimal minAmount = 0.50m;
            if (amount <= minAmount)
            {
                // close invoice as paid
                await _repo.SetInvoicePurchaseDate(externalId);
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
            var user = model.CurrentUser;
            var intent = CreatePaymentIntent(amount);
            var successPg = model.SuccessUrlFormat.Replace("~0", externalId);
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
                    { "user-name", user.UserName },
                    { "user-email", user.Email },
                    { "external-id", data[0].ExternalId ?? string.Empty },
                    { "product-type", model.ProductType }
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
            var response = new PaymentSessionJs
            {
                ExternalId = data[0].ExternalId ?? string.Empty,
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
                SessionType = keyEntity.ActiveName,
                IntentId = intent.Id,
                ClientId = intent.ClientSecret,
                ExternalId = response.ExternalId,
                JsText = js
            };
            var isadded = await _repo.AppendPaymentSession(payment);
            if (!isadded) return null;
            return response;
        }

        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party service")]
        public object SessionStatus(string sessionId)
        {
            try
            {

                var sessionService = new SessionService();
                Session session = sessionService.Get(sessionId);
                return new
                {
                    status = session.RawJObject["status"],
                    customer_email = session.RawJObject["customer_details"]?["email"]
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    status = "none",
                    customer_email = "",
                    message = ex.Message
                };
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Using 3rd resources that should not be invoked from unit tests.")]
        public async Task<object> FetchClientSecretAsync(LevelRequestBo session)
        {
            var data = await FetchClientSecretValueAsync(session);
            return new { clientSecret = data };
        }

        [ExcludeFromCodeCoverage(Justification = "Using 3rd resources that should not be invoked from unit tests.")]
        public async Task<string> FetchClientSecretValueAsync(LevelRequestBo session)
        {
            var nodata = Guid.Empty.ToString("D");
            try
            {
                var actual = await StripeRetryService.FetchClientSecretAsync(session);
                return actual;
            }
            catch
            {
                return nodata;
            }
        }
        [ExcludeFromCodeCoverage(Justification = "Using 3rd resources that should not be invoked from unit tests.")]
        public Tuple<bool, string, Invoice> VerifySubscription(string sessionId)
        {
            const string dash = " - ";
            var failure = new Tuple<bool, string, Invoice>(true, dash, new() { Total = 0 });
            try
            {
                var service = new SubscriptionService();
                var subscription = service.Get(sessionId);
                if (subscription == null) return failure;
                var invoiceId = subscription.LatestInvoiceId;
                var invoiceSvc = new InvoiceService();
                var invoice = invoiceSvc.Get(invoiceId);
                if (invoice == null) return failure;
                _ = subscription.Metadata.TryGetValue("SuccessUrl", out string? successUrl);
                successUrl ??= dash;
                return new Tuple<bool, string, Invoice>(true, successUrl, invoice);
            }
            catch
            {
                return failure;
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private member testing through publicly exposed method.")]
        private async Task<object?> GetPaymentSessionAsync(PaymentCreateModel model)
        {
            var isPaid = await _repo.IsSearchPurchased(model.SearchId);
            if (!isPaid.GetValueOrDefault()) { return null; }
            var search = await _repo.Invoices(model.CurrentUser.Id, model.SearchId);
            if (search == null || !search.Any() || string.IsNullOrEmpty(search.First().ExternalId)) return null;
            var externalId = search.First().ExternalId ?? string.Empty;
            var session = await _repo.GetPaymentSession(externalId);
            if (session == null || string.IsNullOrEmpty(session.JsText)) return null;
            var response = JsonConvert.DeserializeObject<PaymentSessionJs>(session.JsText) ?? new();
            return response;
        }

        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party service")]
        private static PaymentIntent CreatePaymentIntent(decimal amount)
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
            var service = new PaymentIntentService();
            var intent = service.Create(options);
            return intent;
        }
    }
}
