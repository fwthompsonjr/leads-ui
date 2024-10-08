﻿using HtmlAgilityPack;
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
        private readonly ICustomerRepository _custRepo;
        private SubscriptionService? _injectedSubscriptions;
        public PaymentHtmlTranslator(
            IUserSearchRepository db,
            IUserRepository userdb,
            ICustomerInfrastructure customer,
            ISubscriptionInfrastructure subscription,
            IStripeInfrastructure stripeService,
            StripeKeyEntity key,
            ICustomerRepository customerDb)
        {
            _repo = db;
            _custDb = customer;
            _userDb = userdb;
            _subscriptionDb = subscription;
            _paymentKey = key.GetActiveName();
            _custRepo = customerDb;
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

        public async Task<bool> IsRequestValidAsync(string? status, string? id)
        {
            if (string.IsNullOrWhiteSpace(status)) return false;
            if (string.IsNullOrWhiteSpace(id)) return false;
            if (!requestNames.Contains(status)) return false;
            var isValid = await _repo.IsValidExternalId(id);
            return isValid;
        }

        public async Task<PaymentSessionDto?> IsSessionValidAsync(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var session = await _repo.GetPaymentSession(id);
            return session;
        }

        public async Task<LevelRequestBo?> IsSubscriptionValidAsync(string? id, string? sessionid)
        {
            var bo = await _subscriptionDb.GetLevelRequestByIdAsync(id, sessionid);
            if (bo == null || string.IsNullOrEmpty(bo.InvoiceUri)) return null;
            if (bo.InvoiceUri == "NONE") return bo;
            var service = GetSubscriptionService;
            var subscription = await service.GetAsync(bo.SessionId ?? "");
            if (subscription == null) return null;
            return bo;
        }

        public async Task<LevelRequestBo?> IsDiscountValidAsync(string? id, string? sessionid)
        {
            var bo = await _subscriptionDb.GetDiscountRequestByIdAsync(id, sessionid);
            if (bo == null || string.IsNullOrEmpty(bo.InvoiceUri)) return null;
            if (bo.InvoiceUri == "NONE") return bo;
            var service = GetSubscriptionService;
            var subscription = await service.GetAsync(bo.SessionId ?? "");
            if (subscription == null) return null;
            return bo;
        }

        public async Task<bool> IsRequestPaidAsync(PaymentSessionDto? dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.JsText)) return false;
            var obj = JsonConvert.DeserializeObject<PaymentSessionJs>(FormatSessionJson(dto.JsText)) ?? new();
            if (obj.Data.Count == 0) return false;
            var dat = obj.Data[0];
            if (string.IsNullOrEmpty(dat.ReferenceId)) return false;
            var ispaid = await _repo.IsSearchPurchased(dat.ReferenceId);
            return ispaid.GetValueOrDefault();
        }

        public async Task<bool> IsRequestPaidAsync(LevelRequestBo session)
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

        public async Task<bool> IsDiscountPaidAsync(LevelRequestBo session)
        {
            var response = await IsRequestPaidAsync(session);
            return response;
        }
        public async Task<bool> IsRequestDownloadedAndPaidAsync(PaymentSessionDto? dto)
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

        public async Task<DownloadResponse> GetDownloadAsync(PaymentSessionDto dto)
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
                await GenerateExcelResponseAsync(dto, searchId, records, response);
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
            content = session.GetHtml(content, _paymentKey, _custRepo);
            return content;
        }
        public string Transform(DiscountRequestBo discountRequest, string content)
        {

            if (string.IsNullOrEmpty(discountRequest.SessionId)) return content;
            content = discountRequest.GetHtml(content, _paymentKey, _custRepo);
            return content;
        }

        public async Task<string> TransformAsync(bool isvalid, string? status, string? id, string html)
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


        public async Task<string> TransformForPermissionsAsync(bool isvalid, string? status, string? id, string html)
        {
            string paymentType = "Monthly";
            bool? isPermissionSet = default;
            var externalIndex = id ?? string.Empty;
            var bo = (await _custDb.GetLevelRequestByIdAsync(externalIndex)) ?? new() { ExternalId = externalIndex, IsPaymentSuccess = isvalid };
            var expected = ((await _custRepo.GetLevelRequestPaymentAmount(externalIndex)) ?? []).FindAll(x => (x.PriceType ?? string.Empty).Equals(paymentType));
            var user = await _userDb.GetById(bo.UserId ?? string.Empty);
            bo = await _custDb.CompleteLevelRequestAsync(bo);
            if (isvalid && bo != null && !string.IsNullOrWhiteSpace(bo.LevelName) && user != null)
            {
                isPermissionSet = (await _subscriptionDb.SetPermissionGroupAsync(user, bo.LevelName)).Key;
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var price = expected.Sum(x => x.Price);
            var paymentDate = ToDateString(bo?.CompletionDate, dash);
            var paymentAmount = ToCurrencyString(price, dash);
            UserLevelHtmlMapper.SetPageHeading(doc, isvalid);
            UserLevelHtmlMapper.SetUserDetail(doc, user);
            UserLevelHtmlMapper.SetProductDescription(doc, bo?.LevelName);
            UserLevelHtmlMapper.SetProductPaymentDate(doc, paymentDate);
            UserLevelHtmlMapper.SetProductPaymentAmount(doc, paymentAmount);
            UserLevelHtmlMapper.SetProductPaymentReferenceNumber(doc, externalIndex);
            UserLevelHtmlMapper.SetPermissionsErrorFlag(doc, isPermissionSet);

            var tranformed = doc.DocumentNode.OuterHtml;
            return tranformed;
        }

        [ExcludeFromCodeCoverage(Justification = "Coverage is to be handled later. Reference GitHub Issue")]
        public async Task<string> TransformForDiscountsAsync(ISubscriptionInfrastructure infra, bool isvalid, string? id, string html)
        {
            string paymentType = "Monthly";
            bool? isPermissionSet = default;
            if (_custDb is CustomerInfrastructure cdb)
            {
                cdb.SubscriptionInfrastructure(infra);
            }
            var externalId = id ?? string.Empty;
            var bo = (await _custDb.GetDiscountRequestByIdAsync(externalId)) ?? new() { ExternalId = id, IsPaymentSuccess = isvalid };
            var user = await _userDb.GetById(bo.UserId ?? string.Empty);
            bo.IsPaymentSuccess = isvalid;
            bo = await _custDb.CompleteDiscountRequestAsync(bo);
            if (isvalid && bo != null && !string.IsNullOrWhiteSpace(bo.LevelName) && user != null)
            {
                isPermissionSet = true;
            }
            var expected = ((await _custRepo.GetDiscountRequestPaymentAmount(externalId)) ?? []).FindAll(x => (x.PriceType ?? string.Empty).Equals(paymentType));
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var price = expected.Sum(x => x.Price);
            var paymentDate = ToDateString(bo?.CompletionDate, dash);
            var paymentAmount = ToCurrencyString(price, dash);
            var description = InvoiceExtensions.GetDiscountDescription(bo?.LevelName ?? string.Empty, true);
            UserLevelHtmlMapper.SetPageHeading(doc, isvalid);
            UserLevelHtmlMapper.SetUserDetail(doc, user);
            UserLevelHtmlMapper.SetProductPaymentDate(doc, paymentDate);
            UserLevelHtmlMapper.SetProductPaymentAmount(doc, paymentAmount);
            UserLevelHtmlMapper.SetDiscountDescription(doc, description);
            UserLevelHtmlMapper.SetProductPaymentReferenceNumber(doc, externalId);
            UserLevelHtmlMapper.SetPermissionsErrorFlag(doc, isPermissionSet);
            var tranformed = doc.DocumentNode.OuterHtml;
            return tranformed;
        }

        public async Task<object?> ResetDownloadAsync(DownloadResetRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.ExternalId))
            {
                return null;
            }
            var response = await _repo.AllowDownloadRollback(request.UserId, request.ExternalId);
            return response;
        }



        public async Task<bool> IsChangeUserLevelAsync(string? status, string? id)
        {
            var mapped = requestNames.ToList().Find(s => s.Equals(status));
            if (string.IsNullOrEmpty(mapped)) return false;
            if (string.IsNullOrEmpty(id)) return false;
            if (mapped.Equals(requestNames[1])) return false;
            var bo = await _custDb.GetLevelRequestByIdAsync(id);
            return bo != null && !string.IsNullOrEmpty(bo.Id);
        }

        public async Task<bool> IsDiscountLevelAsync(string? status, string? id)
        {
            var mapped = requestNames.ToList().Find(s => s.Equals(status));
            if (string.IsNullOrEmpty(mapped)) return false;
            if (string.IsNullOrEmpty(id)) return false;
            if (mapped.Equals(requestNames[1])) return false;
            var bo = await _custDb.GetDiscountRequestByIdAsync(id);
            return bo != null && !string.IsNullOrEmpty(bo.Id);
        }


        [ExcludeFromCodeCoverage(Justification = "Private member tested thru public method.")]
        private async Task GenerateExcelResponseAsync(PaymentSessionDto dto, string searchId, List<SearchFinalBo> records, DownloadResponse response)
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

            public static void SetDiscountDescription(HtmlDocument document, string? level)
            {
                if (string.IsNullOrEmpty(level)) return;
                var lookup = new Dictionary<string, string>()
                {
                    { "//div[@name='payment-details-payment-product']", level },
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

            public static void SetProductPaymentReferenceNumber(HtmlDocument document, string? referenceId)
            {
                if (string.IsNullOrEmpty(referenceId)) return;
                var lookup = new Dictionary<string, string>()
            {
                { "//div[@name='payment-details-reference-id']", referenceId },
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
