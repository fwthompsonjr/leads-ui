using HtmlAgilityPack;
using legallead.records.search.Classes;
using legallead.records.search.Tools;
using OpenQA.Selenium;

namespace legallead.records.search.Addressing
{
    internal class DentonCountyAddressMatch
    {
        private static readonly string[] Keywords = new[] { "plaintiff", "defendant", "principal", "petitioner", "applicant", "claimant", "decedent", "respondent", "condemnee", "guardian" };
        private readonly IWebDriver driver;
        private readonly List<DentonAddressSummary> addresses;
        public DentonCountyAddressMatch(IWebDriver web)
        {
            driver = web;
            addresses = GetAddresses();
        }

        public List<DentonAddressSummary> Addresses => addresses;

        private List<DentonAddressSummary> GetAddresses()
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            const string indicator = "ssTableHeader";
            const string partyline = "Party Information";
            var dvSelect = By.CssSelector("div.ssCaseDetailSectionTitle");
            var div = driver.FindElements(dvSelect)
                .FirstOrDefault(d => d.Text.Trim().Equals(partyline, oic));
            if (div == null) return new();
            var table = div.FindParent("table");
            if (table == null) return new();
            var content = table.GetAttribute("outerHTML");
            var tb = content.GetNode();
            if (tb == null) return new();
            ApplyRowIndex(tb);
            var linklist = tb.SelectNodes("//th")
                    .ToList()
                    .FindAll(x =>
                    {
                        var attr = x.Attributes
                            .FirstOrDefault(x => x.Name == "class")?.Value;
                        if (null == attr || attr.IndexOf(indicator) < 0) return false;
                        var txt = x.InnerText.Trim().ToLower();
                        return Keywords.Contains(txt);
                    });
            var parents = new List<DentonAddressSummary>();
            linklist.ForEach(x =>
            {
                var row = x.FindParent("tr");
                var tbl = x.FindParent("table");
                var child = TryParse(row, tbl);
                if (child != null) { parents.Add(child); }
            });
            return parents;
        }


        private static DentonAddressSummary? TryParse(HtmlNode? row, HtmlNode? tbl)
        {
            if (row == null || tbl == null) return null;
            var rowIndex = row.Attributes["row-index"]?.Value;
            if (!int.TryParse(rowIndex, out var ridx)) return null;
            var rows = tbl.SelectNodes("//tr");
            if (rows == null) return null;
            if (ridx > rows.Count - 1) return null;
            var personhtm = rows[ridx].OuterHtml;
            var addresshtm = rows[ridx + 1].OuterHtml;
            var summary = new DentonAddressSummary { AddressHTML = addresshtm, PersonHTML = personhtm };
            summary.Calculate();
            return summary;
        }

        private static void ApplyRowIndex(HtmlNode table)
        {
            var doc = table.OwnerDocument;
            var rows = table.SelectNodes("//tr").ToList();
            foreach (var row in rows)
            {
                var attribute = doc.CreateAttribute("row-index");
                var id = rows.IndexOf(row);
                attribute.Value = id.ToString();
                row.Attributes.Append(attribute);
            }
        }

    }
}
