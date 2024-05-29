using HtmlAgilityPack;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using Newtonsoft.Json;
using Stripe;
using System.Globalization;

namespace legallead.permissions.api.Utility
{
    public class PaymentHtmlTranslator : IPaymentHtmlTranslator
    {
        private const string dash = " - ";
        private readonly IUserSearchRepository _repo;
        private readonly ISubscriptionInfrastructure _subscriptionDb;
        private readonly ICustomerInfrastructure _custDb;
        private readonly IUserRepository _userDb;
        private readonly string _paymentKey;
        private SubscriptionService? _injectedSubscriptions;
        public PaymentHtmlTranslator(
            IUserSearchRepository db,
            IUserRepository userdb,
            ICustomerInfrastructure customer,
            ISubscriptionInfrastructure subscription,
            IStripeInfrastructure stripeService,
            StripeKeyEntity key)
        {
            _repo = db;
            _custDb = customer;
            _userDb = userdb;
            _subscriptionDb = subscription;
            _paymentKey = key.GetActiveName();
            InvoiceExtensions.GetInfrastructure ??= stripeService;
        }
        public void SetupSubscriptionService(SubscriptionService? service)
        {
            _injectedSubscriptions = service;
        }
        public SubscriptionService GetSubscriptionService
        {
            get
            {
                if (_injectedSubscriptions == null) return new SubscriptionService();
                return _injectedSubscriptions;
            }
        }

        public async Task<bool> IsRequestValid(string? status, string? id)
        {
            if (string.IsNullOrWhiteSpace(status)) return false;
            if (string.IsNullOrWhiteSpace(id)) return false;
            if (!requestNames.Contains(status)) return false;
            var isValid = await _repo.IsValidExternalId(id);
            return isValid;
        }

        public async Task<PaymentSessionDto?> IsSessionValid(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var session = await _repo.GetPaymentSession(id);
            return session;
        }

        public async Task<LevelRequestBo?> IsSubscriptionValid(string? id, string? sessionid)
        {
            var bo = await _subscriptionDb.GetLevelRequestById(id, sessionid);
            if (bo == null || string.IsNullOrEmpty(bo.InvoiceUri)) return null;
            if (bo.InvoiceUri == "NONE") return bo;
            var service = GetSubscriptionService;
            var subscription = await service.GetAsync(bo.SessionId ?? "");
            if (subscription == null) return null;
            return bo;
        }

        public async Task<LevelRequestBo?> IsDiscountValid(string? id, string? sessionid)
        {
            var bo = await _subscriptionDb.GetDiscountRequestById(id, sessionid);
            if (bo == null || string.IsNullOrEmpty(bo.InvoiceUri)) return null;
            if (bo.InvoiceUri == "NONE") return bo;
            var service = GetSubscriptionService;
            var subscription = await service.GetAsync(bo.SessionId ?? "");
            if (subscription == null) return null;
            return bo;
        }

