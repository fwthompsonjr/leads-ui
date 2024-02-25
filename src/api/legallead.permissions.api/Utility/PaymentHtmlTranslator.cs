using HtmlAgilityPack;
using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace legallead.permissions.api.Utility
{
    public class PaymentHtmlTranslator : IPaymentHtmlTranslator
    {
        private readonly IUserSearchRepository _repo;
        private readonly string _paymentKey;
        public PaymentHtmlTranslator(IUserSearchRepository db, StripeKeyEntity key)
        { 
            _repo = db;
            _paymentKey = key.GetActiveName();
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

        public async Task<bool> IsRequestPaid(PaymentSessionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.JsText)) return false;
            var obj = JsonConvert.DeserializeObject<PaymentSessionJs>(dto.JsText) ?? new();
            if (obj.Data == null || !obj.Data.Any()) return false;
            var dat = obj.Data[0];
            if (dat == null || string.IsNullOrEmpty(dat.ReferenceId)) return false;
            var ispaid = await _repo.IsSearchPurchased(dat.ReferenceId);
            return ispaid.GetValueOrDefault();
        }
        public string Transform(PaymentSessionDto? session, string html)
        {
            var converted = session.GetHtml(html, _paymentKey);
            return converted;
        }
        public async Task<string> Transform(bool isvalid, string? status, string? id, string html)
        {
            const string dash = " - ";
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
            for ( var i = 0; i < replacements.Count; i++ )
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

        private static string ToDateString(DateTime? date, string fallback)
        {
            if (!date.HasValue) return fallback;
            return date.Value.ToString("MMM d, yyyy, h:mm tt");
        }
        private static string ToCurrencyString(decimal? amount, string fallback)
        {
            if (!amount.HasValue) return fallback;
            return amount.Value.ToString("C", CultureInfo.CurrentCulture);
        }

        private static readonly string[] requestNames = new[] { "success", "cancel" };
    }
}
