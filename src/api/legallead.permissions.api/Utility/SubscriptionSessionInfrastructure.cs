using legallead.jdbc.entities;
using legallead.permissions.api.Models;
using Stripe.Checkout;
using System.Security.Cryptography;

namespace legallead.permissions.api.Utility
{
    public partial class SubscriptionInfrastructure
    {
        public async Task<string?> GeneratePermissionSession(HttpRequest request, User user, string level, string externalId = "")
        {
            if (!IsPermissionSessionNeeded(user, level, externalId)) return null;
            if (_customer == null || _payment == null) return null;
            var cust = await _customer.GetOrCreateCustomer(user.Id);
            if (cust == null || string.IsNullOrEmpty(cust.CustomerId)) return string.Empty;
            var successUrl = $"{request.Scheme}://{request.Host}/subscription-result?session_id=~1&sts=success&id=~0"
                .Replace("~1", "{CHECKOUT_SESSION_ID}");
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
            return session.Url;
        }

        private static string GetPermissionCode(string level, PaymentStripeOption payment)
        {
            var lower = level.ToLower().Trim();
            var code = lower switch {
                "admin" => payment.Codes.Admin,
                "platinum" => payment.Codes.Platinum,
                "gold" => payment.Codes.Gold,
                "silver" => payment.Codes.Silver,
                "guest" => payment.Codes.Guest,
                _ => string.Empty
            };
            return code;
        }

        private bool IsPermissionSessionNeeded(User user, string level, string externalId = "")
        {
            string[] nonbillingcodes = new [] { "admin", "guest" };
            if (string.IsNullOrEmpty(level)) { return false; }
            var code = _payment == null ? string.Empty : GetPermissionCode(level, _payment);
            if (string.IsNullOrEmpty(code)) return false;
            if (string.IsNullOrEmpty(externalId)) externalId = PermissionsKey();
            if (string.IsNullOrEmpty(user.Id)) { return false; }
            // is level = guest or admin then write level request and return false
            if (nonbillingcodes.Contains(level, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine("Creating level request as completed.");
                return false;
            }
            Console.WriteLine("{0} - {1} - {2}", user.Id, level, externalId);
            return true;
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
