using HtmlAgilityPack;
using legallead.jdbc.entities;
using legallead.permissions.api.Models;
using Newtonsoft.Json;
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
        public static long CalculatePaymentAmount(this PaymentSessionDto? response)
        {
            if (response == null) return 0;
            var js = response.JsText;
            var dto = string.IsNullOrWhiteSpace(js) ?
                new() :
                JsonConvert.DeserializeObject<PaymentSessionJs>(js) ?? new();
            var data = dto.Data;
            if (data == null || !data.Any()) return 0;
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
            if (data == null || !data.Any()) return html;

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

        private static string GetDescription(string? desciption, string fallback)
        {
            if (string.IsNullOrWhiteSpace(desciption)) return fallback;
            return desciption.Replace("Record Search :", "Search: "); // dash
        }
        private static string? _invoiceScript;
        private static string InvoiceScript()
        {
            if(!string.IsNullOrWhiteSpace(_invoiceScript)) return _invoiceScript;
            _invoiceScript = Properties.Resources.page_invoice_js;
            return _invoiceScript;
        }
        private const string InvoiceScriptTag = "<!-- stripe payment script -->";
    }
}
