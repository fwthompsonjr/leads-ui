using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Stripe;
using System.Security.Cryptography;

namespace legallead.permissions.api.Utility
{
    [ExcludeFromCodeCoverage(Justification = "Coverage is to be handled later. Reference GitHub Issue")]
    public partial class SubscriptionInfrastructure
    {
        public async Task<LevelRequestBo> GeneratePermissionSessionAsync(HttpRequest request, User user, string level, string externalId = "")
        {
            var findSession = await IsPermissionSessionNeededAsync(user, level, externalId);
            if (findSession != null && findSession.IsPaymentSuccess.GetValueOrDefault()) return findSession;
            if (_customer == null || _payment == null) return new();
            if (string.IsNullOrEmpty(externalId) && !string.IsNullOrEmpty(findSession?.ExternalId))
            {
                externalId = findSession.ExternalId;
            }
            var cust = await _customer.GetOrCreateCustomerAsync(user.Id);
            if (cust == null || string.IsNullOrEmpty(cust.CustomerId)) return new();

            var successUrl = $"{request.Scheme}://{request.Host}/subscription-result?session_id=~1&sts=success&id=~0";
            LevelChangeRequest changeRq = await GetPaymentSessionAsync(user, level, externalId, cust, successUrl);
            var bo = (await _customer.AddLevelChangeRequestAsync(changeRq)) ?? new();
            return bo;
        }

        public async Task<LevelRequestBo?> GetLevelRequestByIdAsync(string? id, string? sessionid)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            if (_customer == null) return null;
            var bo = await _customer.GetLevelRequestByIdAsync(id);
            if (bo == null || string.IsNullOrEmpty(bo.SessionId)) return null;
            if (!string.IsNullOrEmpty(sessionid) && bo.SessionId != sessionid) return null;
            return bo;
        }


        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party. Item to be refactored.")]
        private async Task<LevelChangeRequest> GetPaymentSessionAsync(User user, string level, string externalId, PaymentCustomerBo cust, string successUrl)
        {
            if (_customer == null || _payment == null) return new();
            try
            {
                var cancelUrl = successUrl.Replace("sts=success", "sts=cancel");
                var priceId = GetPermissionCode(level, _payment);
                var session = await CreatePaymentSessionAsync(cust, successUrl, cancelUrl, externalId, priceId);
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

        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party. Item to be refactored.")]
        private static async Task<SubscriptionCreatedModel> CreatePaymentSessionAsync(
            PaymentCustomerBo cust,
            string successUrl,
            string cancelUrl,
            string externalId,
            string priceId)
        {
            var response = new SubscriptionCreatedModel();
            return response;
        }
        [ExcludeFromCodeCoverage(Justification = "Item to be refactored.")]
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

        [ExcludeFromCodeCoverage(Justification = "Interacts with 3rd party. Item to be refactored.")]
        private async Task<LevelRequestBo?> IsPermissionSessionNeededAsync(User user, string level, string externalId = "")
        {
            string[] nonbillingcodes = ["admin", "guest"];
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
                var session = await _customer.AddLevelChangeRequestAsync(request);
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
