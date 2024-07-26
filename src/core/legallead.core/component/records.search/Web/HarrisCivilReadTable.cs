using legallead.records.search.Classes;
using System.Text;

namespace legallead.records.search.Web
{
    using legallead.records.search.Dto;
    using legallead.records.search.Models;
    using OpenQA.Selenium;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Byy = OpenQA.Selenium.By;

    public class HarrisCivilReadTable : ElementActionBase
    {
        protected bool IsOverlaid { get; set; }
        private const string actionName = "harris-civil-read-table";
        private const string rowSelector = "#itemPlaceholderContainer tr.even";
        private const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
        public override string ActionName => actionName;

        public List<HLinkDataRow> DataRows { get; private set; } = new();

        public override void Act(NavigationStep item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            IWebDriver? driver = GetWeb;
            if (driver == null) return;
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            IsOverlaid = false;

            DataRows = new List<HLinkDataRow>();

            ReadPage(driver, executor);

            if (item.Wait > 0) { Thread.Sleep(item.Wait); }
        }

        private void ReadPage(IWebDriver driver, IJavaScriptExecutor executor)
        {
            List<CaseRowData> rowData = new();

            while (true)
            {
                if (HasNoData())
                {
                    CaseRowData empty = new()
                    {
                        FileDate = GetFileDateFromPage(),
                        CaseDataAddresses = new List<CaseDataAddress>()
                    {
                        new CaseDataAddress { Role = "Defendant" , Party = "No Data Returned" }
                    }
                    };
                    rowData.Add(empty);
                    break;
                }
                List<IWebElement> rows = driver.FindElements(Byy.CssSelector(rowSelector)).ToList();
                rows.ForEach(row =>
                {
                    CaseRowData caseData = new();
                    List<IWebElement> cells = row.FindElements(Byy.TagName("td")).ToList();
                    for (int i = 1; i <= 8; i++)
                    {
                        IWebElement cell = cells[i];
                        switch (i)
                        {
                            case 1:
                                IWebElement doclinks = cell.FindElement(Byy.CssSelector("a.doclinks"));
                                executor.ExecuteScript("arguments[0].scrollIntoView(true);", doclinks);
                                caseData.Case = doclinks.Text;
                                break;

                            case 2:
                                caseData.Court = cell.Text.Trim();
                                break;

                            case 3:
                                caseData.FileDate = cell.Text.Trim();
                                break;

                            case 4:
                                caseData.Status = cell.Text.Trim();
                                break;

                            case 5:
                                caseData.TypeDesc = cell.Text.Trim();
                                break;

                            case 6:
                                caseData.Subtype = cell.Text.Trim();
                                break;

                            case 7:
                                caseData.Style = cell.Text.Trim();
                                break;

                            case 8:
                                IWebElement hlink = cell.FindElement(Byy.TagName("a"));
                                string link = hlink.GetAttribute("onclick");
                                int n = link.IndexOf("x-", comparisonType: ccic) + 1;
                                link = link[n..];
                                n = link.IndexOf(".", comparisonType: ccic);
                                link = link[1..n];
                                executor.ExecuteScript("arguments[0].click();", hlink);
                                caseData.CaseDataAddresses = GetAddresses(link);
                                rowData.Add(caseData);
                                executor.ExecuteScript("arguments[0].click();", hlink);
                                break;
                        }
                    }
                });
                if (!HasNextPage())
                {
                    break;
                }

                if (!NavigateNextPage())
                {
                    break;
                }
            }
            OuterHtml = rowData.ToHtml();
            HarrisCivilAddressList.AddRange(rowData);
            rowData.ForEach(d => DataRows.AddRange(d.ConvertToDataRow()));
        }

