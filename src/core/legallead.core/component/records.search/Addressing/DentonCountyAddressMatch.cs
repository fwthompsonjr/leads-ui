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
            CaseDetails = GetSummary(web);
        }

        public DentonCaseStyleSummary? CaseDetails { get; private set; }
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

        private static DentonCaseStyleSummary? GetSummary(IWebDriver driver)
        {
            var body = driver.TryFindElement(By.TagName("body"));
            if (body == null) return null;
            var text = body.GetAttribute("innerHTML");
            if (text == null) return null;
            var table = text.GetNode();
            if (table == null) return null;
            var bb = table.SelectNodes("//b").ToList();
            var caseStyle = GetCaseStyle(bb);
            var response = new DentonCaseStyleSummary { CaseStyle = caseStyle };
            var th = table.SelectNodes("//th").ToList().FindAll(x => HasClass(x, "ssTableHeaderLabel"));
            if (th == null || ! th.Any()) return response;
            var searches = new[] { "Case Type:", "Date Filed:", "Location:" };
            th = th.FindAll(x => searches.Contains(x.InnerText));
            th.ForEach(h =>
            {
                var thtxt = h.InnerText.Split(":")[0];
                var tr = h.FindParent("tr")?.ChildNodes[1].InnerText;
                if (thtxt == "Location") { response.Court = tr ?? string.Empty; }
                if (thtxt == "Date Filed") { response.DateFiled = tr ?? string.Empty; }
                if (thtxt == "Case Type") { response.CaseType = tr ?? string.Empty; }
            });
            return response;
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

        private static string GetCaseStyle(List<HtmlNode> rows)
        {
            if (!rows.Any()) return string.Empty;
            var texas = rows.Find(x => x.InnerText.Contains("Texas", StringComparison.OrdinalIgnoreCase));
            if (texas != null) return texas.InnerText;
            return rows[0].InnerText;
        }

        private static bool HasClass(HtmlNode node, string className)
        {
            if (node == null || node.Attributes.Count == 0) return false;
            var attr = node.Attributes.FirstOrDefault(x => x.Name == "class")?.Value;
            if (attr == null || attr.IndexOf(className) < 0) return false;
            return true;
        }

    }
}
