using HtmlAgilityPack;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Models;
using legallead.permissions.api.Utility;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace legallead.permissions.api.Extensions
{
    internal static class InvoiceExtensions
    {
        /*
        <li class="list-group-item text-white" style="background: transparent">
			Search - Level: Guest<br/>
			248 records<br/>
			$ 0.00
		</li>
        background: transparent; border-color: #444
        
        */
        private static readonly object locker = new();
        private static IStripeInfrastructure? stripeServices;
        internal static IStripeInfrastructure? GetInfrastructure
        {
            get
            {
                lock (locker)
                {
                    return stripeServices;
                }
            }
            set
            {
                lock (locker)
                {
                    stripeServices = value;
                }
            }
        }
        public static long CalculatePaymentAmount(this PaymentSessionDto? response)
        {
            if (response == null) return 0;
            var js = response.JsText;
            var dto = string.IsNullOrWhiteSpace(js) ?
                new() :
                JsonConvert.DeserializeObject<PaymentSessionJs>(js) ?? new();
            var data = dto.Data;
            if (data == null || data.Count == 0) return 0;
            var totalCost = data.Sum(x => x.Price.GetValueOrDefault()) * 100;
            return Convert.ToInt64(totalCost);
        }

        public static string GetHtml(this PaymentSessionDto? response, string html, string paymentKey)
        {
            const string dash = " - ";
            if (response == null) return html;
            html = html.Replace(InvoiceScriptTag, InvoiceScript());
            var js = response.JsText;
            var dto = string.IsNullOrWhiteSpace(js) ?
                new() :
                JsonConvert.DeserializeObject<PaymentSessionJs>(js) ?? new();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var parentNode = doc.DocumentNode;
            var detailNode = parentNode.SelectSingleNode("//ul[@name='invoice-line-items']");
            if (detailNode == null) return html;
            detailNode.InnerHtml = string.Empty;
            var data = dto.Data;
            if (data == null || data.Count == 0) return html;

            data.ForEach(d =>
            {
                var li = d.GetItem(doc);
                detailNode.AppendChild(li);
            });
            var createDate = data[0].CreateDate.GetValueOrDefault().ToString("f");
            var externalId = response.ExternalId ?? dash;
            var totalCost = data.Sum(x => x.Price.GetValueOrDefault());
            var replacements = new Dictionary<string, string>()
            {
                { "//span[@name='invoice']", externalId },
                { "//span[@name='invoice-date']", createDate },
                { "//span[@name='invoice-description']", GetDescription(dto.Description, dash) },
                { "//span[@name='invoice-total']", totalCost.ToString("c") ?? dash }
            };
            var keys = replacements.Keys.ToList();
            keys.ForEach(key =>
            {
                var span = parentNode.SelectSingleNode(key);
                if (span != null) span.InnerHtml = replacements[key];
            });
            if (totalCost < 0.50m)
            {
                RemoveCheckout(parentNode);
                return parentNode.OuterHtml;
            }
            var outerHtml = parentNode.OuterHtml;
            outerHtml = outerHtml.Replace("<!-- stripe public key -->", paymentKey);
            outerHtml = outerHtml.Replace("<!-- stripe client secret -->", response.ClientId ?? dash);
            outerHtml = outerHtml.Replace("<!-- payment external id -->", externalId);
            outerHtml = outerHtml.Replace("<!-- payment completed url -->", dto.SuccessUrl ?? dash);
            doc = new HtmlDocument();
            doc.LoadHtml(outerHtml);
            return doc.DocumentNode.OuterHtml;
        }


        public static string GetHtml(
            this LevelRequestBo response,
            string html,
            string paymentKey,
            ICustomerRepository? customerDb = null)
        {
            const string dash = " - ";
            if (string.IsNullOrEmpty(response.SessionId)) return html;
            html = html.Replace(InvoiceScriptTag, InvoiceSubscriptionScript());
            var verification = StripeSubscriptionRetryService.VerifySubscription(response, customerDb).GetAwaiter().GetResult();
            if (!verification.Item1) return html;
            var successUrl = verification.Item2;
            var invoice = verification.Item3;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var parentNode = doc.DocumentNode;
            var detailNode = parentNode.SelectSingleNode("//ul[@name='invoice-line-items']");
            if (detailNode != null) detailNode.InnerHtml = string.Empty;
            var createDate = DateTime.UtcNow.ToString("f");
            var externalId = response.ExternalId ?? dash;
            var heading = "Legal Lead Subscription";
            var description = response.LevelName switch
            {
                "" => "Setup monthly payment",
                null => dash,
                _ => response.LevelName,
            };
            var amount = (invoice.Total * 0.01d).ToString("c");
            var replacements = new Dictionary<string, string>()
            {
                { "//span[@name='invoice']", heading },
                { "//span[@name='invoice-date']", createDate },
                { "//span[@name='invoice-description']", englishText.ToTitleCase(description.ToLower()) },
                { "//span[@name='invoice-total']", amount }
            };
            var keys = replacements.Keys.ToList();
            keys.ForEach(key =>
            {
                var span = parentNode.SelectSingleNode(key);
                if (span != null) span.InnerHtml = replacements[key];
            });
            var outerHtml = parentNode.OuterHtml;
            var domain = GetPaymentIntentUrl(successUrl);
            outerHtml = outerHtml.Replace("<!-- stripe public key -->", paymentKey);
            outerHtml = outerHtml.Replace("<!-- payment external id -->", externalId);
            outerHtml = outerHtml.Replace("<!-- payment completed url -->", successUrl);
            outerHtml = outerHtml.Replace("<!-- payment get intent url -->", domain);
            doc = new HtmlDocument();
            doc.LoadHtml(outerHtml);
            return doc.DocumentNode.OuterHtml;
        }

        public static string GetHtml(
            this DiscountRequestBo response,
            string html,
            string paymentKey,
            ICustomerRepository? customerDb = null)
        {
            const string dash = " - ";
            if (string.IsNullOrEmpty(response.SessionId)) return html;
            html = html.Replace(InvoiceScriptTag, InvoiceDiscountScript());
            var verification = StripeDiscountRetryService.VerifySubscription(
                response, customerDb).GetAwaiter().GetResult();
            if (!verification.Item1) return html;
            var successUrl = verification.Item2;
            var invoice = verification.Item3;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var parentNode = doc.DocumentNode;
            var detailNode = parentNode.SelectSingleNode("//ul[@name='invoice-line-items']");
            if (detailNode != null) detailNode.InnerHtml = string.Empty;
            var createDate = DateTime.UtcNow.ToString("f");
            var externalId = response.ExternalId ?? dash;
            var heading = "Legal Lead Discounts";
            var description = response.LevelName switch
            {
                "" => "Setup monthly payment",
                null => dash,
                _ => GetDiscountDescription(response.LevelName),
            };
            var amount = (invoice.Total * 0.01d).ToString("c");
            var replacements = new Dictionary<string, string>()
            {
                { "//span[@name='invoice']", heading },
                { "//span[@name='invoice-date']", createDate },
                { "//span[@name='invoice-description']", description },
                { "//span[@name='invoice-total']", amount }
            };
            var keys = replacements.Keys.ToList();
            keys.ForEach(key =>
            {
                var span = parentNode.SelectSingleNode(key);
                if (span != null) span.InnerHtml = replacements[key];
            });
            var outerHtml = parentNode.OuterHtml;
            outerHtml = outerHtml.Replace("<!-- stripe public key -->", paymentKey);
            outerHtml = outerHtml.Replace("<!-- payment external id -->", externalId);
            outerHtml = outerHtml.Replace("<!-- payment completed url -->", successUrl);
            doc = new HtmlDocument();
            doc.LoadHtml(outerHtml);
            return doc.DocumentNode.OuterHtml;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        private static void RemoveCheckout(HtmlNode parentNode)
        {
            var stripejs = "//script[@name='stripe-api']";
            var chkoutjs = "//script[@name='checkout-stripe-js']";
            var paymentcss = "//style[@name='custom-payment-css']";
            var chkoutdv = "//*[@id='checkout']";
            var paymentform = "//*[@id='payment-form']";
            var elements = new[] {
                chkoutdv,
                stripejs,
                paymentcss,
                paymentform
            };
            foreach (var elem in elements)
            {
                var node = parentNode.SelectSingleNode(elem);
                node?.ParentNode.RemoveChild(node);
            }
            var script = parentNode.SelectSingleNode(chkoutjs);
            if (script != null) { script.InnerHtml = string.Empty; }
        }
        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        private static HtmlNode GetItem(this SearchInvoiceBo data, HtmlDocument document)
        {
            const string headerformat = "<span>{0}</span><br/>{1}";
            const string unitformat = "<span style='margin-left: 10px'>{0}</span><br/>{1}";
            const string totalformat = "<span style='margin-left: 10px'>{0:c}</span><br/>";
            var nwl = Environment.NewLine;
            var hasLine = int.TryParse(data.LineId, out var lineId);
            if (!hasLine) { lineId = -1; }
            var line = $"{data.ItemCount} records";
            if (lineId != 0)
            {
                line = (data.UnitPrice.GetValueOrDefault(0) * 100).ToString("F2") + " %";
            }
            var node = document.CreateElement("li");
            var attr = document.CreateAttribute("style", "background: transparent; border-color: #444");
            var attr1 = document.CreateAttribute("class", "list-group-item text-white");
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendFormat(headerformat, data.ItemType, nwl);
            if (lineId != 1000) sb.AppendFormat(unitformat, line, nwl);
            sb.AppendLine(string.Format(totalformat, data.Price.GetValueOrDefault()));
            node.Attributes.Add(attr);
            node.Attributes.Add(attr1);
            node.InnerHtml = sb.ToString();
            return node;
        }
        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        private static string GetDescription(string? desciption, string fallback)
        {
            if (string.IsNullOrWhiteSpace(desciption)) return fallback;
            return desciption.Replace("Record Search :", "Search: "); // dash
        }
        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        private static string InvoiceScript()
        {
            if (!string.IsNullOrWhiteSpace(_invoiceScript)) return _invoiceScript;
            _invoiceScript = Properties.Resources.page_invoice_js;
            return _invoiceScript;
        }
        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        private static string InvoiceSubscriptionScript()
        {
            if (!string.IsNullOrWhiteSpace(_invoiceSubscriptionScript)) return _invoiceSubscriptionScript;
            _invoiceSubscriptionScript = Properties.Resources.page_invoice_subscription_js;
            return _invoiceSubscriptionScript;
        }
        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        private static string InvoiceDiscountScript()
        {
            if (!string.IsNullOrWhiteSpace(_invoiceDiscountScript)) return _invoiceDiscountScript;
            _invoiceDiscountScript = Properties.Resources.page_invoice_discount_js;
            return _invoiceDiscountScript;
        }
        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        public static string GetDiscountDescription(string jstext, bool hasTitle = false)
        {
            const string fallback = "n/a";
            const char slash = '\\';
            try
            {
                if (jstext.Contains(slash)) { jstext = jstext.Replace(slash.ToString(), string.Empty); }
                var source = JsonConvert.DeserializeObject<DiscountChangeParent>(jstext);
                if (source == null) return fallback;
                if (!source.Choices.Any(a => a.IsSelected)) return "No Discounts Selected";
                var counties = source.Choices.Where(w => w.IsSelected && !string.IsNullOrWhiteSpace(w.CountyName));
                var countyNames = englishText.ToTitleCase(string.Join(", ", counties.Select(x => x.CountyName)).ToLower());
                var states = source.Choices.Where(w => w.IsSelected && string.IsNullOrWhiteSpace(w.CountyName));
                var stateNames = englishText.ToTitleCase(string.Join(", ", states.Select(x => x.StateName)).ToLower());
                if (string.IsNullOrWhiteSpace(stateNames)) { stateNames = "-None-"; }
                if (string.IsNullOrWhiteSpace(countyNames)) { countyNames = "-None-"; }
                var title = hasTitle ? "Initialize Discount(s) " : string.Empty;
                var items = new[]
                {
                    $"{title}<br/>",
                    $"<span style='margin-left: 10px'>States: {stateNames}</span><br/>",
                    $"<span style='margin-left: 10px'>Counties: {countyNames}</span>",
                };
                return string.Join(Environment.NewLine, items);
            }
            catch (Exception)
            {
                return fallback;
            }

        }


        [ExcludeFromCodeCoverage(Justification = "Private member is accessed from public method")]
        private static string GetPaymentIntentUrl(string landing)
        {
            if (!Uri.TryCreate(landing, UriKind.Absolute, out var url)) return string.Empty;
            var host = (url.Scheme) switch
            {
                "https" => url.Port == 443 ? url.Host : string.Concat(url.Host, ":", url.Port.ToString()),
                "http" => url.Port == 80 ? url.Host : string.Concat(url.Host, ":", url.Port.ToString()),
                _ => url.Host,
            };
            var constructedUrl = $"{url.Scheme}://{host}";
            return constructedUrl;
        }

        private const string InvoiceScriptTag = "<!-- stripe payment script -->";
        private static string? _invoiceScript;
        private static string? _invoiceSubscriptionScript;
        private static string? _invoiceDiscountScript;
        private static readonly TextInfo englishText = new CultureInfo("en-US", false).TextInfo;
    }
}