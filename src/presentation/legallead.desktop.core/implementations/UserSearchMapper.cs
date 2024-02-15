using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Newtonsoft.Json;

namespace legallead.desktop.implementations
{
    internal class UserSearchMapper : IUserSearchMapper
    {
        public async Task<string> Map(IPermissionApi api, UserBo user, string source, string target)
        {
            var key = (target ?? string.Empty).Trim().ToLower();
            if (!Substitutions.ContainsKey(key)) { return source; }
            switch (key)
            {
                case "history":
                    var content = await MapHistory(api, user, source);
                    return content;
            }
            return source;
        }


        private async Task<string> MapHistory(IPermissionApi api, UserBo user, string source)
        {
            var payload = new { Id = Guid.NewGuid().ToString(), Name = "legallead.permissions.api" };
            var template = Substitutions["history"];
            var response = await api.Post("search-get-history", payload, user);
            if (response.StatusCode != 200) return source;
            var json = response.Message;
            var items = JsonConvert.DeserializeObject<List<UserSearchQueryBo>>(json);
            if (items == null || items.Count == 0) return source;
            var document = ToDocument(source);
            var transform = TransformRows(document, items.Cast<ISearchIndexable>().ToList(), template);
            var styled = ApplyHistoryStatus(ToDocument(transform), template);
            return styled;
        }

        private static string ApplyHistoryStatus(HtmlDocument document, MySearchSubstitutions substitutions)
        {
            var node = document.DocumentNode;
            var table = node.SelectSingleNode(substitutions.Table);
            if (table == null) return node.OuterHtml;
            var tbody = table.SelectSingleNode("tbody");
            if (tbody == null) return node.OuterHtml;
            var rows = tbody.SelectNodes("//tr[@data-position]")?.ToList();
            if (rows == null || rows.Count == 0) return node.OuterHtml;
            rows.ForEach(row =>
            {
                var status = row.SelectNodes("td")?.ToList()[^1].SelectSingleNode("span");
                var text = status?.InnerText.Trim();
                if (status != null && !string.IsNullOrEmpty(text))
                {
                    var stsCss = GetStatusCss(text);
                    if (stsCss != null)
                    {
                        var attr = document.CreateAttribute("class", stsCss);
                        status.Attributes.Add(attr);
                    }
                }
            });
            return node.OuterHtml;
        }
        private static string? GetStatusCss(string searchStatus)
        {
            return searchStatus switch
            {
                "Completed" => "text-success",
                "Processing" => "text-warning-emphasis",
                "Error" => "text-danger",
                "Purchased" => "text-info",
                "Downloaded" => "text-primary",
                _ => null,
            };
        }
        private readonly Dictionary<string, MySearchSubstitutions> Substitutions =
            new()
            {
                { "history", JsonConvert.DeserializeObject<MySearchSubstitutions>(substitutions_history) ?? new() }
            };
        private static readonly string substitutions_history = "{ " + Environment.NewLine +
            " \"table\": \"//table[@name='search-dt-table']\", " + Environment.NewLine +
            " \"template\": \"//tr[@id='tr-subcontent-history-data-template']\", " + Environment.NewLine +
            " \"nodatatemplate\": \"//tr[@id='tr-subcontent-history-no-data']\", " + Environment.NewLine +
             " \"targets\": 6 " + Environment.NewLine + " }";

        private static HtmlDocument ToDocument(string content)
        {
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;
        }

        private static string TransformRows(HtmlDocument document, List<ISearchIndexable> data, MySearchSubstitutions substitutions)
        {
            var node = document.DocumentNode;
            var table = node.SelectSingleNode(substitutions.Table);
            var template = node.SelectSingleNode(substitutions.Template);
            var nodata = node.SelectSingleNode(substitutions.NoDataTemplate);
            if (table == null || template == null) return node.OuterHtml;
            var tbody = table.SelectSingleNode("tbody");
            var style = document.CreateAttribute("style", "display: none");
            nodata.Attributes.Add(style);
            foreach (var item in data)
            {
                var r = data.IndexOf(item);
                var pg = r / tableRowCount;
                var position = r % 2 == 0 ? "even" : "odd";
                var rowdata = document.CreateElement("tr");
                var attr = document.CreateAttribute("search-uuid", item[0]);
                var rwnumber = document.CreateAttribute("data-row-number", r.ToString());
                var pgnumber = document.CreateAttribute("data-page-number", pg.ToString());
                var attrwpos = document.CreateAttribute("data-position", position);
                var rwstyle = document.CreateAttribute("style", "display: none");
                rowdata.Attributes.Add(attr);
                rowdata.Attributes.Add(rwnumber);
                rowdata.Attributes.Add(pgnumber);
                rowdata.Attributes.Add(attrwpos);
                if (pg > 0) rowdata.Attributes.Add(rwstyle);
                var row = template.InnerHtml.Replace("~0", r.ToString());
                for (var i = 1; i < substitutions.Targets + 1; i++)
                {
                    var search = $"~{i}";
                    row = row.Replace(search, item[i]);
                }
                rowdata.InnerHtml = row;
                tbody.AppendChild(rowdata);
            }
            TransformPaging(table, data);
            return node.OuterHtml;
        }

        private static void TransformPaging(HtmlNode table, List<ISearchIndexable> data)
        {
            var tfoot = table.SelectSingleNode("tfoot");
            var trow = tfoot.SelectSingleNode("tr");
            var cells = trow.SelectNodes("td").ToArray();
            var cbo = cells[0].SelectSingleNode("select");
            var td = cells[1];
            td.InnerHtml = $"Records: {data.Count}";
            cbo.ChildNodes.Clear();
            var doc = table.OwnerDocument;
            if (data.Count > 0 && tfoot.Attributes["class"] != null)
            {
                var attr = tfoot.Attributes["class"];
                attr.Value = attr.Value.Replace("d-none", "").Trim();
            }
            for (var i = 0; i < data.Count; i += tableRowCount)
            {
                var pg = i / tableRowCount;
                var mx = Math.Min(i + tableRowCount, data.Count);
                var lbl = $"Records: {i + 1} to {mx}";
                var optn = doc.CreateElement("option");
                var att1 = doc.CreateAttribute("value", pg.ToString());
                optn.InnerHtml = lbl;
                optn.Attributes.Append(att1);
                if (i == 0)
                {
                    var att2 = doc.CreateAttribute("selected", "selected");
                    optn.Attributes.Add(att2);
                }
                cbo.AppendChild(optn);
            }
        }

        private const int tableRowCount = 10;
    }
}
