using legallead.records.search.Addressing;
using legallead.records.search.DriverFactory;
using legallead.records.search.Dto;
using legallead.records.search.Models;
using OpenQA.Selenium;
using System.Configuration;
using System.Runtime.InteropServices;

namespace legallead.records.search.Classes
{
    public partial class WebUtilities
    {
        /// <summary>
        /// Populates search parameters into target
        /// Executes search
        /// and reads the results to gets the cases associated.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A list of case information including person information associated to each case</returns>
        public static List<HLinkDataRow> GetCases(WebInteractive data)
        {
            List<ICaseFetch> fetchers = new()
            {
                new NonCriminalCaseFetch(data),
                new CriminalCaseFetch(data)
            };
            List<HLinkDataRow> cases = new();
            fetchers.ForEach(f => cases.AddRange(f.GetLinkedCases()));
            return cases;
        }

        private static IWebElement? GetCaseData(WebInteractive data,
            ref List<HLinkDataRow> cases,
            string navTo, ElementAssertion helper)
        {
            helper.Navigate(navTo);

            WebNavigationKey? parameter = GetParameter(data, CommonKeyIndexes.IsCriminalSearch);
            bool isCriminalSearch = parameter != null &&
                parameter.Value.Equals(CommonKeyIndexes.NumberOne, StringComparison.CurrentCultureIgnoreCase);
            IWebElement? tbResult = helper.Process(data, isCriminalSearch);
            if (tbResult == null)
            {
                return null;
            }
            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> rows = tbResult.FindElements(By.TagName("tr"));
            foreach (IWebElement? rw in rows)
            {
                string html = rw.GetAttribute("outerHTML");
                IWebElement? link = TryFindElement(rw, By.TagName("a"));
                IWebElement? tdCaseStyle = TryFindElement(rw, By.XPath("td[2]"));
                string caseStyle = tdCaseStyle == null ? string.Empty : tdCaseStyle.Text;
                string address = link == null ? string.Empty : link.GetAttribute("href");
                HLinkDataRow dataRow = new() { Data = html, WebAddress = address, IsCriminal = isCriminalSearch };
                if (link != null)
                {
                    dataRow.Case = link.Text.Trim();
                    dataRow.CriminalCaseStyle = caseStyle;
                }
                cases.Add(new HLinkDataRow { Data = html, WebAddress = address });
            }

            return tbResult;
        }

        /// <summary>
        /// Method to inspect the case result record and drill-down
        /// to the case detail and extract the address information from the page
        /// </summary>
        /// <param name="driver">the current browser instance with automation hooks</param>
        /// <param name="webInteractive">the wrapper for the users inbound parameters and any navigation instructions needed to read the website</param>
        /// <param name="linkData">the html of the source case result record</param>
        internal static void Find(IWebDriver driver, HLinkDataRow linkData)
        {
            List<FindDefendantBase> finders = new()
            {
                new FindDefendantNavigation(),
                new FindMultipleDefendantMatch(),
                new FindDefendantByWordMatch(),
                new FindPrincipalByWordMatch(),
                new FindPetitionerByWordMatch(),
                new FindRespondentByWordMatch(),
                new FindDefendantByCondemneeMatch(),
                new FindApplicantByWordMatch(),
                new FindDefendantByGuardianMatch(),
                new NoFoundMatch()
            };
            foreach (FindDefendantBase finder in finders)
            {
                finder.Find(driver, linkData);
                if (finder.CanFind)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the web driver.
        /// </summary>
        /// <returns></returns>
        public static IWebDriver GetWebDriver()
        {
            var dto = new WebDriverDto().Get() ?? new();
            WebDrivers wdriver = dto.WebDrivers;
            Driver? driver = wdriver.Drivers.FirstOrDefault(d => d.Id == wdriver.SelectedIndex);
            StructureMap.Container container = WebDriverContainer.GetContainer;
            IWebDriverProvider provider = container.GetInstance<IWebDriverProvider>(driver?.Name ?? string.Empty);
            return provider.GetWebDriver();
        }

        public static string? GetChromeBinary()
        {
            const string linuxChromLocation = @"/usr/bin/google-chrome";
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            return isLinux ? linuxChromLocation : ChromeBinaryFileName();
        }

        /// <summary>
        /// Tries the find a child element using the By condition supplied.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <param name="by">The by condition used to locate the element</param>
        /// <returns></returns>
        private static IWebElement? TryFindElement(IWebElement parent, By by)
        {
            try
            {
                return parent.FindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string? _chromeBinaryName;

        private static string? ChromeBinaryFileName()
        {
            if (_chromeBinaryName != null)
            {
                return _chromeBinaryName;
            }

            List<string?> settings = ConfigurationManager.AppSettings
                .AllKeys.ToList().FindAll(x => (x ?? "").StartsWith("chrome.exe.location",
                StringComparison.CurrentCultureIgnoreCase))
                .Select(x => ConfigurationManager.AppSettings[x])
                .ToList().FindAll(x => File.Exists(x));
            if (settings.Any())
            {
                _chromeBinaryName = settings[0];
                return _chromeBinaryName;
            }

            DirectoryInfo di = new(@"c:\");
            DirectorySearch search = new(di, "*chrome.exe", 2);
            List<string> found = search.FileList;
            if (found.Any())
            {
                _chromeBinaryName = found[0];
                return _chromeBinaryName;
            }
            search = new DirectorySearch(di, "*chrome.exe");
            found = search.FileList;
            if (found.Any())
            {
                _chromeBinaryName = found[0];
                return _chromeBinaryName;
            }
            _chromeBinaryName = string.Empty;
            return _chromeBinaryName;
        }
    }
}