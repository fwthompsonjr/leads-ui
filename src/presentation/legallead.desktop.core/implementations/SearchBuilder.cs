using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using System.Runtime.Caching;

namespace legallead.desktop.implementations
{
    internal class SearchBuilder : ISearchBuilder
    {
        private const string PageName = "application-state-configuration";
        private readonly IPermissionApi _api;
        private string? _jsConfiguration;

        public SearchBuilder(IPermissionApi api)
        {
            _api = api;
        }

        public string GetHtml()
        {
            var search = Configuration()?.ToList();

            var doc = new HtmlDocument();
            var parent = doc.CreateElement("div");
            var wrapper = doc.CreateElement("div");
            var table = doc.CreateElement("table");
            var tbody = doc.CreateElement("tbody");
            var tfoot = doc.CreateElement("tfoot");
            // build elements
            BuildWrapper(wrapper);
            BuildParent(doc, parent);
            BuildTable(doc, table);
            BuildTableBody(doc, tbody, search);
            BuildTableFooter(doc, tfoot, _jsConfiguration);
            // append to parents
            parent.AppendChild(wrapper);
            table.AppendChild(tbody);
            table.AppendChild(tfoot);
            wrapper.AppendChild(table);
            return parent.OuterHtml;
        }

        public StateSearchConfiguration[]? GetConfiguration()
        {
            return Configuration();
        }

        protected StateSearchConfiguration[]? Configuration()
        {
            if (string.IsNullOrEmpty(_jsConfiguration))
            {
                var response = _api.Get(PageName).Result;
                if (response == null || response.StatusCode != 200) return null;
                if (string.IsNullOrEmpty(response.Message)) return null;
                _jsConfiguration = response.Message;
            }
            return ObjectExtensions.TryGet<List<StateSearchConfiguration>>(_jsConfiguration).ToArray();
        }

        private static void BuildWrapper(HtmlNode node)
        {
            node.Attributes.Add("id", "dv-search-table-wrapper");
            node.Attributes.Add("name", "search-wrapper");
            node.Attributes.Add("class", "table-responsive");
        }

        private static void BuildParent(HtmlDocument doc, HtmlNode node)
        {
            var heading = doc.CreateElement("h4");
            var subheading = doc.CreateElement("p");
            heading.InnerHtml = "Search";
            subheading.InnerHtml = "Complete the fields below to begin search";
            subheading.Attributes.Add("class", "lead");
            node.Attributes.Add("id", "dv-search-container");
            node.Attributes.Add("name", "search-container");
            node.Attributes.Add("class", "container p-2 w-75 rounded border-secondary");
            node.AppendChild(heading);
            node.AppendChild(subheading);
        }

        private static void BuildTable(HtmlDocument doc, HtmlNode node)
        {
            var cols = new[] { "width: 150px;", "" }.ToList();
            var colgroup = doc.CreateElement("colgroup");
            cols.ForEach(c =>
            {
                var col = doc.CreateElement("col");
                if (!string.IsNullOrEmpty(c)) col.Attributes.Add("style", c);
                colgroup.AppendChild(col);
            });
            node.Attributes.Add("id", "table-search");
            node.Attributes.Add("name", "search-table");
            node.Attributes.Add("class", "container p-2 w-75 rounded border-secondary");
            node.AppendChild(colgroup);
        }

