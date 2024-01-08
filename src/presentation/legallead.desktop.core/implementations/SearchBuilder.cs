using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Newtonsoft.Json;
using System.Linq;

namespace legallead.desktop.implementations
{
    internal class SearchBuilder : ISearchBuilder
    {
        private const string PageName = "application-state-configuration";
        private static ContentParser Parser = new();
        private readonly IPermissionApi _api;
        private string? _jsConfiguration;

        public SearchBuilder(IPermissionApi api)
        {
            _api = api;
        }

        public string GetHtml()
        {
            const string fmt = "<html><body>{0}</body></html>";
            var search = Configuration()?.ToList();

            var doc = new HtmlDocument();
            var parent = doc.CreateElement("div");
            var wrapper = doc.CreateElement("div");
            var table = doc.CreateElement("table");
            var tbody = doc.CreateElement("tbody");
            var tfoot = doc.CreateElement("tfoot");
            // build elements
            BuildWrapper(wrapper);
            var parentIndex = BuildParent(doc, parent);
            BuildTable(doc, table);
            BuildTableBody(doc, tbody, search);
            var jsonIndex = BuildTableFooter(doc, tfoot, _jsConfiguration);
            // append to parents
            parent.AppendChild(wrapper);
            table.AppendChild(tbody);
            table.AppendChild(tfoot);
            wrapper.AppendChild(table);
            var tempHtml = parent.OuterHtml;
            var html = Parser.BeautfyHTML(string.Format(fmt, tempHtml));
            doc = new HtmlDocument();
            doc.LoadHtml(html);
            var element = doc.DocumentNode.SelectSingleNode($"//*[@id='{parentIndex}']");
            if (element == null) return tempHtml;
            tempHtml = element.OuterHtml;
            var txtbox = element.SelectSingleNode($"//*[@id='{jsonIndex}']");
            if (txtbox == null) return tempHtml;
            txtbox.InnerHtml = _jsConfiguration ?? string.Empty;
            return element.OuterHtml;
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
                var mapped = ObjectExtensions.TryGet<List<StateSearchConfiguration>>(_jsConfiguration);
                if (mapped.Any())
                {
                    _jsConfiguration = JsonConvert.SerializeObject(mapped, Formatting.None);
                }
            }
            return ObjectExtensions.TryGet<List<StateSearchConfiguration>>(_jsConfiguration).ToArray();
        }

        private static void BuildWrapper(HtmlNode node)
        {
            node.Attributes.Add("id", "dv-search-table-wrapper");
            node.Attributes.Add("name", "search-wrapper");
            node.Attributes.Add("class", "table-responsive");
        }

