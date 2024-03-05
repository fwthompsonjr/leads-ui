using legallead.jdbc.entities;
using Stripe.Checkout;
using System.Security.Cryptography;

namespace legallead.permissions.api.Utility
{
    public partial class SubscriptionInfrastructure
    {
        public async Task<string?> GenerateSession(HttpRequest request, User user, string level, string externalId = "")
        {
            if (!IsPermissionSessionNeeded(user, level, externalId)) return null;
            var successUrl = $"{request.Scheme}://{request.Host}/subscription-result?session_id=~1&sts=success&id=~0"
                .Replace("~1", "{CHECKOUT_SESSION_ID}");
            var cancelUrl = successUrl.Replace("sts=success", "sts=cancel");
            var priceId = "{{PRICE_ID}}";
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
                Customer = "",
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
            return session.Url;
        }

        private bool IsPermissionSessionNeeded(User user, string level, string externalId = "")
        {
            if (string.IsNullOrEmpty(externalId))
            {
                externalId = PermissionsKey();
            }
            if (string.IsNullOrEmpty(level)) { return false; }
            if (string.IsNullOrEmpty(user.Id)) { return false; }
            Console.WriteLine("{0} - {1} - {2}", user.Id, level, externalId);
            return true;
        }

        private static string PermissionsKey()
        {
            string allowed = "ABCDEFGHIJKLMONOPQRSTUVWXYZabcdefghijklmonopqrstuvwxyz0123456789";
            int strlen = 10;
            char[] randomChars = new char[strlen];

            for (int i = 0; i < strlen; i++)
            {
                randomChars[i] = allowed[RandomNumberGenerator.GetInt32(0, allowed.Length)];
            }

            return new string(randomChars);
        }
    }
}
