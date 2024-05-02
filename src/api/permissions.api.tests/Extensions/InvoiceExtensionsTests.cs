using legallead.jdbc.entities;
using legallead.permissions.api.Models;

namespace permissions.api.tests
{
    using legallead.permissions.api.Extensions;
    using Newtonsoft.Json;
    using HtmlTemplates = legallead.permissions.api.Properties.Resources;

    public class InvoiceExtensionsTests
    {
        private static readonly string InvoiceContent = HtmlTemplates.page_invoice_html;
        private static readonly string SubscriptionContent = HtmlTemplates.page_invoice_subscription_html;
        private static readonly string DiscountContent = HtmlTemplates.page_invoice_discount_html;

        private static readonly Faker<LevelRequestBo> levelBofaker =
            new Faker<LevelRequestBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.InvoiceUri, y => y.Hacker.Phrase())
            .RuleFor(x => x.LevelName, y => y.Hacker.Phrase())
            .RuleFor(x => x.SessionId, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());

        private static readonly Faker<SearchInvoiceBo> invoiceFaker
            = new Faker<SearchInvoiceBo>()
            .RuleFor(x => x.LineId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemType, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ItemCount, y => y.Random.Int(1, 5))
            .RuleFor(x => x.UnitPrice, y => y.Random.Decimal(1, 10))
            .RuleFor(x => x.ReferenceId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.PurchaseDate, y => y.Date.Recent())
            .FinishWith((a, b) =>
            {
                b.Price = b.UnitPrice.GetValueOrDefault() * b.ItemCount.GetValueOrDefault();
            });


        private static readonly Faker<PaymentSessionJs> paymentfaker = new Faker<PaymentSessionJs>()
            .RuleFor(x => x.Description, y => y.Person.UserName)
            .RuleFor(x => x.ExternalId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.SuccessUrl, y => y.Random.Guid().ToString())
            .FinishWith((a, b) =>
            {
                var invoice = invoiceFaker.Generate(a.Random.Int(2, 6));
                b.Data = invoice;
            });

        private static readonly Faker<PaymentSessionDto> sessionfaker = new Faker<PaymentSessionDto>()
            .RuleFor(x => x.JsText, y => y.Person.UserName)
            .RuleFor(x => x.CreateDate, y => y.Date.Recent())
            .RuleFor(x => x.ClientId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .FinishWith((a, b) =>
            {
                var data = paymentfaker.Generate();
                b.JsText = JsonConvert.SerializeObject(data);
            });

        private static readonly Faker<DiscountRequestBo> discountBofaker =
            new Faker<DiscountRequestBo>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString("D"))
            .RuleFor(x => x.ExternalId, y => y.Hacker.Phrase())
            .RuleFor(x => x.InvoiceUri, y => y.Hacker.Phrase())
            .RuleFor(x => x.LevelName, y => y.Hacker.Phrase())
            .RuleFor(x => x.SessionId, y => y.Hacker.Phrase())
            .RuleFor(x => x.IsPaymentSuccess, y => y.Random.Bool())
            .RuleFor(x => x.CompletionDate, y => y.Date.Recent())
            .RuleFor(x => x.CreateDate, y => y.Date.Recent());
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InvoiceCanMap(bool hasData)
        {
            var session = sessionfaker.Generate();
            if (!hasData)
            {
                var js = session.JsText;
                var dto = string.IsNullOrWhiteSpace(js) ?
                new() :
                JsonConvert.DeserializeObject<PaymentSessionJs>(js) ?? new();
                dto.Data.ForEach(d => d.Price = 0);
                session.JsText = JsonConvert.SerializeObject(dto);
            }
            var html = session.GetHtml(InvoiceContent, "123-456-789");
            Assert.False(string.IsNullOrEmpty(html));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InvoiceCalculatePaymentAmount(bool hasData)
        {
            var session = sessionfaker.Generate();
            if (!hasData)
            {
                var js = session.JsText;
                var dto = string.IsNullOrWhiteSpace(js) ?
                new() :
                JsonConvert.DeserializeObject<PaymentSessionJs>(js) ?? new();
                dto.Data.ForEach(d => d.Price = 0);
                session.JsText = JsonConvert.SerializeObject(dto);
            }
            var amount = session.CalculatePaymentAmount();
            if (hasData) Assert.True(amount > 0);
        }

        [Fact]
        public void SubscriptionCanMap()
        {
            InvoiceExtensions.GetInfrastructure = null;
            var session = levelBofaker.Generate();
            var html = session.GetHtml(SubscriptionContent, "123-456-789");
            Assert.False(string.IsNullOrEmpty(html));
        }
        [Fact]
        public void DiscountCanMap()
        {
            InvoiceExtensions.GetInfrastructure = null;
            var session = discountBofaker.Generate();
            var html = session.GetHtml(DiscountContent, "123-456-789");
            Assert.False(string.IsNullOrEmpty(html));
        }
    }
}