        private static string BuildParent(HtmlDocument doc, HtmlNode node)
        {
            const string parentId = "dv-search-container";
            var heading = doc.CreateElement("h4");
            var subheading = doc.CreateElement("p");
            heading.InnerHtml = "Search";
            subheading.InnerHtml = "Complete the fields below to begin search";
            subheading.Attributes.Add("class", "lead");
            node.Attributes.Add("id", parentId);
            node.Attributes.Add("name", "search-container");
            node.Attributes.Add("class", "container p-2 w-75 rounded border-secondary");
            node.AppendChild(heading);
            node.AppendChild(subheading);
            return parentId;
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
                var isParameterRow = r.StartsWith("Dynamic-");
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
                tr.AppendChild(td1);
                tr.AppendChild(td2);
                if (isParameterRow)
                {
                    tr.Attributes.Add("style", "display: none");
                    var rowId = Convert.ToInt32(r.Split('-')[1]);
                    PopulateParameters(rowId, tr, configurations);
                }
                node.AppendChild(tr);
            });
        }

        private static string BuildTableFooter(HtmlDocument doc, HtmlNode node, string? json = null)
        {
            const string tareaId = "tarea-search-js-content";
            var tr = doc.CreateElement("tr");
            var td = doc.CreateElement("td");
            var tarea = doc.CreateElement("textarea");
            var button = doc.CreateElement("button");
            td.Attributes.Add("colspan", "2");
            td.Attributes.Add("class", "p-1");
            button.Attributes.Add("id", "search-submit-button");
            button.Attributes.Add("class", "btn btn-primary");
            button.InnerHtml = "Search";
            tarea.Attributes.Add("id", tareaId);
            tarea.Attributes.Add("style", "display: none");
            tarea.InnerHtml = json ?? string.Empty;
            td.AppendChild(button);
            td.AppendChild(tarea);
            tr.AppendChild(td);
            node.AppendChild(tr);
            return tareaId;
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
                var hasCase = (s.Data?.CaseSearchTypes ?? Array.Empty<CaseSearchModel>()).Length > 0;
                var dropDownCount = 0;
                if (s.Data != null) { dropDownCount = s.Data.DropDowns.Length; }
                var id = s.Index.ToString();
                var abrev = s.Name?.ToLower() ?? id;
                var itm = cbo.OwnerDocument.CreateElement("option");
                itm.Attributes.Add("value", id);
                itm.Attributes.Add("dat-state-index", s.StateCode?.ToLower() ?? id);
                itm.Attributes.Add("dat-county-index", abrev);
                itm.Attributes.Add("dat-has-case-search", hasCase ? "true" : "false");
                itm.Attributes.Add("dat-parameter-count", dropDownCount.ToString());
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

        private static void PopulateOptions(int indx, HtmlNode cbo, List<StateSearchConfiguration>? configurations = null)
        {
            var indexes = new[] { 0, 1 };
            if (configurations == null) return;
            if (!indexes.Contains(indx)) return;
            if (indx == 0) AppendStates(cbo, configurations);
            if (indx == 1) AppendCounties(cbo, configurations);
        }

        private static void PopulateParameters(int indx, HtmlNode tr, List<StateSearchConfiguration>? configurations = null)
        {
            var indexes = new[] { 0, 1, 2, 3, 4, 5, 6 };
            if (configurations == null) return;
            if (!indexes.Contains(indx)) return;
            var cbo = FindComboBox(tr);
            if (cbo == null) return;
            /* find all configurations,
             * county having count of drop-downs
             * greater than zero */
            if (indx != indexes[^1])
            {
                AppendDropDownValues(indx, cbo, configurations);
                return;
            }
            AppendCaseTypeValues(cbo, configurations);
        }

        private static HtmlNode? FindComboBox(HtmlNode tr)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            if (tr.ChildNodes.Count != 2) return null;
            if (tr.ChildNodes[1].ChildNodes.Count < 1) return null;
            var element = tr.ChildNodes[1].ChildNodes[0];
            if (!element.Name.Equals("select", oic)) return null;
            return element;
        }

        private static void AppendDropDownValues(int indx, HtmlNode cbo, List<StateSearchConfiguration> configurations)
        {
            var temp = configurations
            .SelectMany(s => s.Counties)
            .Where(w => w.Data != null && w.Data.DropDowns.Length > 0)
            .ToList();
            if (!temp.Any()) return;
            CreateDefaultCountyOption(cbo);
            temp.ForEach(t =>
            {
                if (t.Data != null && t.Data.DropDowns.Any())
                {
                    var dd = t.Data.DropDowns;
                    foreach (var dd2 in dd) { dd2.CountyId = t.Index; }
                }
            });
            var dropdowns = temp.Select(s =>
            {
                if (s.Data == null || !s.Data.DropDowns.Any() || s.Data.DropDowns.Length < indx)
                    return null;
                return s.Data.DropDowns;
            }).Where(w => w != null).ToList();
            dropdowns.ForEach(s =>
            {
                if (s != null)
                {
                    foreach (var item in s)
                    {
                        if (item.Id != indx) continue;
                        var countyIndex = item.CountyId.GetValueOrDefault();
                        var countyNumber = countyIndex.ToString();
                        var rowLabel = item.Name ?? $"Parameter {indx + 1}";
                        foreach (var (child, itm) in from child in item.Members
                                                     where item.IsDisplayed.GetValueOrDefault()
                                                     let itm = cbo.OwnerDocument.CreateElement("option")
                                                     select (child, itm))
                        {
                            itm.Attributes.Add("value", child.Id.ToString());
                            itm.Attributes.Add("dat-row-index", indx.ToString());
                            itm.Attributes.Add("dat-row-name", rowLabel);
                            itm.Attributes.Add("dat-county-index", countyNumber);
                            itm.Attributes.Add("style", "display: none");
                            itm.InnerHtml = child.Name ?? child.Id.ToString();
                            cbo.AppendChild(itm);
                        }
                    }
                }
            });
        }

        private static void AppendCaseTypeValues(HtmlNode cbo, List<StateSearchConfiguration> configurations)
        {
            var temp = configurations
            .SelectMany(s => s.Counties)
            .Where(w => w.Data != null && w.Data.CaseSearchTypes != null)
            .ToList();
            if (!temp.Any()) return;
            CreateDefaultCountyOption(cbo);
            temp.ForEach(t =>
            {
                if (t.Data?.CaseSearchTypes != null && t.Data.CaseSearchTypes.Any())
                {
                    var dd = t.Data.CaseSearchTypes;
                    foreach (var dd2 in dd) { dd2.CountyId = t.Index; }
                }
            });
            var dropdowns = temp.Select(s =>
            {
                if (s.Data?.CaseSearchTypes == null || !s.Data.CaseSearchTypes.Any())
                    return null;
                return new { searches = s.Data.CaseSearchTypes };
            }).Where(w => w != null).ToList();
            dropdowns.ForEach(s =>
            {
                if (s != null)
                {
                    foreach (var item in s.searches)
                    {
                        var id = item.CountyId.GetValueOrDefault();
                        var itm = cbo.OwnerDocument.CreateElement("option");
                        itm.Attributes.Add("value", item.Id.ToString());
                        itm.Attributes.Add("dat-county-index", id.ToString());
                        itm.Attributes.Add("style", "display: none");
                        itm.InnerHtml = item.Name ?? item.Id.ToString();
                        cbo.AppendChild(itm);
                    }
                }
            });
        }

        private static void CreateDefaultCountyOption(HtmlNode cbo)
        {
            if (cbo.HasChildNodes) return;
            var nde = cbo.OwnerDocument.CreateElement("option");
            nde.Attributes.Add("value", "");
            nde.Attributes.Add("dat-county-index", "");
            nde.Attributes.Add("dat-state-index", "");
            nde.InnerHtml = "- select -";
            cbo.AppendChild(nde);
        }
    }
}