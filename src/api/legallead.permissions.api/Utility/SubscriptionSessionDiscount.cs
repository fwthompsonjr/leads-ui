using legallead.jdbc.entities;
using legallead.models;
using legallead.permissions.api.Model;
using legallead.permissions.api.Models;
using legallead.permissions.api.Services;
using Newtonsoft.Json;
using Stripe;

namespace legallead.permissions.api.Utility
{
    public partial class SubscriptionInfrastructure
    {
        public async Task<LevelRequestBo> GenerateDiscountSession(HttpRequest request, User user, string json, bool isAdmin, string externalId = "")
        {

            var findSession = await IsDiscountSessionNeeded(user, json, isAdmin, externalId);
            if (findSession != null && findSession.IsPaymentSuccess.GetValueOrDefault()) return findSession;
            if (_customer == null || _payment == null) return new();
            if (string.IsNullOrWhiteSpace(externalId) && !string.IsNullOrEmpty(findSession?.ExternalId))
            {
                externalId = findSession.ExternalId;
            }
            var cust = await _customer.GetOrCreateCustomer(user.Id);
            if (cust == null || string.IsNullOrEmpty(cust.CustomerId)) return new();

            var successUrl = $"{request.Scheme}://{request.Host}/discount-result?session_id=~1&sts=success&id=~0";
            LevelChangeRequest changeRq = await GetDiscountSession(user, json, externalId, cust, successUrl);
            var bo = (await _customer.AddDiscountChangeRequest(changeRq)) ?? new();
            return bo;
        }

        public async Task<LevelRequestBo?> GetDiscountRequestById(string? id, string? sessionid)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            if (_customer == null) return null;
            var bo = await _customer.GetDiscountRequestById(id);
            if (bo == null || string.IsNullOrEmpty(bo.SessionId)) return null;
            if (!string.IsNullOrEmpty(sessionid) && bo.SessionId != sessionid) return null;
            return bo;
        }


        private async Task<LevelRequestBo?> IsDiscountSessionNeeded(User user, string json, bool isAdmin, string externalId = "")
        {
            if (string.IsNullOrEmpty(json) || !CanMapDiscountJson(json)) { return null; }
            var payloadObj = MapDiscountJson(json);
            if (payloadObj == null) return null;
            payloadObj = MapPricingCodes(payloadObj);
            if (!payloadObj.Choices.Any(x => x.IsSelected)) return null;
            var payload = JsonConvert.SerializeObject(payloadObj);
            if (string.IsNullOrEmpty(externalId)) externalId = PermissionsKey();
            if (string.IsNullOrEmpty(user.Id)) { return null; }
            // if level = admin then write level request and return object
            if (isAdmin && _customer != null)
            {
                Console.WriteLine("Creating level request as completed.");
                var request = new LevelChangeRequest
                {
                    ExternalId = externalId,
                    InvoiceUri = "NONE",
                    LevelName = payload,
                    SessionId = "NONE",
                    UserId = user.Id
                };
                var session = await _customer.AddDiscountChangeRequest(request);
                return session;
            }
            return new LevelRequestBo { ExternalId = externalId };
        }

