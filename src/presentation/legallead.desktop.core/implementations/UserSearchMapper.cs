using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return TransformHistory(document, items, template);

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

        private static string TransformHistory(HtmlDocument document, List<UserSearchQueryBo> data, MySearchSubstitutions substitutions)
        {
            var node = document.DocumentNode;
            var table = node.SelectSingleNode(substitutions.Table);
            var template = node.SelectSingleNode(substitutions.Template);
            var nodata = node.SelectSingleNode(substitutions.NoDataTemplate);
            if (table == null || template == null) return node.OuterHtml;
            var tbody = table.SelectSingleNode("tbody");
            var style = document.CreateAttribute("style", "display: none");
            nodata.Attributes.Add(style);
            foreach ( var item in data )
            {
                var r = data.IndexOf(item);
                var pg = r / 10;
                var rowdata = document.CreateElement("tr");
                var attr = document.CreateAttribute("search-uuid", item[0]);
                var rwnumber = document.CreateAttribute("data-row-number", r.ToString());
                var pgnumber = document.CreateAttribute("data-page-number", pg.ToString()); 
                var rwstyle = document.CreateAttribute("style", "display: none");
                rowdata.Attributes.Add(attr);
                rowdata.Attributes.Add(rwnumber);
                rowdata.Attributes.Add(pgnumber);
                if (pg > 0) rowdata.Attributes.Add(rwstyle);
                var row = template.InnerHtml;
                for ( var i = 1; i < substitutions.Targets + 1; i++ ) 
                {
                    var search = $"~{i}";
                    row = row.Replace(search, item[i]);
                }
                rowdata.InnerHtml = row;
                tbody.AppendChild(rowdata);
            }
            return node.OuterHtml;
        }
    }
}
