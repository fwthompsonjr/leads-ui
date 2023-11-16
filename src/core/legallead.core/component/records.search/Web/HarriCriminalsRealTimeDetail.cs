using HtmlAgilityPack;
using legallead.records.search.Classes;
using OpenQA.Selenium;
using System.Diagnostics.CodeAnalysis;

namespace legallead.records.search.Web
{
    [ExcludeFromCodeCoverage]
    public class HarriCriminalsRealTimeDetail : HarrisCriminalRealTime
    {
        public void GetCaseDetails(IWebDriver driver, List<HarrisCriminalDto> details, DateTime runDate, string currentWindow)
        {
            // read case detail
            List<IWebElement> searchable = GetElementsOrFail(By.XPath(searchLinks));
            // read case details
            searchable.ForEach(s =>
            {
                if (searchable.IndexOf(s) != 0)
                {
                    driver.ClickAndOrSetText(s);
                    string? newWindow = driver.WindowHandles.ToList()
                        .Find(h => !h.Equals(currentWindow, StringComparison.OrdinalIgnoreCase));
                    driver.SwitchTo().Window(newWindow);
                    List<HtmlDocument?> cdResults = GetElementsOrFail(By.XPath(resultGrid))
                        .Select(x => ToHtmlDocument(x.GetAttribute("outerHTML")))
                        .ToList()
                        .FindAll(f => f != null);
                    AppendDetail(details, cdResults, runDate);
                    driver.Close();
                    driver.SwitchTo().Window(currentWindow);
                }
            });
        }

        private static void AppendDetail(List<HarrisCriminalDto> details, List<HtmlDocument> cdResults, DateTime runDate)
        {
            if (details == null | cdResults == null)
            {
                return;
            }
            if (cdResults.Count < 3)
            {
                return;
            }
            const string tbl = "//table";
            const string tr = "//tr";
            PopUpTable rows = new()
            {
                Index = details.Count + 1,
                RunDate = runDate,
                CauseSummary = cdResults[0].DocumentNode?.FirstChild?.SelectNodes(tr)?.ToList(),
                DefendantAddress = cdResults[1]?.DocumentNode?.FirstChild?.SelectNodes(tr)?.ToList(),
                DefendantBio = cdResults[1]?.DocumentNode?.FirstChild?.SelectNodes(tbl)[0]?.SelectNodes(tr)?.ToList(),
                DefendantMetrics = cdResults[1]?.DocumentNode?.FirstChild?.SelectNodes(tbl)[1]?.SelectNodes(tr)?.ToList(),
                Court = cdResults[2]?.DocumentNode?.FirstChild?.SelectNodes(tr)?.ToList()
            };
            HarrisCriminalDto dto = rows.CriminalDto();
            if (dto == null)
            {
                return;
            }

            details.Add(dto);
        }

        private static HtmlDocument? ToHtmlDocument(string table)
        {
            try
            {
                HtmlDocument doc = new();
                doc.LoadHtml(table);
                return doc;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}