        private async Task<LevelChangeRequest> GetDiscountSession(User user, string json, string externalId, PaymentCustomerBo cust, string successUrl)
        {
            if (_customer == null || _payment == null || !CanMapDiscountJson(json)) return new();
            var payloadObj = MapDiscountJson(json);
            if (payloadObj == null) return new();
            payloadObj = MapPricingCodes(payloadObj);
            if (!payloadObj.Choices.Any(x => x.IsSelected)) return new();
            try
            {
                var payload = JsonConvert.SerializeObject(payloadObj);
                var cancelUrl = successUrl.Replace("sts=success", "sts=cancel");
                var session = await CreateDiscountSession(cust, successUrl, cancelUrl, externalId, payloadObj);
                var changeRq = new LevelChangeRequest
                {
                    ExternalId = externalId,
                    InvoiceUri = session.Url,
                    LevelName = payload,
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


        private static async Task<SubscriptionCreatedModel> CreateDiscountSession(
            PaymentCustomerBo cust,
            string successUrl,
            string cancelUrl,
            string externalId,
            DiscountChangeParent changeRequest)
        {
            var response = new SubscriptionCreatedModel();
            // Automatically save the payment method to the subscription
            // when the first payment is successful.
            var paymentSettings = new SubscriptionPaymentSettingsOptions
            {
                SaveDefaultPaymentMethod = "on_subscription",
            };
            // Create the subscription. Note we're expanding the Subscription's
            // latest invoice and that invoice's payment_intent
            // so we can pass it to the front end to confirm the payment
            var dat = new Dictionary<string, string>(){
                    { "SubscriptionType", "discount" },
                    { "SuccessUrl", successUrl },
                    { "CancelUrl", cancelUrl },
                    { "ExternalId", externalId },
                };
            changeRequest = MapPricingCodes(changeRequest);
            var selections = changeRequest.Choices.Where(x => x.IsSelected);

            var items = selections.Select(x => new SubscriptionItemOptions
            {
                Price = x.MonthlyBillingCode,
                Quantity = 1
            }).ToList();

            var options = new SubscriptionCreateOptions
            {
                Customer = cust.CustomerId,
                Items = items,
                InvoiceSettings = new() { Issuer = new() { Type = "self" } },
                PaymentSettings = paymentSettings,
                PaymentBehavior = "default_incomplete",
                Metadata = dat
            };
            options.AddExpand("latest_invoice.payment_intent");
            var service = new SubscriptionService();
            try
            {
                var returnUri = GetDiscountPaymentUrl(successUrl);
                returnUri = returnUri.Replace("~0", externalId);
                var session = await service.CreateAsync(options);
                returnUri = returnUri.Replace("~1", session.Id);
                dat["SuccessUrl"] = successUrl.Replace("~0", externalId).Replace("~1", session.Id);
                dat["CancelUrl"] = cancelUrl.Replace("~0", externalId).Replace("~1", session.Id);
                dat.Add("InitializeUrl", returnUri);
                service.Update(session.Id, new() { Metadata = dat });
                response.Id = session.Id;
                response.Url = returnUri;
                return response;
            }
            catch (StripeException e)
            {
                Console.WriteLine($"Failed to create subscription.{e}");
                return response;
            }
        }

        private static string GetDiscountPaymentUrl(string landing)
        {
            if (!Uri.TryCreate(landing, UriKind.Absolute, out var url)) return landing;
            var host = (url.Scheme) switch
            {
                "https" => url.Port == 443 ? url.Host : string.Concat(url.Host, ":", url.Port.ToString()),
                "http" => url.Port == 80 ? url.Host : string.Concat(url.Host, ":", url.Port.ToString()),
                _ => url.Host,
            };
            var constructedUrl = $"{url.Scheme}://{host}/discount-checkout?sessionid=~1&id=~0";
            return constructedUrl;
        }

        private static bool CanMapDiscountJson(string json)
        {
            try
            {
                var test = JsonConvert.DeserializeObject<ChangeDiscountRequest>(json);
                return test != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static DiscountChangeParent? MapDiscountJson(string json)
        {
            var test = JsonConvert.DeserializeObject<ChangeDiscountRequest>(json);
            if (test == null) return null;
            return ModelMapper.Mapper.Map<DiscountChangeParent>(test);
        }
        /// <summary>
        /// Looks up pricing from price list and maps codes into discount object
        /// </summary>
        /// <param name="discounts"></param>
        /// <remarks>
        /// This method should actually return a value to
        /// </remarks>
        private static DiscountChangeParent MapPricingCodes(DiscountChangeParent discounts)
        {
            const string level = ".Discount.";
            if (!discounts.Choices.Any(a => a.IsSelected)) { return discounts; }
            var pricing = PricingLookupService.PricingCodes.FindAll(x =>
                (x.KeyName ?? "").Contains(level, StringComparison.OrdinalIgnoreCase) &&
                x.IsActive.GetValueOrDefault());
            if (pricing == null || !pricing.Any()) { return discounts; }
            var choices = discounts.Choices.Where(w => w.IsSelected).ToList();
            choices.ForEach(c =>
            {
                var search = string.IsNullOrEmpty(c.CountyName) ?
                $"State.Discount.Pricing.{c.StateName}" :
                $"County.Discount.Pricing.{c.CountyName}.{c.StateName}";
                var price = pricing.Find(x => (x.KeyName ?? "").Equals(search, StringComparison.OrdinalIgnoreCase));
                if (price != null)
                {
                    c.AnnualBillingCode = price.PriceCodeAnnual;
                    c.MonthlyBillingCode = price.PriceCodeMonthly;
                }
            });
            return discounts;
        }
    }
}