        private bool NavigateNextPage()
        {
            string cssPagerLink = "a.pgr";
            IWebDriver? driver = GetWeb;
            if (driver == null) return false;
            // if there are paging hyperlinks then their might be a next page
            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> paging = driver.FindElements(Byy.CssSelector(cssPagerLink));
            if (paging == null)
            {
                return false;
            }

            List<IWebElement> list = paging.ToList();
            IWebElement? next = list.Find(a => a.Text.Contains("Next"));
            if (next == null)
            {
                return false;
            }

            if (!(next.GetAttribute("disabled") ?? "false").Equals("true", ccic))
            {
                // click element (executor)
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", next);
                return true;
            }
            return false;
        }

        private string GetFileDateFromPage()
        {
            const string dte = "MM/dd/yyyy";
            string cssSelector = "#ctl00_ContentPlaceHolder1_txtDateFrom";
            string dateMin = DateTime.MinValue.ToString(dte, CultureInfo.CurrentCulture);
            IWebElement? found = GetWeb?.TryFindElement(Byy.CssSelector(cssSelector));
            if (found == null)
            {
                return dateMin;
            }
            if (DateTime.TryParse(found.Text, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out DateTime dtFrom))
            {
                return dtFrom.ToString(dte, CultureInfo.CurrentCulture);
            }
            return dateMin;
        }

        private List<CaseDataAddress> GetAddresses(string search)
        {
            List<CaseDataAddress> data = new();
            IWebDriver? driver = GetWeb;
            IWebElement? div = driver?.FindElement(Byy.CssSelector("div[id*='" + search + "']"));
            List<IWebElement>? elements = div?.FindElements(Byy.CssSelector("table[rules='rows'] tr[align='center']")).ToList();
            if (elements == null) { return data; }
            for (int i = 0; i < elements.Count; i++)
            {
                IWebElement row = elements[i];
                List<IWebElement> tds = row.FindElements(Byy.TagName("td")).ToList();
                CaseDataAddress obj = new()
                {
                    Case = HtmlDecode(tds[0].Text.Trim()),
                    Role = HtmlDecode(tds[1].Text.Trim()),
                    Party = HtmlDecode(tds[2].GetAttribute("innerHTML").Trim()),
                    Attorney = HtmlDecode(tds[3].GetAttribute("innerHTML").Trim())
                };
                data.Add(obj);
            }
            return data;
        }

        private static string HtmlDecode(string input)
        {
            const string pipe = " | ";
            const char space = ' ';
            var nobr = (char)190;
            string cleaned = Regex.Replace(System.Net.WebUtility.HtmlDecode(input), @"\s+", " ").Replace(nobr, space);
            StringBuilder sb = new(cleaned);
            sb.Replace("<br>", pipe);
            sb.Replace("<br/>", pipe);
            sb.Replace("<br />", pipe);
            return sb.ToString();
        }

        protected bool HasNoData()
        {
            // #ctl00_ContentPlaceHolder1_lblListViewCasesEmptyMsg:visible
            string cssNoData = "ctl00_ContentPlaceHolder1_lblListViewCasesEmptyMsg";
            IWebDriver? driver = GetWeb;
            IWebElement? found = driver?.TryFindElement(Byy.Id(cssNoData));
            if (found == null)
            {
                return false;
            }

            return found.Displayed;
        }

        protected bool HasNextPage()
        {
            try
            {
                string cssPagerLink = "a.pgr";
                IWebDriver? driver = GetWeb;
                // if there are paging hyperlinks then their might be a next page
                System.Collections.ObjectModel.ReadOnlyCollection<IWebElement>? paging = driver?.FindElements(Byy.CssSelector(cssPagerLink));
                if (paging == null)
                {
                    return false;
                }

                List<IWebElement> list = paging.ToList();
                IWebElement? next = list.Find(a => a.Text.Contains("Next"));
                if (next == null)
                {
                    return false;
                }

                return (next.GetAttribute("disabled") ?? "false").Equals("true", ccic);
            }
            catch (Exception)
            {
                // element not found or something
                return false;
            }
        }
    }
}