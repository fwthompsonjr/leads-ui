﻿using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Newtonsoft.Json;

namespace legallead.desktop.utilities
{
    internal static class SearchHistoryContentHelper
    {
        /*
        
		<tr id="tr-subcontent-purchases-data-template" style="display: none" search-uuid="~0">
			<td class="text-end"><span name="purchase-date">~1</span></td>
			<td><span name="item-name">~2</span></td>
			<td class="text-end"><span name="item-quantity">~3</span></td>
			<td class="text-end"><span name="total-price">~4</span></td>
			<td class="text-end" search-uuid="~0"><span name="purchase-status">~5</span></td>
		</tr>

        */
        public static async Task<string> InjectHistory(IPermissionApi api, UserBo user, string html)
        {
            try
            {
                var payload = new { id = Guid.NewGuid().ToString(), name = "legallead.permissions.api" };
                var response = await api.Post("search-get-purchases", payload, user);
                if (response == null || response.StatusCode != 200) return html;
                var data = GetData(response);
                if (data == null || !data.Any()) return html;
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var parent = doc.DocumentNode;
                var template = parent.SelectSingleNode("//*[@id='tr-subcontent-purchases-data-template']");
                var header = template.ParentNode;
                data.ForEach(x => { AppendRow(template, header, x); });
                return doc.DocumentNode.OuterHtml;
            }
            catch
            {
                return html;
            }
        }

        private static void AppendRow(HtmlNode template, HtmlNode header, MyPurchaseBo x)
        {
            var document = header.OwnerDocument;
            var newNode = document.CreateElement("tr");
            var txt = template.InnerHtml;
            txt = txt.Replace("~0", x.ReferenceId ?? "");
            txt = txt.Replace("~1", x.PurchaseDate.GetValueOrDefault().ToString("MM/dd/yyyy"));
            txt = txt.Replace("~2", x.ItemType ?? "");
            txt = txt.Replace("~3", x.ItemCount.GetValueOrDefault().ToString() ?? "");
            txt = txt.Replace("~4", x.Price.GetValueOrDefault().ToString("c") ?? "");
            txt = txt.Replace("~5", x.StatusText ?? "");
            newNode.InnerHtml = txt;
            header.AppendChild(newNode);
        }

        private static List<MyPurchaseBo> GetData(ApiResponse response)
        {
            var message = response.Message;
            if (string.IsNullOrWhiteSpace(message)) return new();
            var data = JsonConvert.DeserializeObject<List<MyPurchaseBo>>(message) ?? new();
            return data;
        }
    }
}