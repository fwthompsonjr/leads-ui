﻿using legallead.records.search.Addressing;
using legallead.records.search.DriverFactory;
using legallead.records.search.Dto;
using legallead.records.search.Models;
using OpenQA.Selenium;
using System.Configuration;

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
            var fetchers = new List<ICaseFetch>
            {
                new NonCriminalCaseFetch(data),
                new CriminalCaseFetch(data)
            };
            var cases = new List<HLinkDataRow>();
            fetchers.ForEach(f => cases.AddRange(f.GetCases()));
            return cases;
        }

        private static IWebElement GetCaseData(WebInteractive data,
            ref List<HLinkDataRow> cases,
            string navTo, ElementAssertion helper)
        {
            IWebElement tbResult;
            helper.Navigate(navTo);
            // this is where denton county does it's data fetching..... i think
            // todo: allow criminal hyperlink click modification...
            var parameter = GetParameter(data, CommonKeyIndexes.IsCriminalSearch); // "isCriminalSearch");
            var isCriminalSearch = parameter != null &&
                parameter.Value.Equals(CommonKeyIndexes.NumberOne, StringComparison.CurrentCultureIgnoreCase);
            tbResult = helper.Process(data, isCriminalSearch);
            var rows = tbResult.FindElements(By.TagName("tr"));
            foreach (var rw in rows)
            {
                var html = rw.GetAttribute("outerHTML");
                var link = TryFindElement(rw, By.TagName("a"));
                var tdCaseStyle = TryFindElement(rw, By.XPath("td[2]"));
                var caseStyle = tdCaseStyle == null ? string.Empty : tdCaseStyle.Text;
                var address = link == null ? string.Empty : link.GetAttribute("href");
                var dataRow = new HLinkDataRow { Data = html, WebAddress = address, IsCriminal = isCriminalSearch };
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
            var finders = new List<FindDefendantBase>
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
            foreach (var finder in finders)
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
            var wdriver = (new WebDriverDto().Get()).WebDrivers;
            var driver = wdriver.Drivers.Where(d => d.Id == wdriver.SelectedIndex).FirstOrDefault();
            var container = WebDriverContainer.GetContainer;
            var provider = container.GetInstance<IWebDriverProvider>(driver.Name);
            return provider.GetWebDriver();
        }

        public static string GetChromeBinary()
        {
            return ChromeBinaryFileName();
        }

        /// <summary>
        /// Tries the find a child element using the By condition supplied.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <param name="by">The by condition used to locate the element</param>
        /// <returns></returns>
        private static IWebElement TryFindElement(IWebElement parent, By by)
        {
            try
            {
                return parent.FindElement(by);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return null;
            }
        }

        // private const int SW_HIDE = 0;
        // private const int SW_SHOW = 5;

        private static string _chromeBinaryName;

        private static string ChromeBinaryFileName()
        {
            if (_chromeBinaryName != null)
            {
                return _chromeBinaryName;
            }

            var settings = ConfigurationManager.AppSettings
                .AllKeys.ToList().FindAll(x => x.StartsWith("chrome.exe.location",
                StringComparison.CurrentCultureIgnoreCase))
                .Select(x => ConfigurationManager.AppSettings[x])
                .ToList().FindAll(x => File.Exists(x));
            if (settings.Any())
            {
                _chromeBinaryName = settings.First();
                return _chromeBinaryName;
            }

            DirectoryInfo di = new(@"c:\");
            var search = new DirectorySearch(di, "*chrome.exe", 2);
            var found = search.FileList;
            if (found.Any())
            {
                _chromeBinaryName = found.First();
                return _chromeBinaryName;
            }
            search = new DirectorySearch(di, "*chrome.exe");
            found = search.FileList;
            if (found.Any())
            {
                _chromeBinaryName = found.First();
                return _chromeBinaryName;
            }
            _chromeBinaryName = string.Empty;
            return _chromeBinaryName;
        }
    }
}