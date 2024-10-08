﻿using legallead.records.search.Addressing;
using legallead.records.search.Dto;
using legallead.records.search.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace legallead.records.search.Classes
{
    public class CollinWebInteractive : TarrantWebInteractive
    {
        public CollinWebInteractive(WebNavigationParameter parameters) : base(parameters)
        {
        }

        public CollinWebInteractive(WebNavigationParameter parameters,
            DateTime startDate, DateTime endingDate) : base(parameters, startDate, endingDate)
        {
        }

        public CollinWebInteractive()
        {
        }

        public override WebFetchResult Fetch()
        {
            // settings have been retrieved from the constructor
            // get any output file to store data from extract
            DateTime startingDate = GetParameterValue<DateTime>(CommonKeyIndexes.StartDate);
            DateTime endingDate = GetParameterValue<DateTime>(CommonKeyIndexes.EndDate);
            List<PersonAddress> peopleList = new();
            WebFetchResult? webFetch = null;
            DateTimeFormatInfo formatDate = CultureInfo.CurrentCulture.DateTimeFormat;
            if (!string.IsNullOrEmpty(Result))
            {
                _ = Persistence?.Add(UniqueId, "data-output-file-name", Result);
            }
            while (startingDate.CompareTo(endingDate) <= 0)
            {
                XmlContentHolder results = new SettingsManager().GetOutput(this);

                var dte = startingDate.ToString(CommonKeyIndexes.DateTimeShort, formatDate);
                _ = Persistence?.Add(UniqueId, "data-fetch-date", dte);
                // need to open the navigation file(s)
                List<NavigationStep> steps = new();
                string navigationFile = GetParameterValue<string>(CommonKeyIndexes.NavigationControlFile) ?? string.Empty;
                List<string> sources = navigationFile.Split(',').ToList();
                List<HLinkDataRow> cases = new();
                sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));
                CaseTypeSelectionDto caseTypes = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.CollinCountyCaseType);
                int caseTypeId = GetParameterValue<int>(CommonKeyIndexes.CaseTypeSelectedIndex);
                int searchTypeId = GetParameterValue<int>(CommonKeyIndexes.SearchTypeSelectedIndex);
                DropDown selectedCase = caseTypes.DropDowns[caseTypeId];
                // set special item values
                NavigationStep caseTypeSelect = steps.First(x =>
                    x.ActionName.Equals(CommonKeyIndexes.SetSelectValue, // "set-select-value",
                        StringComparison.CurrentCultureIgnoreCase));
                caseTypeSelect.ExpectedValue = caseTypeId.ToString(CultureInfo.CurrentCulture.NumberFormat);
                NavigationStep searchSelect = steps.First(x =>
                    x.DisplayName.Equals(CommonKeyIndexes.SearchTypeHyperlink, // "search-type-hyperlink",
                        StringComparison.CurrentCultureIgnoreCase));
                searchSelect.Locator.Query = selectedCase.Options[searchTypeId].Query;
                webFetch = SearchWeb(results, steps, startingDate, startingDate,
                    ref cases, out List<PersonAddress>? people);
                people.ForEach(p =>
                {
                    if (p.Zip == "00000" || string.IsNullOrEmpty(p.Zip))
                    {
                        var match = CollinAddressList.List.Find(x => x.CaseNumber == p.CaseNumber);
                        if (match != null)
                        {
                            p.Address1 = match.Address1;
                            p.Address2 = match.Address2;
                            p.Address3 = match.Address3;
                            p.Zip = match.Zip;
                        }
                    }
                });
                CollinAddressList.List.Clear();
                peopleList.AddRange(people);
                webFetch.PeopleList = peopleList;
                var addressobject = JsonConvert.SerializeObject(peopleList);
                _ = Persistence?.Add(UniqueId, "data-output-person-address", addressobject);
                _ = Persistence?.Add(UniqueId, "data-record-count", peopleList.Count.ToString());
                startingDate = startingDate.AddDays(1);
            }
            return webFetch ?? new();
        }

        private WebFetchResult SearchWeb(XmlContentHolder results, List<NavigationStep> steps, DateTime startingDate, DateTime endingDate, ref List<HLinkDataRow> cases, out List<PersonAddress> people)
        {
            IWebDriver driver = WebUtilities.GetWebDriver();

            try
            {
                ElementAssertion assertion = new(driver);
                string caseList = string.Empty;
                ElementActions.ForEach(x => x.GetAssertion = assertion);
                ElementActions.ForEach(x => x.GetWeb = driver);
                DateTimeFormatInfo formatDate = CultureInfo.CurrentCulture.DateTimeFormat;
                AssignStartAndEndDate(startingDate, endingDate, formatDate, steps);
                foreach (NavigationStep item in steps)
                {
                    string actionName = item.ActionName;
                    Interfaces.IElementActionBase? action = ElementActions
                        .Find(x =>
                            x.ActionName.Equals(item.ActionName,
                            StringComparison.CurrentCultureIgnoreCase));
                    if (action == null)
                    {
                        continue;
                    }
                    if (item.DisplayName == "case-type-search")
                    {
                        WaitSequence(1, driver);
                    }
                    action.Act(item);
                    if (item.DisplayName == "search-type-hyperlink" || item.DisplayName == "case-type-search")
                    {
                        WaitSequence(4, driver);
                    }
                    cases = ExtractCaseData(results, cases, actionName, action);
                    if (string.IsNullOrEmpty(caseList) && !string.IsNullOrEmpty(action.OuterHtml))
                    {
                        caseList = action.OuterHtml;
                    }
                }
                cases.FindAll(c => string.IsNullOrEmpty(c.Address))
                    .ForEach(c => GetAddressInformation(driver, this, c));

                cases.FindAll(c => c.IsCriminal && !string.IsNullOrEmpty(c.CriminalCaseStyle))
                    .ForEach(d => d.CaseStyle = d.CriminalCaseStyle);

                cases.FindAll(c => c.IsJustice && !string.IsNullOrEmpty(c.CriminalCaseStyle))
                    .ForEach(d => d.CaseStyle = d.CriminalCaseStyle);

                people = ExtractPeople(cases);

                return new WebFetchResult
                {
                    Result = results.FileName,
                    CaseList = caseList,
                    PeopleList = people
                };
            }
            catch (Exception)
            {
                driver.Quit();
                driver.Dispose();
                throw;
            }
            finally
            {
                driver?.Quit();
                driver?.Dispose();
            }
        }

        protected override List<PersonAddress> ExtractPeople(List<HLinkDataRow> cases)
        {
            if (cases == null || !cases.Any())
            {
                return new();
            }

            List<PersonAddress> list = new();
            foreach (HLinkDataRow item in cases)
            {
                string styleInfo = item.IsCriminal | item.IsJustice ? item.CriminalCaseStyle : GetCaseStyle(item);
                if (item.IsProbate)
                {
                    styleInfo = item.CaseStyle;
                }

                PersonAddress person = new()
                {
                    Name = item.Defendant,
                    CaseNumber = item.Case,
                    DateFiled = item.DateFiled,
                    Court = item.Court,
                    CaseType = item.CaseType,
                    CaseStyle = styleInfo
                };
                item.Address = CleanUpAddress(item.Address);
                person = ParseAddress(item.Address, person);
                if (string.IsNullOrEmpty(person.CaseStyle))
                {
                    string mismatched = $"Case Style Data is empty for {person.CaseNumber}";
                    Console.WriteLine(mismatched);
                }
                list.Add(person);
            }
            return list;
        }

        private static string CleanUpAddress(string uncleanAddress)
        {
            const string driverLicense = @"DL: ";
            const string secondLicense = @"SID: ";
            const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
            if (string.IsNullOrEmpty(uncleanAddress))
            {
                return uncleanAddress;
            }
            if (uncleanAddress.Contains(driverLicense))
            {
                int dlstart = uncleanAddress.IndexOf(driverLicense, comparison);
                uncleanAddress = uncleanAddress[..dlstart].Replace(driverLicense, "");
            }
            if (uncleanAddress.Contains(secondLicense))
            {
                int dlstart = uncleanAddress.IndexOf(secondLicense, comparison);
                uncleanAddress = uncleanAddress[..dlstart].Replace(secondLicense, "");
            }
            if (uncleanAddress.IndexOf("DOB: ", comparison) != 0)
            {
                uncleanAddress = string.Empty;
            }
            if (uncleanAddress.IndexOf("Retained", comparison) != 0)
            {
                uncleanAddress = string.Empty;
            }
            if (uncleanAddress.Equals("Pro Se", comparison))
            {
                uncleanAddress = string.Empty;
            }
            return uncleanAddress;
        }

        /// <summary>
        /// Gets the address information.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="jsonWebInteractive">The json web interactive.</param>
        /// <param name="linkData">The link data.</param>
        private void GetAddressInformation(IWebDriver driver, TarrantWebInteractive jsonWebInteractive, HLinkDataRow linkData)
        {
            if (jsonWebInteractive == null)
            {
                return;
            }

            string fmt = GetParameterValue<string>(CommonKeyIndexes.HlinkUri) ?? string.Empty;
            ElementAssertion helper = new(driver);
            helper.Navigate(string.Format(CultureInfo.CurrentCulture, fmt, linkData.WebAddress));
            driver.WaitForNavigation();
            // we have weird situation where the defendant is sometimes PIr11, PIr12
            // heres where we will get the case style for criminal cases

            FindDefendant(driver, ref linkData);

            // can we get the case-style data here

            driver.Navigate().Back();
        }

        private static void FindDefendant(IWebDriver driver, ref HLinkDataRow linkData)
        {
            IWebElement? criminalLink = TryFindElement(driver, By.XPath(CommonKeyIndexes.CriminalLinkXpath));
            IWebElement? elementCaseName = TryFindElement(driver, By.XPath(CommonKeyIndexes.CaseStlyeBoldXpath));
            if (criminalLink != null && elementCaseName != null)
            {
                linkData.CriminalCaseStyle = elementCaseName.Text;
                linkData.IsCriminal = true;
            }
            IWebElement? probateLink = TryFindElement(driver, By.XPath(CommonKeyIndexes.ProbateLinkXpath));
            if (probateLink != null)
            {
                linkData.IsProbate = true;
            }
            if (linkData.IsJustice && elementCaseName != null)
            {
                linkData.CriminalCaseStyle = elementCaseName.Text;
            }
            List<FindDefendantBase> finders = new()
            {
                new FindCollinAddressByJscript(),
                new FindMultipleDefendantMatch(),
                new FindDefendantByWordMatch(),
                new FindPrincipalByWordMatch(),
                new FindPetitionerByWordMatch(),
                new FindApplicantByWordMatch(),
                new FindDefendantByCondemneeMatch(),
                new FindDefendantByGuardianMatch(),
                new FindOwnerByWordMatch()
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

        private static void AssignStartAndEndDate(DateTime startingDate, DateTime endingDate, DateTimeFormatInfo formatDate, List<NavigationStep> items)
        {
            if (items == null)
            {
                return;
            }

            if (!items.Any())
            {
                return;
            }

            items.ForEach(item =>
            {
                AssignStartAndEndDate(
                    startingDate,
                    endingDate,
                    formatDate,
                    item);
            });
        }

        private static void AssignStartAndEndDate(DateTime startingDate, DateTime endingDate, DateTimeFormatInfo formatDate, NavigationStep item)
        {
            if (!item.ActionName.Equals(CommonKeyIndexes.SetText,
                                    StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }
            if (item.DisplayName.Equals(CommonKeyIndexes.StartDate,
                StringComparison.CurrentCultureIgnoreCase))
            {
                item.ExpectedValue =
                    startingDate.Date.ToString(CommonKeyIndexes.DateTimeShort, formatDate);
            }
            if (item.DisplayName.Equals(CommonKeyIndexes.EndDate,
                StringComparison.CurrentCultureIgnoreCase))
            {
                item.ExpectedValue =
                    endingDate.Date.ToString(CommonKeyIndexes.DateTimeShort, formatDate);
            }
        }
        private static void WaitSequence(int delay, IWebDriver driver)
        {
            for (int i = 0; i < delay; i++)
            {
                Thread.Sleep(450);
                driver.WaitForNavigation();
                Proceed(driver);
            }
        }

        private static void Proceed(IWebDriver driver)
        {
            var by = By.CssSelector("#proceed-button");
            var element = driver.TryFindElement(by);
            if (element == null) { return; }
            var alert = GetAlert(driver);
            alert?.Accept();
            var command = "document.getElementById('proceed-button').click()";
            var jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript(command);
        }

        private static IAlert? GetAlert(IWebDriver driver)
        {
            try
            {
                return driver.SwitchTo().Alert();
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}