        public async Task<bool> IsRequestPaid(PaymentSessionDto? dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.JsText)) return false;
            var obj = JsonConvert.DeserializeObject<PaymentSessionJs>(FormatSessionJson(dto.JsText)) ?? new();
            if (obj.Data.Count == 0) return false;
            var dat = obj.Data[0];
            if (string.IsNullOrEmpty(dat.ReferenceId)) return false;
            var ispaid = await _repo.IsSearchPurchased(dat.ReferenceId);
            return ispaid.GetValueOrDefault();
        }

        public async Task<bool> IsRequestPaid(LevelRequestBo session)
        {
            var isSuccess = session.IsPaymentSuccess.GetValueOrDefault();
            if (session.InvoiceUri == "NONE") return isSuccess;
            var service = GetSubscriptionService;
            var subscription = await service.GetAsync(session.SessionId ?? "");
            if (subscription == null) return false;
            // check subscription status to see if invoice has been paid
            var unpaid = new[] { "incomplete", "incomplete_expired", "canceled" };
            var status = !unpaid.Contains(subscription.Status);
            isSuccess &= status;
            return isSuccess;
        }

        public async Task<bool> IsDiscountPaid(LevelRequestBo session)
        {
            var response = await IsRequestPaid(session);
            return response;
        }
        public async Task<bool> IsRequestDownloadedAndPaid(PaymentSessionDto? dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.JsText)) return false;
            var obj = JsonConvert.DeserializeObject<PaymentSessionJs>(FormatSessionJson(dto.JsText)) ?? new();
            if (obj.Data.Count == 0) return false;
            var dat = obj.Data[0];
            if (string.IsNullOrEmpty(dat.ReferenceId)) return false;
            var paid = await _repo.IsSearchPaidAndDownloaded(dat.ReferenceId);
            if (paid == null) return false;
            return paid.IsPaid.GetValueOrDefault() && paid.IsDownloaded.GetValueOrDefault();
        }

        public async Task<DownloadResponse> GetDownload(PaymentSessionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.JsText)) return new() { Error = "Invalid session parameter." };
            var js = FormatSessionJson(dto.JsText);
            dto.JsText = js;
            var obj = JsonConvert.DeserializeObject<PaymentSessionJs>(js) ?? new();
            if (obj.Data.Count == 0) return new() { Error = "No search records found for associated request." };
            var search = obj.Data[0];
            var searchId = search.ReferenceId ?? Guid.NewGuid().ToString();
            var records = (await _repo.GetFinal(searchId)).ToList();
            if (records.Count == 0) return new() { Error = "Invalid session parameter." };
            if (records.Count > search.ItemCount) records = records.Take(records.Count).ToList();
            var response = new DownloadResponse()
            {
                ExternalId = dto.ExternalId,
                Description = obj.Description,
            };
            try
            {
                await GenerateExcelResponse(dto, searchId, records, response);
                return response;
            }
            catch (Exception ex)
            {
                response.Error = ex.Message;
                return response;
            }
        }
        public string Transform(PaymentSessionDto? session, string html)
        {
            var converted = session.GetHtml(html, _paymentKey);
            return converted;
        }
        public string Transform(LevelRequestBo session, string content)
        {
            if (string.IsNullOrEmpty(session.SessionId)) return content;
            content = session.GetHtml(content, _paymentKey);
            return content;
        }
        public string Transform(DiscountRequestBo discountRequest, string content)
        {

            if (string.IsNullOrEmpty(discountRequest.SessionId)) return content;
            content = discountRequest.GetHtml(content, _paymentKey);
            return content;
        }

        public async Task<string> Transform(bool isvalid, string? status, string? id, string html)
        {

            if (!isvalid || status == null || id == null) return html;
            var issuccess = status == requestNames[0];
            if (issuccess) await _repo.SetInvoicePurchaseDate(id);
            var summary = await _repo.GetPurchaseSummary(id) ?? new();

            var title = issuccess ? "Payment Received - Thank You" : "Payment Request Failed";
            var replacements = new Dictionary<string, string>()
            {
                { "//h5", title },
                { "//span[@name='account-user-email']", summary.Email ?? dash },
                { "//span[@name='account-user-name']", summary.UserName ?? dash },
                { "//div[@name='payment-details-payment-date']", ToDateString(summary.PurchaseDate, dash) },
                { "//div[@name='payment-details-payment-product']", summary.ItemType ?? dash },
                { "//div[@name='payment-details-payment-amount']", ToCurrencyString(summary.Price, dash) },
                { "//div[@name='payment-details-reference-id']", summary.ExternalId ?? dash },
            };
            var keynames = replacements.Keys.ToList();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            for (var i = 0; i < replacements.Count; i++)
            {
                var key = keynames[i];
                var value = replacements[key];
                var node = doc.DocumentNode.SelectSingleNode(key);
                if (node == null) continue;
                if (!issuccess && i == 0)
                {
                    var attributes = node.Attributes["class"].Value.Split(' ').ToList();
                    attributes.Remove("text-success");
                    attributes.Add("text-danger");
                    node.Attributes["class"].Value = string.Join(" ", attributes);
                }
                node.InnerHtml = value;
            }
            if (!issuccess) return doc.DocumentNode.OuterHtml;
            var callout = "//div[@name='payment-details-reference-callout']";
            var item = doc.DocumentNode.SelectSingleNode(callout);
            item.Attributes.Remove("style");
            return doc.DocumentNode.OuterHtml;
        }



        [ExcludeFromCodeCoverage(Justification = "Coverage is to be handled later. Reference GitHub Issue")]
        public async Task<string> TransformForPermissions(bool isvalid, string? status, string? id, string html)
        {
            bool? isPermissionSet = default;
            var externalIndex = id ?? string.Empty;
            var bo = (await _custDb.GetLevelRequestById(externalIndex)) ?? new() { ExternalId = externalIndex, IsPaymentSuccess = isvalid };
            var user = await _userDb.GetById(bo.UserId ?? string.Empty);
            bo = await _custDb.CompleteLevelRequest(bo);
            var summary = await _repo.GetPurchaseSummary(externalIndex) ?? new();
            if (isvalid && bo != null && !string.IsNullOrWhiteSpace(bo.LevelName) && user != null)
            {
                isPermissionSet = (await _subscriptionDb.SetPermissionGroup(user, bo.LevelName)).Key;
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var paymentDate = ToDateString(bo?.CompletionDate, dash);
            var paymentAmount = ToCurrencyString(summary.Price, dash);
            UserLevelHtmlMapper.SetPageHeading(doc, isvalid);
            UserLevelHtmlMapper.SetUserDetail(doc, user);
            UserLevelHtmlMapper.SetProductDescription(doc, bo?.LevelName);
            UserLevelHtmlMapper.SetProductPaymentDate(doc, paymentDate);
            UserLevelHtmlMapper.SetProductPaymentAmount(doc, paymentAmount);
            UserLevelHtmlMapper.SetPermissionsErrorFlag(doc, isPermissionSet);

            var tranformed = doc.DocumentNode.OuterHtml;
            return tranformed;
        }

        [ExcludeFromCodeCoverage(Justification = "Coverage is to be handled later. Reference GitHub Issue")]
        public async Task<string> TransformForDiscounts(ISubscriptionInfrastructure infra, bool isvalid, string? id, string html)
        {
            bool? isPermissionSet = default;
            if (_custDb is CustomerInfrastructure cdb)
            {
                cdb.SubscriptionInfrastructure(infra);
            }
            var bo = (await _custDb.GetDiscountRequestById(id ?? string.Empty)) ?? new() { ExternalId = id, IsPaymentSuccess = isvalid };
            var user = await _userDb.GetById(bo.UserId ?? string.Empty);
            bo.IsPaymentSuccess = isvalid;
            bo = await _custDb.CompleteDiscountRequest(bo);
            if (isvalid && bo != null && !string.IsNullOrWhiteSpace(bo.LevelName) && user != null)
            {
                isPermissionSet = true;
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            UserLevelHtmlMapper.SetPageHeading(doc, isvalid);
            UserLevelHtmlMapper.SetUserDetail(doc, user);
            UserLevelHtmlMapper.SetProductDescription(doc, bo?.LevelName);
            UserLevelHtmlMapper.SetPermissionsErrorFlag(doc, isPermissionSet);
            var tranformed = doc.DocumentNode.OuterHtml;
            return tranformed;
        }

        public async Task<object?> ResetDownload(DownloadResetRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.ExternalId))
            {
                return null;
            }
            var response = await _repo.AllowDownloadRollback(request.UserId, request.ExternalId);
            return response;
        }



        public async Task<bool> IsChangeUserLevel(string? status, string? id)
        {
            var mapped = requestNames.ToList().Find(s => s.Equals(status));
            if (string.IsNullOrEmpty(mapped)) return false;
            if (string.IsNullOrEmpty(id)) return false;
            if (mapped.Equals(requestNames[1])) return false;
            var bo = await _custDb.GetLevelRequestById(id);
            return bo != null && !string.IsNullOrEmpty(bo.Id);
        }

        public async Task<bool> IsDiscountLevel(string? status, string? id)
        {
            var mapped = requestNames.ToList().Find(s => s.Equals(status));
            if (string.IsNullOrEmpty(mapped)) return false;
            if (string.IsNullOrEmpty(id)) return false;
            if (mapped.Equals(requestNames[1])) return false;
            var bo = await _custDb.GetDiscountRequestById(id);
            return bo != null && !string.IsNullOrEmpty(bo.Id);
        }


        [ExcludeFromCodeCoverage(Justification = "Private member tested thru public method.")]
        private async Task GenerateExcelResponse(PaymentSessionDto dto, string searchId, List<SearchFinalBo> records, DownloadResponse response)
        {
            response.Content = ExcelExtensions.WriteExcel(dto, records);
            if (response.Content != null)
            {
                var conversion = Convert.ToBase64String(response.Content);
                _ = await _repo.CreateOrUpdateDownloadRecord(searchId, conversion);
                response.CreateDate = DateTime.UtcNow.ToString("s");
            }
            else
            {
                response.Error = "Unable to generate excel ouput";
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested thru public method.")]
        private static string ToDateString(DateTime? date, string fallback)
        {
            if (!date.HasValue) return fallback;
            return date.Value.ToString("MMM d, yyyy, h:mm tt");
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested thru public method.")]
        private static string ToCurrencyString(decimal? amount, string fallback)
        {
            if (!amount.HasValue) return fallback;
            return amount.Value.ToString("C", CultureInfo.CurrentCulture);
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested thru public method.")]
        private static string FormatSessionJson(string? original)
        {
            const string slash = @"\";
            const string openBraceQt = "\"[";
            const string openBrace = "[";
            const string closeBraceQt = "]\"";
            const string closeBrace = "]";
            if (string.IsNullOrEmpty(original)) return string.Empty;
            if (original.Contains(openBraceQt)) original = original.Replace(openBraceQt, openBrace);
            if (original.Contains(closeBraceQt)) original = original.Replace(closeBraceQt, closeBrace);
            return original.Replace(slash.ToString(), string.Empty);
        }


        private static readonly string[] requestNames = ["success", "cancel"];

        [ExcludeFromCodeCoverage(Justification = "Private member tested thru public method.")]
        private static class UserLevelHtmlMapper
        {
            public static void SetPageHeading(HtmlDocument document, bool isvalid)
            {
                const string nodeSelector = "//*[@id='heading-level-completion-payment-status']";
                var message = isvalid ? "Payment Received  - Thank You" : "Payment Failed - Please retry";
                var node = document.DocumentNode.SelectSingleNode(nodeSelector);
                if (node == null) return;
                node.InnerHtml = message;
                if (isvalid) return;

                node.RemoveClass("text-success");
                node.AddClass("text-danger");
            }

            public static void SetUserDetail(HtmlDocument document, User? user)
            {
                if (user == null) return;
                var lookup = new Dictionary<string, string>()
                {
                    { "//span[@name='account-user-name']", user.UserName ?? " - " },
                    { "//span[@name='account-user-email']", user.Email ?? " - " },
                };
                var keys = lookup.Keys.ToList();
                keys.ForEach(key =>
                {
                    var message = lookup[key];
                    var node = document.DocumentNode.SelectSingleNode(key);
                    if (node != null) { node.InnerHtml = message; }
                });
            }

            public static void SetProductDescription(HtmlDocument document, string? level)
            {
                if (string.IsNullOrEmpty(level)) return;
                if (!Descriptions.TryGetValue(level, out var message)) return;
                message = englishText.ToTitleCase(level.ToLower());
                var lookup = new Dictionary<string, string>()
                {
                    { "//div[@name='payment-details-payment-product']", $" Subscription {message}" },
                    { "//*[@id='payment-details-description']", $" Subscription {message}" },
                };
                var keys = lookup.Keys.ToList();
                keys.ForEach(key =>
                {
                    var txt = lookup[key];
                    var node = document.DocumentNode.SelectSingleNode(key);
                    if (node != null) { node.InnerHtml = txt; }
                });
            }

            public static void SetProductPaymentDate(HtmlDocument document, string? paymentDate)
            {
                if (string.IsNullOrEmpty(paymentDate)) return;
                var lookup = new Dictionary<string, string>()
            {
                { "//div[@name='payment-details-payment-date']", paymentDate },
            };
                var keys = lookup.Keys.ToList();
                keys.ForEach(key =>
                {
                    var txt = lookup[key];
                    var node = document.DocumentNode.SelectSingleNode(key);
                    if (node != null) { node.InnerHtml = txt; }
                });
            }


            public static void SetProductPaymentAmount(HtmlDocument document, string? paymentAmount)
            {
                if (string.IsNullOrEmpty(paymentAmount)) return;
                var lookup = new Dictionary<string, string>()
            {
                { "//div[@name='payment-details-payment-amount']", paymentAmount },
            };
                var keys = lookup.Keys.ToList();
                keys.ForEach(key =>
                {
                    var txt = lookup[key];
                    var node = document.DocumentNode.SelectSingleNode(key);
                    if (node != null) { node.InnerHtml = txt; }
                });
            }

            public static void SetPermissionsErrorFlag(HtmlDocument document, bool? isSuccess)
            {
                const string nodeSelector = "//*[@id='heading-level-completion-provisioning-error']";
                if (isSuccess == null || isSuccess.Value) return;
                var node = document.DocumentNode.SelectSingleNode(nodeSelector);
                node?.RemoveClass("d-none");
            }

            private static readonly Dictionary<string, string> Descriptions = new()
            {
                { "admin", Properties.Resources.description_role_admin },
                { "gold", Properties.Resources.description_role_gold },
                { "guest", Properties.Resources.description_role_guest },
                { "platinum", Properties.Resources.description_role_platinum },
                { "silver", Properties.Resources.description_role_silver }
            };

            private static readonly TextInfo englishText = new CultureInfo("en-US", false).TextInfo;
        }

    }
}
