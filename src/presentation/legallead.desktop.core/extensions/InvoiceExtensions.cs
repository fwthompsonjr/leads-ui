using HtmlAgilityPack;
using legallead.desktop.entities;
using System.Text;

namespace legallead.desktop.extensions
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

        public static string GetHtml(this GenerateInvoiceResponse? response, string html, string paymentKey)
        {
            const string dash = " - ";
            if (response == null) return html;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var parentNode = doc.DocumentNode;
            var detailNode = parentNode.SelectSingleNode("//ul[@name='invoice-line-items']");
            if (detailNode == null) return html;
            detailNode.InnerHtml = string.Empty;
            if (response.Data == null || !response.Data.Any()) return html;
            response.Data.ForEach(d =>
            {
                var li = d.GetItem(doc);
                detailNode.AppendChild(li);
            });
            var createDate = response.Data[0].CreateDate.GetValueOrDefault().ToString("f");
            var totalCost = response.Data.Sum(x => x.Price.GetValueOrDefault());
            var replacements = new Dictionary<string, string>()
            {
                { "//span[@name='invoice']", response.ExternalId ?? dash },
                { "//span[@name='invoice-date']", createDate },
                { "//span[@name='invoice-description']", GetDescription(response.Description, dash) },
                { "//span[@name='invoice-total']", totalCost.ToString("c") ?? dash }
            };
            var keys = replacements.Keys.ToList();
            keys.ForEach(key =>
            {
                var span = parentNode.SelectSingleNode(key);
                if (span != null) span.InnerHtml = replacements[key];
            });
            if (totalCost < 0.50f)
            {
                RemoveCheckout(parentNode);
                return parentNode.OuterHtml;
            }
            var outerHtml = parentNode.OuterHtml;
            outerHtml = outerHtml.Replace("<!-- stripe public key -->", paymentKey);
            outerHtml = outerHtml.Replace("<!-- stripe client secret -->", response.ClientSecret ?? dash);
            doc = new HtmlDocument();
            doc.LoadHtml(outerHtml);
            return doc.DocumentNode.OuterHtml;
        }

        private static void RemoveCheckout(HtmlNode parentNode)
        {
            var stripejs = "//script[@name='stripe-api']";
            var chkoutjs = "//script[@name='checkout-stripe-js']";
            var chkoutdv = "//*[@id='checkout']";
            var elements = new[] {
                chkoutdv,
                stripejs
            };
            foreach (var elem in elements) 
            { 
                var node = parentNode.SelectSingleNode(elem);
                if (node != null) {
                    node.ParentNode.RemoveChild(node);
                }
            }
            var script = parentNode.SelectSingleNode(chkoutjs);
            if (script != null) { script.InnerHtml = string.Empty; }
        }

        private static HtmlNode GetItem(this InvoiceResponseData data, HtmlDocument document)
        {
            var nwl = Environment.NewLine;
            var hasLine = int.TryParse(data.LineId, out var lineId);
            if (!hasLine) { lineId = -1; }
            var line = $"{data.ItemCount} records";
            if (lineId != 0)
            {
                line = (data.UnitPrice.GetValueOrDefault(0) * 100f).ToString("F2") + " %";
            }
            var node = document.CreateElement("li");
            var attr = document.CreateAttribute("style", "background: transparent; border-color: #444");
            var attr1 = document.CreateAttribute("class", "list-group-item text-white");
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendFormat("{0}<br/>{1}", data.ItemType, nwl);
            sb.AppendFormat("{0}<br/>{1}", line, nwl);
            sb.AppendLine(data.Price.GetValueOrDefault().ToString("c"));
            node.Attributes.Add(attr);
            node.Attributes.Add(attr1);
            node.InnerHtml = sb.ToString();
            return node;
        }

        private static string GetDescription(string? desciption, string fallback)
        {
            if(string.IsNullOrWhiteSpace(desciption)) return fallback;
            return desciption.Replace("Record Search :", "Search: "); // dash
        }
    }
}
