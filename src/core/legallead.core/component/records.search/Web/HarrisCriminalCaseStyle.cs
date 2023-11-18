using HtmlAgilityPack;
using legallead.records.search.Classes;
using legallead.records.search.Db;
using legallead.records.search.Dto;
using legallead.records.search.Parsing;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace legallead.records.search.Web
{
    [ExcludeFromCodeCoverage]
    public class HarrisCriminalCaseStyle : HarrisCriminalData
    {
        /// <summary>
        /// The user identity
        /// </summary>
        private const string uid = "frank.thompson.jr@gmail.com";

        /// <summary>
        /// The user credential
        /// </summary>
        private const string pwd = "123William890";

        /// <summary>
        /// The web address for criminal records search
        /// </summary>
        private const string url = "https://www.hcdistrictclerk.com/eDocs/Public/Search.aspx?Tab=tabCriminal";

        private static class Controls
        {
            private const string ContentPlaceHolder = "ctl00_ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder2_ContentPlaceHolder2";
            private const string Div_SearchResult = ContentPlaceHolder + "_pnlSearchResult";

            /// <summary>
            /// The login button
            /// </summary>
            public const string Btn_Login = "btnLoginImageButton";

            /// <summary>
            /// The search button
            /// </summary>
            public const string Btn_Search = ContentPlaceHolder + "_btnSearch";

            /// <summary>
            /// The search again button
            /// </summary>
            public const string Btn_SearchAgain = ContentPlaceHolder + "_btnSearchAgainTop";

            /// <summary>
            /// The print button
            /// </summary>
            public const string Btn_Print = ContentPlaceHolder + "_lblPrintResult";

            /// <summary>
            /// The textbox case number
            /// </summary>
            public const string Text_CaseNo = ContentPlaceHolder + "_tabSearch_tabCriminal_txtCrimCaseNumber";

            /// <summary>
            /// The textbox case status
            /// </summary>
            public const string Text_CaseStatus = ContentPlaceHolder + "_tabSearch_tabCriminal_ddlCriminalCaseStatus";

            /// <summary>
            /// The textbox start date
            /// </summary>
            public const string Text_StartDate = ContentPlaceHolder + "_tabSearch_tabCriminal_txtCrimStartDate";

            /// <summary>
            /// The textbox end date
            /// </summary>
            public const string Text_EndDate = ContentPlaceHolder + "_tabSearch_tabCriminal_txtCrimEndDate";

            /// <summary>
            /// The table search
            /// </summary>
            public static string Table_Search = @"//*[@id='" + Div_SearchResult + "']/table[1]/tbody/tr[4]/td/table/tbody";

            /// <summary>
            /// The table rows
            /// </summary>
            public static string Table_Rows = Table_Search + "/tr";

            /// <summary>
            /// The table print
            /// </summary>
            public static string Table_Print = @"//table[@id='tblResults']/tbody";
        }

        private static class Selectors
        {
            /// <summary>
            /// Gets the username control.
            /// </summary>
            /// <value>
            /// The name of the user.
            /// </value>
            public static By UserName => By.CssSelector("#txtUserName");

            /// <summary>
            /// Gets the password control.
            /// </summary>
            /// <value>
            /// The password.
            /// </value>
            public static By Password => By.CssSelector("#txtPassword");

            /// <summary>
            /// Gets the login button.
            /// </summary>
            /// <value>
            /// The login.
            /// </value>
            public static By LoginButton => By.Id(Controls.Btn_Login);

            /// <summary>
            /// Gets the case number.
            /// </summary>
            /// <value>
            /// The case number.
            /// </value>
            public static By CaseNumber => By.Id(Controls.Text_CaseNo);

            /// <summary>
            /// Gets the case status.
            /// </summary>
            /// <value>
            /// The case status.
            /// </value>
            public static By CaseStatus => By.Id(Controls.Text_CaseStatus);

            /// <summary>
            /// Gets the start date.
            /// </summary>
            /// <value>
            /// The start date.
            /// </value>
            public static By StartDate => By.Id(Controls.Text_StartDate);

            /// <summary>
            /// Gets the end date.
            /// </summary>
            /// <value>
            /// The end date.
            /// </value>
            public static By EndDate => By.Id(Controls.Text_EndDate);

            /// <summary>
            /// Gets the search button.
            /// </summary>
            /// <value>
            /// The search.
            /// </value>
            public static By Search => By.Id(Controls.Btn_Search);

            /// <summary>
            /// Gets the search again button.
            /// </summary>
            /// <value>
            /// The search.
            /// </value>
            public static By SearchAgain => By.Id(Controls.Btn_SearchAgain);

            /// <summary>
            /// Gets the print button.
            /// </summary>
            /// <value>
            /// The print.
            /// </value>
            public static By Print => By.Id(Controls.Btn_Print);

            /// <summary>
            /// Gets the print table.
            /// </summary>
            /// <value>
            /// The print table.
            /// </value>
            public static By PrintTable => By.XPath(Controls.Table_Print);

            /// <summary>
            /// Gets the table.
            /// </summary>
            /// <value>
            /// The table.
            /// </value>
            public static By Table => By.XPath(Controls.Table_Search);

            /// <summary>
            /// Gets the table rows.
            /// </summary>
            /// <value>
            /// The table rows.
            /// </value>
            public static By TableRows => By.XPath(Controls.Table_Rows);

            /// <summary>
            /// Gets the i frame.
            /// </summary>
            /// <value>
            /// The i frame.
            /// </value>
            public static By IFrame => By.Id("ctl00_ctl00_ctl00_TopLoginIFrame1_iFrameContent2");
        }

        /// <summary>
        /// Gets case style details from remote source for a specific range of dates.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        public List<HarrisCriminalStyleDto> GetCases(IWebDriver driver, DateTime startDate, DateTime endDate)
        {
            List<HarrisCriminalStyleDto> result = new();
            TimeSpan interval = startDate.Subtract(endDate);
            List<HarrisCaseDateDto> list = HarrisCaseDateDto.BuildList(
                startDate,
                interval,
                Convert.ToInt32(Math.Abs(interval.TotalDays)));
            driver = GetOrSetInternalDriver(driver);
            driver.Navigate().GoToUrl(url);
            driver.WaitForNavigation();

            Login(driver);

            if (!driver.IsElementPresent(Selectors.CaseNumber))
            {
                return result;
            }

            foreach (HarrisCaseDateDto dto in list)
            {
                result.Append(PopulateDates(dto));
            }
            return result;
        }

        /// <summary>
        /// Gets case style details from remote source for a specific range of dates.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="totalDays">The total days from the start date to search</param>
        /// <returns></returns>
        public List<HarrisCriminalStyleDto> GetCases(IWebDriver driver, DateTime startDate, int totalDays = 180)
        {
            List<HarrisCriminalStyleDto> result = new();
            TimeSpan interval = new(-7, 0, 0, 0);
            List<HarrisCaseDateDto> list = HarrisCaseDateDto.BuildList(startDate, interval, totalDays);
            driver = GetOrSetInternalDriver(driver);
            driver.Navigate().GoToUrl(url);
            driver.WaitForNavigation();

            Login(driver);

            if (!driver.IsElementPresent(Selectors.CaseNumber))
            {
                return result;
            }

            foreach (HarrisCaseDateDto dto in list)
            {
                result.Append(PopulateDates(dto));
            }
            return result;
        }

        /// <summary>
        /// Gets case style details from remote source for a specific case number.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="searchDto">The search parameters containing case number and date filed.</param>
        /// <returns></returns>
        public List<HarrisCriminalStyleDto> GetData(IWebDriver driver, HarrisCaseSearchDto searchDto)
        {
            List<HarrisCriminalStyleDto> result = new();
            string caseNumber = searchDto?.CaseNumber ?? string.Empty;
            driver = GetOrSetInternalDriver(driver);
            driver.Navigate().GoToUrl(url);
            driver.WaitForNavigation();

            Login(driver);

            if (!driver.IsElementPresent(Selectors.CaseNumber))
            {
                return result;
            }
            IWebElement txCaseNumber = driver.FindElement(Selectors.CaseNumber);
            driver.ClickAndOrSetText(txCaseNumber, caseNumber);
            if (!driver.IsElementPresent(Selectors.Search))
            {
                return result;
            }
            IWebElement btnSearch = driver.FindElement(Selectors.Search);
            driver.ClickAndOrSetText(btnSearch);
            PopulateWhenPresent(Selectors.StartDate, searchDto?.DateFiled, 0);
            PopulateWhenPresent(Selectors.EndDate, searchDto?.DateFiled, 1);
            if (!driver.IsElementPresent(Selectors.Table))
            {
                return result;
            }
            List<IWebElement> rows = driver.FindElements(Selectors.TableRows).ToList();
            foreach (IWebElement? item in rows)
            {
                HarrisCriminalStyleDto? dto = ReadTable(rows, item);
                if (dto != null) { result.Add(dto); }
            }
            return result;
        }

        private IWebDriver GetOrSetInternalDriver(IWebDriver driver)
        {
            if (driver == null)
            {
                TheDriver ??= GetDriver();
                driver = TheDriver;
            }
            else
            {
                TheDriver = driver;
            }

            return driver;
        }

        private void PopulateWhenPresent(By selector, string? dateFiled, int incrementDays = 0)
        {
            if (string.IsNullOrEmpty(dateFiled))
            {
                return;
            }
            CultureInfo culture = CultureInfo.InvariantCulture;
            DateTimeStyles style = DateTimeStyles.AssumeLocal;
            if (!DateTime.TryParseExact(dateFiled, "MM/dd/yyyy", culture, style, out DateTime date))
            {
                return;
            }
            IWebElement? control = TheDriver?.FindElement(selector, 1);
            if (control == null)
            {
                return;
            }
            string dateFmt = date.AddDays(incrementDays).ToString("MM/dd/yyyy", culture);
            TheDriver?.SetText(control, dateFmt);
        }

        private string TryGetCompleteHtml(int recordCount, DateTime stopDateTime)
        {
            if (TheDriver == null) return string.Empty;
            string html = TheDriver.FindElement(Selectors.PrintTable).GetAttribute("outerHTML");
            HtmlDocument doc = new();
            doc.LoadHtml(html);
            HtmlNode parentNode = doc.DocumentNode.FirstChild;
            List<HtmlNode> nodes = parentNode.SelectNodes("tr[@style]").Cast<HtmlNode>().ToList();
            if (nodes.Count == recordCount | stopDateTime < DateTime.Now)
            {
                return html;
            }
            Thread.Sleep(250);
            return string.Empty;
        }

        [ExcludeFromCodeCoverage]
        private static int GetRecordCount(string text)
        {
            const int notfound = -1;
            if (string.IsNullOrEmpty(text))
            {
                return notfound;
            }

            if (!text.Contains(" "))
            {
                return notfound;
            }

            string count = text.Split(' ')[^1].Replace(".", "");
            if (int.TryParse(count, out int rcount))
            {
                return rcount;
            }
            return notfound;
        }

        [ExcludeFromCodeCoverage]
        private List<HarrisCriminalStyleDto> PopulateDates(HarrisCaseDateDto dateDto)
        {
            const string format = "MM/dd/yyyy";
            const string fileFormat = "yyyy-MM-dd";
            const string status = "Active - CRIMINAL";
            string[] controlNames = new string[] { "Case Status", "Print Button", "Print Results Table" };

            var driver = TheDriver;
            if (driver is not IJavaScriptExecutor executor) return new();
            string currentWindow = driver.CurrentWindowHandle;
            CultureInfo culture = CultureInfo.InvariantCulture;
            List<DateTime> dates = new() { dateDto.StartDate, dateDto.EndDate };
            string startDate = dates.Min().ToString(format, culture);
            string endingDate = dates.Max().ToString(format, culture);
            string fsStartDate = dates.Min().ToString(fileFormat, culture);
            string fsEndingDate = dates.Max().ToString(fileFormat, culture);
            List<HarrisCriminalStyleDto> result = new();
            TimeSpan threeMinutes = TimeSpan.FromSeconds(180);
            string strFileName = $"{fsStartDate}_{fsEndingDate}_HarrisCriminalStyleDto.json";
            if (DataPersistence.FileExists(strFileName))
            {
                return DataPersistence.GetContent<List<HarrisCriminalStyleDto>>(strFileName);
            }
            driver.Manage().Timeouts().PageLoad = threeMinutes;

            // populate form
            PopulateWhenPresent(Selectors.StartDate, startDate);
            PopulateWhenPresent(Selectors.EndDate, endingDate);
            if (!IsElementPresent(driver, Selectors.CaseStatus, controlNames[0]))
            {
                throw new ElementNotInteractableException($"{controlNames[0]} Element is not found.");
            }
            IWebElement btnSearch = driver.FindElement(Selectors.Search);
            SelectElement cboCaseStatus = new(driver.FindElement(Selectors.CaseStatus));
            cboCaseStatus.SelectByText(status);

            // submit form ... this occassionally can take longer than 30 seconds
            // so we need to click element through wed-driver and not javascript
            executor.ExecuteScript("arguments[0].scrollIntoView(true);", btnSearch);
            btnSearch.Click();
            driver.WaitForNavigation();
            const string searchRecordCount = "//*[@id=\"ctl00_ctl00_ctl00_ContentPlaceHolder1_ContentPlaceHolder2_ContentPlaceHolder2_recCountCas\"]";
            int recordCount = GetRecordCount(driver.FindElement(By.XPath(searchRecordCount)).Text);
            // print results to new window
            if (!IsElementPresent(driver, Selectors.Print, controlNames[1]))
            {
                throw new ElementNotInteractableException($"{controlNames[1]} Element is not found.");
            }
            IWebElement btnPrint = driver.FindElement(Selectors.Print);
            driver.ClickAndOrSetText(btnPrint);
            string printWindowHandle = FindWindow(driver, currentWindow);
            driver.SwitchTo().Window(printWindowHandle);

            // read results from table
            if (!IsElementPresent(driver, Selectors.PrintTable, controlNames[2]))
            {
                throw new ElementNotInteractableException($"{controlNames[2]} Element is not found.");
            }

            string tableHtml =
                recordCount < 0 ?
                driver.FindElement(Selectors.PrintTable).GetAttribute("outerHTML") :
                string.Empty;

            TimeSpan waitFor = TimeSpan.FromSeconds(30);
            DateTime stopDateTime = DateTime.Now.Add(waitFor);
            while (string.IsNullOrEmpty(tableHtml))
            {
                tableHtml = TryGetCompleteHtml(recordCount, stopDateTime);
            }
            HtmlDocument doc = new();
            doc.LoadHtml(tableHtml);
            HtmlNode parentNode = doc.DocumentNode.FirstChild;
            List<HtmlNode> nodes = parentNode.SelectNodes("tr[@style]").Cast<HtmlNode>().ToList();
            List<List<HtmlNode>> rowdata = nodes.Select(x => x.ChildNodes.Cast<HtmlNode>().ToList())
                .ToList();
            List<IEnumerable<string>> rowinfo = new();
            rowdata.ForEach(r =>
            {
                IEnumerable<string> txts = r.Select(c => c.InnerText);
                rowinfo.Add(txts);
            });

            foreach (IEnumerable<string> item in rowinfo)
            {
                HarrisCriminalStyleDto? dto = ReadTable(rowinfo, item);
                if (dto != null) { result.Add(dto); }
            }

            driver.Close(); // close the pop-up window
            // return to main window
            driver.SwitchTo().Window(currentWindow);

            if (!IsElementPresent(driver, Selectors.SearchAgain))
            {
                throw new ElementNotInteractableException($"Expected Element 'Search Again' is not found.");
            }
            driver.ClickAndOrSetText(driver.FindElement(Selectors.SearchAgain));

            // write result to file here
            if (DataPersistence.FileExists(strFileName))
            {
                return result;
            }

            DataPersistence.Save(strFileName, result);
            return result;
        }

        [ExcludeFromCodeCoverage]
        private static HarrisCriminalStyleDto? Parse(HarrisCriminalStyleDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            dto.Style = Regex.Replace(dto.Style, @"\u00A0", " ");
            dto.Style = Regex.Replace(dto.Style, @"&nbsp;", " ");
            CaseStyleDbParser parser = new() { Data = dto.Style };
            if (!parser.CanParse())
            {
                return dto;
            }

            ParseCaseStyleDbDto parseResult = parser.Parse();
            dto.Style = parseResult.CaseData;
            dto.Plantiff = parseResult.Plantiff;
            dto.Defendant = parseResult.Defendant;
            return dto;
        }

        [ExcludeFromCodeCoverage]
        private static string ParseText(string text, string separator)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(separator))
            {
                return text;
            }
            text = Regex.Replace(text, @"\u00A0", " ");
            if (text.Contains(separator))
            {
                return text.Split(separator.ToCharArray())[0];
            }
            return new string(text.TakeWhile(c => !Char.IsLetter(c)).ToArray());
        }

        [ExcludeFromCodeCoverage]
        private static void Login(IWebDriver driver)
        {
            try
            {
                const int timeout = 5;
                bool bxSearch = IsElementPresent(driver, Selectors.CaseNumber, "Case Number");
                if (bxSearch)
                {
                    return;
                }
                string currentWindow = driver.CurrentWindowHandle;
                IWebElement frame = driver.FindElement(Selectors.IFrame);
                driver.SwitchTo().Frame(frame);
                IWebElement txUserName = driver.FindElement(Selectors.UserName);
                IWebElement txPassword = driver.FindElement(Selectors.Password);
                IWebElement btnLogin = driver.FindElement(Selectors.LoginButton, timeout);

                driver.ClickAndOrSetText(txUserName, uid);
                driver.ClickAndOrSetText(txPassword, pwd);
                driver.ClickAndOrSetText(btnLogin);
                driver.SwitchTo().Window(currentWindow);
                driver.WaitForNavigation();
            }
            catch (Exception)
            {
                // hide these exceptions
            }
        }

        [ExcludeFromCodeCoverage]
        private static HarrisCriminalStyleDto? ReadTable(List<IWebElement> rows, IWebElement item)
        {
            int index = rows.IndexOf(item);
            if (index == 0) { return default; }
            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> cells = item.FindElements(By.TagName("td"));
            if (cells.Count < 6) { return default; } // in the print layout there are separator rows
            return Parse(new HarrisCriminalStyleDto
            {
                Index = index,
                CaseNumber = ParseText(cells[0].Text, Environment.NewLine),
                Style = cells[1].Text,
                FileDate = cells[2].Text,
                Court = cells[3].Text,
                Status = cells[4].Text,
                TypeOfActionOrOffense = cells[5].Text
            });
        }

        [ExcludeFromCodeCoverage]
        private static HarrisCriminalStyleDto? ReadTable(List<IEnumerable<string>> rows, IEnumerable<string> item)
        {
            int index = rows.IndexOf(item);
            List<string> data = item.ToList();
            return Parse(new HarrisCriminalStyleDto
            {
                Index = index,
                CaseNumber = ParseText(data[1], Environment.NewLine).Trim(),
                Style = data[3].Trim(),
                FileDate = data[5].Trim(),
                Court = data[7].Trim(),
                Status = data[9].Trim(),
                TypeOfActionOrOffense = data[11].Trim()
            });
        }

        [ExcludeFromCodeCoverage]
        private static string FindWindow(IWebDriver driver, string topWindowHandle)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            List<string> handles = driver.WindowHandles.ToList();
            return handles.Find(a => !a.Equals(topWindowHandle, oic)) ?? string.Empty;
        }

        [ExcludeFromCodeCoverage]
        private static bool IsElementPresent(IWebDriver driver, By by, string friendlyName = "Element")
        {
            try
            {
                ElementAssertion assertion = new(driver);
                assertion.WaitForElementExist(by, friendlyName);
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}