using legallead.jdbc.entities;
using legallead.permissions.api.Models;
using Stripe.Checkout;
using System.Security.Cryptography;

namespace legallead.permissions.api.Utility
{
    public partial class SubscriptionInfrastructure
    {
        public async Task<LevelRequestBo> GeneratePermissionSession(HttpRequest request, User user, string level, string externalId = "")
        {
            var findSession = await IsPermissionSessionNeeded(user, level, externalId);
            if (findSession != null && findSession.IsPaymentSuccess.GetValueOrDefault()) return findSession;
            if (_customer == null || _payment == null) return new();
            var cust = await _customer.GetOrCreateCustomer(user.Id);
            if (cust == null || string.IsNullOrEmpty(cust.CustomerId)) return new();
            var successUrl = $"{request.Scheme}://{request.Host}/subscription-result?session_id=~1&sts=success&id=~0"
                .Replace("~1", "{CHECKOUT_SESSION_ID}");
            LevelChangeRequest changeRq = await GetPaymentSession(user, level, externalId, cust, successUrl);
            var bo = (await _customer.AddLevelChangeRequest(changeRq)) ?? new();
            return bo;
        }

        private async Task<LevelChangeRequest> GetPaymentSession(User user, string level, string externalId, PaymentCustomerBo cust, string successUrl)
        {
            if (_customer == null || _payment == null) return new();
            try
            {
                var cancelUrl = successUrl.Replace("sts=success", "sts=cancel");
                var priceId = GetPermissionCode(level, _payment);
                var options = new SessionCreateOptions
                {
                    // See https://stripe.com/docs/api/checkout/sessions/create
                    // for additional parameters to pass.
                    // {CHECKOUT_SESSION_ID} is a string literal; do not change it!
                    // the actual Session ID is returned in the query parameter when your customer
                    // is redirected to the success page.
                    SuccessUrl = successUrl,
                    CancelUrl = cancelUrl,
                    Mode = "subscription",
                    Customer = cust.CustomerId,
                    LineItems = new List<SessionLineItemOptions>
                  {
                    new() {

                      Price = priceId,
                      // For metered billing, do not pass quantity
                      Quantity = 1,
                    },
                  },
                };
                var service = new SessionService();
                var session = await service.CreateAsync(options);
                var changeRq = new LevelChangeRequest
                {
                    ExternalId = externalId,
                    InvoiceUri = session.Url,
                    LevelName = level,
                    SessionId = session.Id,
                    UserId = user.Id
                };
                return changeRq;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new();
            }
        }

        private static string GetPermissionCode(string level, PaymentStripeOption payment)
        {
            var lower = level.ToLower().Trim();
            var pricing = PricingLookupService.PricingCodes.Find(x => (x.KeyName ?? "").Contains(level, StringComparison.OrdinalIgnoreCase));
            if (pricing != null && !string.IsNullOrEmpty(pricing.PriceCodeMonthly)) return pricing.PriceCodeMonthly;
            var code = lower switch
            {
                "admin" => payment.Codes.Admin,
                "platinum" => payment.Codes.Platinum,
                "gold" => payment.Codes.Gold,
                "silver" => payment.Codes.Silver,
                "guest" => payment.Codes.Guest,
                _ => string.Empty
            };
            return code;
        }

        private async Task<LevelRequestBo?> IsPermissionSessionNeeded(User user, string level, string externalId = "")
        {
            string[] nonbillingcodes = new[] { "admin", "guest" };
            if (string.IsNullOrEmpty(level)) { return null; }
            var code = _payment == null ? string.Empty : GetPermissionCode(level, _payment);
            if (string.IsNullOrEmpty(code)) return null;
            if (string.IsNullOrEmpty(externalId)) externalId = PermissionsKey();
            if (string.IsNullOrEmpty(user.Id)) { return null; }
            // try to get payment session by external id

            // if level = guest or admin then write level request and return false
            if (nonbillingcodes.Contains(level, StringComparer.OrdinalIgnoreCase) && _customer != null)
            {
                Console.WriteLine("Creating level request as completed.");
                var request = new LevelChangeRequest
                {
                    ExternalId = externalId,
                    InvoiceUri = "NONE",
                    LevelName = level.ToLower(),
                    SessionId = "NONE",
                    UserId = user.Id
                };
                var session = await _customer.AddLevelChangeRequest(request);
                return session;
            }
            return new LevelRequestBo { ExternalId = externalId };
        }

        private static string PermissionsKey()
        {
            string allowed = "ABCDEFGHIJKLMONOPQRSTUVWXYZabcdefghijklmonopqrstuvwxyz0123456789";
            int strlen = 16;
            char[] randomChars = new char[strlen];

            for (int i = 0; i < strlen; i++)
            {
                randomChars[i] = allowed[RandomNumberGenerator.GetInt32(0, allowed.Length)];
            }

            return new string(randomChars);
        }
    }
}
