using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using Stripe;
using Stripe.Checkout;

namespace legallead.permissions.api.Utility
{
    public class StripeInfrastructure : IStripeInfrastructure
    {
        private readonly IUserSearchRepository _repo;
        public StripeInfrastructure(IUserSearchRepository repo)
        {
            _repo = repo;
        }
        public async Task<object> CreatePaymentAsync(
            PaymentCreateModel model, List<SearchInvoiceBo> data)
        {
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
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>(),
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
            var response = new
            {
                session.Id,
                PaymentIntentId = intent.Id,
                clientSecret = intent.ClientSecret,
                externalId = data[0].ExternalId ?? string.Empty,
                description,
                data
            };
            return response;
        }

        public object SessionStatus(string sessionId)
        {
            var sessionService = new SessionService();
            Session session = sessionService.Get(sessionId);
            return new
            {
                status = session.RawJObject["status"],
                customer_email = session.RawJObject["customer_details"]["email"]
            };
        }


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
            return service.Create(options);
        }
    }
}
