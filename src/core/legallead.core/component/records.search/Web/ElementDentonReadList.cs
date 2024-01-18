using HtmlAgilityPack;
using legallead.records.search.Classes;
using legallead.records.search.Models;
using legallead.records.search.Tools;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System.Diagnostics.CodeAnalysis;

namespace legallead.records.search.Web
{
    public class ElementDentonReadList : ElementNavigationBase
    {
        public DentonTableRead? JsContent { get; set; }
        public string? TableXPath { get; set; }
        public override IWebElement? Execute(WebNavInstruction item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            JsContent = default;
            IWebDriver? driver = PageDriver;
            if (driver == null) { return null; }
            var builder = new GetPageTable(driver);
            if (builder.Table == null) { return null; }
            var jsreader = builder.Table;
            if (jsreader == null ||
                string.IsNullOrWhiteSpace(jsreader.Heading) ||
                string.IsNullOrWhiteSpace(jsreader.Records)) return null;
            var header = GetResponse<DentonTableReadHeader>(jsreader.Heading);
            var records = GetResponse<DentonTableReadRecord[]>(jsreader.Records);
            if (header == null || records == null) return null;
            jsreader.Header = header;
            jsreader.RecordSet = records;
            JsContent = jsreader;
            if(string.IsNullOrEmpty(TableXPath)) { return null; }
            var table = driver.TryFindElement(By.XPath(TableXPath));
            return table;
        }


        [ExcludeFromCodeCoverage(Justification = "Private method unit tested from public accessor.")]
        private static T? GetResponse<T>(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return default;
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }


        [SuppressMessage("Sonar Cube",
            "S2589:Boolean expressions should not be gratuitous",
            Justification = "Conflicting lint suggestion. Suppressing.")]
        [ExcludeFromCodeCoverage(Justification = "Private method unit tested from public accessor.")]
        private class GetPageTable
        {
            private int? _pageTypeIndex;
            private readonly IWebDriver driver;
            private readonly string UriPrefix;
            public GetPageTable(IWebDriver driver)
            {
                string[] supported = new[] { "http", "https" };
                this.driver = driver;
                Table = GetSearchHeading();
                UriPrefix = string.Empty;
                var hasUri = Uri.TryCreate(driver.Url, UriKind.RelativeOrAbsolute, out var result);
                if (hasUri && 
                    result != null &&
                    result.AbsoluteUri.Contains('/') &&
                    supported.Contains(result.Scheme, StringComparer.OrdinalIgnoreCase))
                {
                    var fulladdress = result.AbsoluteUri;
                    var last = fulladdress.Split('/')[^1];
                    var length = fulladdress.Length - (last.Length);
                    UriPrefix = fulladdress[..length];
                }
                
            }

            public DentonTableRead? Table { get; private set; }

            private int GetPageType()
            {
                const int fallback = -1;
                if (_pageTypeIndex != null) return _pageTypeIndex.Value;
                var table = driver.FindElements(By.TagName("table"))[0];
                var content = table.GetAttribute("outerHTML");
                var tb = content.GetNode(); 
                if (tb == null)
                {
                    _pageTypeIndex = fallback;
                    return _pageTypeIndex.Value;
                }
                var body = tb.SelectSingleNode("//tbody");
                if (body == null)
                {
                    _pageTypeIndex = fallback;
                    return _pageTypeIndex.Value;
                }
                var rows = body.SelectNodes("//tr");
                if (rows == null || rows.Count == 0)
                {
                    _pageTypeIndex = fallback;
                    return _pageTypeIndex.Value;
                }
                var cells = rows[0].SelectNodes("//td");
                if (cells == null || cells.Count == 0)
                {
                    _pageTypeIndex = fallback;
                    return _pageTypeIndex.Value;
                }
                var txt = cells[0].InnerText.Trim().ToLower();
                if (txt.Contains("criminal", StringComparison.CurrentCulture)) { _pageTypeIndex = 2; }
                if (_pageTypeIndex == null && txt.Contains("district", StringComparison.CurrentCulture)) { _pageTypeIndex = 1; }
                if (_pageTypeIndex == null && txt.StartsWith("civil, family", StringComparison.CurrentCulture)) { _pageTypeIndex = 0; }
                _pageTypeIndex ??= fallback;
                return _pageTypeIndex.GetValueOrDefault(fallback);
            }

            private DentonTableRead? GetSearchHeading()
            {
                const string parmid = "SearchParamList";
                if (Table != null) return Table;
                var td = driver.TryFindElement(By.Id(parmid));
                if (td == null) return null;
                var response = new DentonTableRead();
                var parent = td.FindParent("table");
                if (parent == null) return response;
                var content = parent.GetAttribute("outerHTML");
                var tb = content.GetNode();
                if (tb == null) return response;
                var rows = tb.SelectNodes("//tr");
                if (rows == null || rows.Count == 0) return response;
                var rc = rows[0].InnerText;
                var search = td.Text.Trim();
                var obj = JsonConvert.SerializeObject(new { searchBy = search, rowCount = rc });
                response.Heading = obj;
                response.Records = GetSearchRecordDetails();
                Table = response;
                return response;
            }

            private string? GetSearchRecordDetails()
            {
                const string casefmt = "//a[contains(@href, '{0}')]";
                const string caseindicator = "CaseDetail.aspx?";
                var pageTypeId = GetPageType();
                var linklist = driver.FindElements(By.TagName("a"))
                    .ToList()
                    .FindAll(x =>
                    {
                        var attr = x.GetAttribute("href");
                        if (null == attr || attr.IndexOf(caseindicator) < 0) return false;
                        return true;
                    });
                if (linklist.Count == 0) return null;
                var parentTable = linklist[0].FindParent("table");
                var html = parentTable?.GetAttribute("outerHTML");
                if (string.IsNullOrEmpty(html)) return null;
                var node = html.GetNode();
                if (node == null) return null;
                var elements = node.SelectNodes(string.Format(casefmt, caseindicator))?.ToList();
                if (elements == null) return null;
                var links = new List<string>();
                elements.ForEach(ele =>
                {
                    var casedata = GetCaseStyle(ele, pageTypeId, UriPrefix);
                    if (casedata != null) links.Add(casedata);
                });
                var rows = string.Join($",{Environment.NewLine}", links);
                return $"[ {rows} ]";
            }

            private static string? GetCaseStyle(HtmlNode lnk, int typeId, string prefix = "")
            {
                int[] itemIndexes = new[] { 0, 1, 2 };
                if (!itemIndexes.Contains(typeId)) return null;
                var tr1 = lnk.FindParent("tr");
                if (tr1 == null) return null;
                var cells = tr1.SelectNodes("//td");
                var divId = typeId == 0 || typeId == 1 ? 2 : 3;
                var divs = cells[divId].SelectNodes("//div");
                var caseNumber = lnk.InnerText.Trim();
                var caseStyle = typeId == 0 || typeId == 1 ?
                    cells[1].InnerText.Trim() :
                    string.Concat(cells[1].InnerText.Trim(), ", ", cells[5].InnerText.Trim());
                var dteFiled = divs[0].InnerText.Trim();
                var court = divs[1].InnerText.Trim();
                var jdo = divs[2].InnerText.Trim();
                var webAddress = string.Concat(prefix, lnk.Attributes["href"].Value);
                var obj = new
                {
                    webAddress,
                    caseNumber,
                    caseStyle,
                    dateFiled = dteFiled,
                    court,
                    officer = jdo
                };
                return JsonConvert.SerializeObject(obj);
            }
        }
    }
}
