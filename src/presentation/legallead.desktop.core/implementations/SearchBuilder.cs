using HtmlAgilityPack;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using System.Runtime.Caching;

namespace legallead.desktop.implementations
{
    internal class SearchBuilder : ISearchBuilder
    {
        private const string PageName = "application-state-configuration";
        private const string PageKeyName = "application-state-configuration-key";
        private readonly IPermissionApi _api;
        private static readonly MemoryCache memoryCache = MemoryCache.Default;

        public SearchBuilder(IPermissionApi api)
        {
            _api = api;
        }

        public string GetHtml()
        {
            _ = Configuration();
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
            BuildTableBody(doc, tbody);
            BuildTableFooter(doc, tfoot);
            // append to parents
            parent.AppendChild(wrapper);
            table.AppendChild(tbody);
            table.AppendChild(tfoot);
            wrapper.AppendChild(table);
            return parent.OuterHtml;
        }

        public IEnumerable<StateSearchConfiguration>? GetConfiguration()
        {
            return Configuration();
        }

        protected IEnumerable<StateSearchConfiguration>? Configuration()
        {
            if (!memoryCache.Contains(PageKeyName))
            {
                var expiration = DateTimeOffset.UtcNow.AddMinutes(30);
                var message = string.Empty;
                _ = Task.Run(() =>
                {
                    var response = _api.Get(PageName).Result;
                    if (response == null || response.StatusCode != 200) return null;
                    if (string.IsNullOrEmpty(response.Message)) return null;
                    message = response.Message;
                    return ObjectExtensions.TryGet<List<StateSearchConfiguration>>(message);
                });
                if (string.IsNullOrEmpty(message)) return null;
                memoryCache.Add($"{PageKeyName}-text", message, expiration);
            }
            var keyvalue = memoryCache.Get($"{PageKeyName}-text");
            if (keyvalue is not string config) return null;
            return ObjectExtensions.TryGet<List<StateSearchConfiguration>>(config);
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

        private static void BuildTableBody(HtmlDocument doc, HtmlNode node)
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
                    if (indexes.Contains(indx)) PopulateOptions(indx, cbo);
                    td2.AppendChild(cbo);
                }
                if (r.StartsWith("Dynamic-"))
                {
                    tr.Attributes.Add("class", "d-none");
                }
                tr.AppendChild(td1);
                tr.AppendChild(td2);
                node.AppendChild(tr);
            });
        }

        private static void BuildTableFooter(HtmlDocument doc, HtmlNode node)
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
            tarea.InnerHtml = ConfigurationText();
            td.AppendChild(button);
            td.AppendChild(tarea);
            tr.AppendChild(td);
            node.AppendChild(tr);
        }

        private static void PopulateOptions(int indx, HtmlNode cbo)
        {
            var indexes = new[] { 0, 1 };
            var config = GetConfig();
            if (config == null) return;

            for (int i = 0; i < indexes.Length; i++)
            {
                switch (indx)
                {
                    case 0:
                        AppendStates(cbo, config);
                        break;

                    case 1:
                        AppendCounties(cbo, config); break;
                }
            }
        }

        private static void AppendCounties(HtmlNode cbo, List<StateSearchConfiguration> config)
        {
            Console.WriteLine("Append Counties called with node: {0}. config {1}",
                cbo.GetType().Name,
                config.GetType().Name);
        }

        private static void AppendStates(HtmlNode cbo, List<StateSearchConfiguration> config)
        {
            Console.WriteLine("Append States called with node: {0}. config {1}",
                cbo.GetType().Name,
                config.GetType().Name);
        }

        private static string ConfigurationText()
        {
            var keyname = $"{PageKeyName}-text";
            var keyvalue = memoryCache.Get(keyname);
            if (keyvalue is not string config) return string.Empty;
            return config;
        }

        private static List<StateSearchConfiguration>? GetConfig()
        {
            var txt = ConfigurationText();
            if (string.IsNullOrEmpty(txt)) return null;
            return ObjectExtensions.TryGet<List<StateSearchConfiguration>?>(txt);
        }
    }
}