        private static void BuildTableBody(HtmlDocument doc, HtmlNode node, List<StateSearchConfiguration>? configurations = null)
        {
            var indexes = new[] { 0, 1 };
            var rows = new[] {
                "State",
                "County",
                "Dynamic-0",
                "Dynamic-1",
                "Dynamic-2",
                "Dynamic-3",
                "Dynamic-4",
                "Dynamic-5",
                "Dynamic-6",
                "Start Date",
                "End Date"}.ToList();
            rows.ForEach(r =>
            {
                var tr = doc.CreateElement("tr");
                var lbl = doc.CreateElement("label");
                var td1 = doc.CreateElement("td");
                var td2 = doc.CreateElement("td");
                var rwname = r.Replace(' ', '-').ToLower();
                tr.Attributes.Add("id", $"tr-search-{rwname}");
                lbl.InnerHtml = r;
                td1.AppendChild(lbl);
                if (r.EndsWith("Date"))
                {
                    var tbx = doc.CreateElement("input");
                    var tbname = r.Replace(" ", string.Empty).ToLower();
                    tbx.Attributes.Add("id", $"tbx-search-{tbname}");
                    tbx.Attributes.Add("name", "search-field");
                    tbx.Attributes.Add("type", "date");
                    tbx.Attributes.Add("class", "form-control");
                    td2.AppendChild(tbx);
                }
                else
                {
                    var cbo = doc.CreateElement("select");
                    var cboname = r.Replace(" ", string.Empty).ToLower();
                    cbo.Attributes.Add("id", $"cbo-search-{cboname}");
                    cbo.Attributes.Add("name", "search-field");
                    cbo.Attributes.Add("class", "form-control");
                    var indx = rows.IndexOf(r);
                    if (indexes.Contains(indx)) PopulateOptions(indx, cbo, configurations);
                    td2.AppendChild(cbo);
                }
                if (r.StartsWith("Dynamic-"))
                {
                    tr.Attributes.Add("style", "display: none");
                }
                tr.AppendChild(td1);
                tr.AppendChild(td2);
                node.AppendChild(tr);
            });
        }

        private static void BuildTableFooter(HtmlDocument doc, HtmlNode node, string? json = null)
        {
            var tr = doc.CreateElement("tr");
            var td = doc.CreateElement("td");
            var tarea = doc.CreateElement("textarea");
            var button = doc.CreateElement("button");
            td.Attributes.Add("colspan", "2");
            td.Attributes.Add("class", "p-1");
            button.Attributes.Add("id", "search-submit-button");
            button.Attributes.Add("class", "btn btn-primary");
            button.InnerHtml = "Search";
            tarea.Attributes.Add("style", "display: none");
            tarea.InnerHtml = json ?? string.Empty;
            td.AppendChild(button);
            td.AppendChild(tarea);
            tr.AppendChild(td);
            node.AppendChild(tr);
        }

        private static void PopulateOptions(int indx, HtmlNode cbo, List<StateSearchConfiguration>? configurations = null)
        {
            var indexes = new[] { 0, 1 };
            if (configurations == null) return;
            if (!indexes.Contains(indx)) return;
            if (indx == 0) AppendStates(cbo, configurations);
            if (indx == 1) AppendCounties(cbo, configurations);
        }

        private static void AppendCounties(HtmlNode cbo, List<StateSearchConfiguration>? config)
        {
            if (config == null) return;
            var nde = cbo.OwnerDocument.CreateElement("option");
            nde.Attributes.Add("value", "0");
            nde.Attributes.Add("dat-county-index", "");
            nde.Attributes.Add("dat-state-index", "");
            nde.InnerHtml = "- select -";
            cbo.AppendChild(nde);
            var temp = config.SelectMany(s => s.Counties).ToList();

            temp.Sort((a, b) =>
            {
                var aa = (a.StateCode ?? string.Empty).CompareTo(b.StateCode ?? string.Empty);
                if (aa != 0) return aa;
                return (a.Name ?? string.Empty).CompareTo(b.Name ?? string.Empty);
            });

            temp.ForEach(s =>
            {
                var id = s.Index.ToString();
                var abrev = s.Name?.ToLower() ?? id;
                var itm = cbo.OwnerDocument.CreateElement("option");
                itm.Attributes.Add("value", id);
                itm.Attributes.Add("dat-state-index", s.StateCode?.ToLower() ?? id);
                itm.Attributes.Add("dat-county-index", abrev);
                itm.Attributes.Add("style", "display: none");
                itm.InnerHtml = s.Name ?? id;
                cbo.AppendChild(itm);
            });
        }

        private static void AppendStates(HtmlNode cbo, List<StateSearchConfiguration>? config)
        {
            if (config == null) return;
            var nde = cbo.OwnerDocument.CreateElement("option");
            nde.Attributes.Add("value", "0");
            nde.Attributes.Add("dat-state-index", "");
            nde.InnerHtml = "- select -";
            cbo.AppendChild(nde);
            config.ForEach(s =>
            {
                var id = (config.IndexOf(s) + 1).ToString();
                var abrev = s.ShortName?.ToLower() ?? id;
                var itm = cbo.OwnerDocument.CreateElement("option");
                itm.Attributes.Add("value", id);
                itm.Attributes.Add("dat-state-index", abrev);
                itm.InnerHtml = s.Name ?? id;
                cbo.AppendChild(itm);
            });
        }
    }
}