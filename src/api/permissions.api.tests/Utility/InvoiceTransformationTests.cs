using HtmlAgilityPack;
using legallead.jdbc.entities;
using legallead.permissions.api.Extensions;
using legallead.permissions.api.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace permissions.api.tests.Utility
{
    using HtmlTemplates = legallead.permissions.api.Properties.Resources;
    public class InvoiceTransformationTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SutCanTransformPaymentSessionDto(bool hasNullRequest)
        {
            var error = Record.Exception(() =>
            {
                var converted = ConvertPaymentInvoice(hasNullRequest, InvoiceHtmlBase, out var _);
                if (hasNullRequest) Assert.Equal(InvoiceHtmlBase, converted);
                else Assert.NotEqual(InvoiceHtmlBase, converted);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true, "<!-- stripe public key -->")]
        [InlineData(true, "<!-- payment external id -->")]
        [InlineData(true, "<!-- payment completed url -->")]
        [InlineData(false, "<!-- stripe public key -->")]
        [InlineData(false, "<!-- payment external id -->")]
        [InlineData(false, "<!-- payment completed url -->")]
        public void SutCanTransformPaymentSessionDtoTokenReplacements(bool hasNullRequest, string searchString)
        {
            var error = Record.Exception(() =>
            {
                var converted = ConvertPaymentInvoice(hasNullRequest, InvoiceHtmlBase, out var _);
                Assert.DoesNotContain(searchString, converted);
                if (hasNullRequest) Assert.Equal(InvoiceHtmlBase, converted);
                else Assert.NotEqual(InvoiceHtmlBase, converted);
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData("//script[@name='checkout-stripe-js']")]
        [InlineData("//ul[@name='invoice-line-items']")]
        [InlineData("//span[@name='invoice']")]
        [InlineData("//span[@name='invoice-date']")]
        [InlineData("//span[@name='invoice-description']")]
        [InlineData("//span[@name='invoice-total']")]
        public void SutCanTransformPaymentSessionDtoHtmlElements(string searchString)
        {
            const string dash = " - ";
            var error = Record.Exception(() =>
            {
                var converted = ConvertPaymentInvoice(false, InvoiceHtmlBase, out var _);
                var doc = new HtmlDocument();
                doc.LoadHtml(converted);
                var parentNode = doc.DocumentNode;
                var expected = parentNode.SelectSingleNode(searchString);
                Assert.NotNull(expected);
                var content = expected.InnerHtml.Trim();
                Assert.NotEqual(dash, content);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SutCanTransformSubscriptionRequest(bool hasNullRequest)
        {
            var error = Record.Exception(() =>
            {
                var converted = ConvertSubscriptionInvoice(hasNullRequest, out var _);
                Assert.False(string.IsNullOrEmpty(converted));
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SutCanTransformDiscountRequest(bool hasNullRequest)
        {
            var error = Record.Exception(() =>
            {
                var converted = ConvertDiscountInvoice(hasNullRequest, out var _);
                Assert.False(string.IsNullOrEmpty(converted));
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(true, "cancel")]
        [InlineData(true, "success", null)]
        public async Task SutCanTransformInvoiceRequest(
            bool isvalid,
            string? status = "success",
            string? id = "123-456-789")
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var converted = await ConvertInvoice(isvalid, status, id);
                Assert.False(string.IsNullOrEmpty(converted));
            });
            Assert.Null(error);
        }


        private static string ConvertPaymentInvoice(bool hasNullRequest, string html, out PaymentSessionDto? request)
        {
            using var builder = new PaymentHtmlTranslatorBuilder();
            var service = builder.Translator;
            request = hasNullRequest ? null : PaymentHtmlHelper.SessionFaker.Generate();
            return service.Transform(request, html);
        }

        private static string ConvertSubscriptionInvoice(bool hasNullRequest, out LevelRequestBo request)
        {
            using var builder = new PaymentHtmlTranslatorBuilder();
            var service = builder.Translator;
            request = PaymentHtmlHelper.LevelFaker.Generate();
            if (hasNullRequest) { request.SessionId = string.Empty; }
            return service.Transform(request, SubscriptionBase);
        }

        private static string ConvertDiscountInvoice(bool hasNullRequest, out DiscountRequestBo request)
        {
            using var builder = new PaymentHtmlTranslatorBuilder();
            var service = builder.Translator;
            request = PaymentHtmlHelper.DiscountBoFaker.Generate();
            if (hasNullRequest) { request.SessionId = string.Empty; }
            return service.Transform(request, DiscountBase);
        }

        private async static Task<string> ConvertInvoice(bool isvalid, string? status, string? id)
        {
            using var builder = new PaymentHtmlTranslatorBuilder();
            var service = builder.Translator;
            var repo = builder.MockRepo;
            var purchase = string.IsNullOrEmpty(id) ? null : PaymentHtmlHelper.PurchaseSummaryFaker.Generate();
            var html = isvalid ? InvoiceCompletedBase : InvoiceInvalidBase;
            repo.Setup(m => m.GetPurchaseSummary(It.IsAny<string>())).ReturnsAsync(purchase);
            repo.Setup(m => m.SetInvoicePurchaseDate(It.IsAny<string>())).ReturnsAsync(true);
            var response = await service.Transform(isvalid, status, id, html);
            return response;
        }

        private static readonly string InvoiceHtmlBase = HtmlTemplates.page_invoice_html;
        private static readonly string SubscriptionBase = HtmlTemplates.page_invoice_subscription_html;
        private static readonly string DiscountBase = HtmlTemplates.page_invoice_discount_html;
        private static readonly string InvoiceCompletedBase = HtmlTemplates.page_payment_completed;
        private static readonly string InvoiceInvalidBase = HtmlTemplates.page_payment_detail_invalid;
    }
}
