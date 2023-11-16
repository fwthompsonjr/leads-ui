using legallead.records.search.Addressing;
using legallead.records.search.Dto;
using legallead.records.search.Interfaces;
using legallead.records.search.Models;
using legallead.records.search.Web;
using OpenQA.Selenium;

namespace legallead.records.search.Classes
{
    public class HarrisCivilInteractive : TarrantWebInteractive
    {
        private class AddressCondition
        {
            public string Name { get; set; }
            public bool IsViolation { get; set; }
        }

        public HarrisCivilInteractive(WebNavigationParameter parameters) : base(parameters)
        {
        }

        public HarrisCivilInteractive(WebNavigationParameter parameters,
            DateTime startDate, DateTime endingDate) : base(parameters, startDate, endingDate)
        {
        }

        public HarrisCivilInteractive()
        {
        }

        public override WebFetchResult Fetch()
        {
            // settings have been retrieved from the constructor
            // get any output file to store data from extract
            DateTime startingDate = GetParameterValue<DateTime>(CommonKeyIndexes.StartDate);
            DateTime endingDate = GetParameterValue<DateTime>(CommonKeyIndexes.EndDate);
            int courtIndex = GetParameterValue<int>("courtIndex");
            int caseStatusIndex = GetParameterValue<int>("caseStatusIndex");
            List<PersonAddress> peopleList = new();
            WebFetchResult webFetch = null;
            while (startingDate.CompareTo(endingDate) <= 0)
            {
                XmlContentHolder results = new SettingsManager().GetOutput(this);

                // need to open the navigation file(s)
                List<NavigationStep> steps = new();
                string navigationFile = GetParameterValue<string>(CommonKeyIndexes.NavigationControlFile);
                List<string> sources = navigationFile.Split(',').ToList();
                List<HLinkDataRow> cases = new();
                List<PersonAddress>? people = new();
                sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));
                CaseTypeSelectionDto caseTypes = CaseTypeSelectionDto.GetDto(CommonKeyIndexes.CollinCountyCaseType);
                int caseTypeId = GetParameterValue<int>(CommonKeyIndexes.CaseTypeSelectedIndex);
                int searchTypeId = GetParameterValue<int>(CommonKeyIndexes.SearchTypeSelectedIndex);
                DropDown selectedCase = caseTypes.DropDowns[caseTypeId];
                // set special item values
                NavigationStep caseTypeSelect = steps.First(x =>
                    x.ActionName.Equals("jquery-set-selected-index", StringComparison.CurrentCultureIgnoreCase) &
                    x.DisplayName.Equals("court-drop-down list", StringComparison.CurrentCultureIgnoreCase));
                caseTypeSelect.ExpectedValue = courtIndex.ToString(CultureInfo.CurrentCulture.NumberFormat);

                NavigationStep caseStatusSelect = steps.First(x =>
                    x.ActionName.Equals("jquery-set-selected-index", StringComparison.CurrentCultureIgnoreCase) &
                    x.DisplayName.Equals("case-status-drop-down list", StringComparison.CurrentCultureIgnoreCase));
                caseStatusSelect.ExpectedValue = caseStatusIndex.ToString(CultureInfo.CurrentCulture.NumberFormat);

                steps.First(x =>
                    x.ActionName.Equals("jquery-set-text", StringComparison.CurrentCultureIgnoreCase) &
                    x.DisplayName.Equals("startDate", StringComparison.CurrentCultureIgnoreCase))
                    .ExpectedValue = startingDate.ToString("MM/dd/yyyy");

                steps.First(x =>
                    x.ActionName.Equals("jquery-set-text", StringComparison.CurrentCultureIgnoreCase) &
                    x.DisplayName.Equals("endDate", StringComparison.CurrentCultureIgnoreCase))
                    .ExpectedValue = startingDate.ToString("MM/dd/yyyy");

                webFetch = SearchWeb(results, steps, startingDate, startingDate,
                    ref cases, out people);
                peopleList.AddRange(people);
                peopleList.ForEach(p =>
                {
                    p = p.ToCalculatedNames();
                    p = p.ToCalculatedZip();
                });
                webFetch.PeopleList = peopleList;
                webFetch.CaseList = peopleList.ToHtml();
                startingDate = startingDate.AddDays(1);
            }
            // webFetch.CaseList = CaseSearchType.
            return webFetch;
        }

        /// <summary>
        /// Extracts the case data.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="cases">The cases.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        protected override List<HLinkDataRow> ExtractCaseData(XmlContentHolder results,
            List<HLinkDataRow> cases,
            string actionName, IElementActionBase action)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            if (cases == null)
            {
                throw new ArgumentNullException(nameof(cases));
            }

            if (actionName == null)
            {
                throw new ArgumentNullException(nameof(actionName));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (!actionName.Equals("harris-civil-read-table", comparison))
            {
                return cases;
            }
            
            HarrisCivilReadTable htmlAction = (HarrisCivilReadTable)action;
            List<HLinkDataRow> data = htmlAction.DataRows;
            if (data != null && data.Any())
            {
                cases.AddRange(data);
            }

            return cases;
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
                    IElementActionBase? action = ElementActions
                        .Find(x =>
                            x.ActionName.Equals(item.ActionName,
                            StringComparison.CurrentCultureIgnoreCase));
                    if (action == null)
                    {
                        continue;
                    }

                    action.Act(item);
                    cases = ExtractCaseData(results, cases, actionName, action);
                    if (string.IsNullOrEmpty(caseList) && !string.IsNullOrEmpty(action.OuterHtml))
                    {
                        // this value needs to be populated with table data
                        caseList = action.OuterHtml;
                    }
                }
                cases.FindAll(c => string.IsNullOrEmpty(c.Address))
                    .ForEach(c => GetAddressInformation(driver, this, c));

                people = ExtractPeople(cases);
                caseList = people.ToHtml();

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
                driver.Quit();
                driver.Dispose();
            }
        }

        protected override List<PersonAddress> ExtractPeople(List<HLinkDataRow> cases)
        {
            if (cases == null)
            {
                return null;
            }

            if (!cases.Any())
            {
                return new List<PersonAddress>();
            }

            List<PersonAddress> list = new();
            foreach (HLinkDataRow item in cases)
            {
                string styleInfo = item.CaseStyle;
                PersonAddress person = new()
                {
                    Name = item.Defendant,
                    CaseNumber = item.Case,
                    DateFiled = item.DateFiled,
                    Court = item.Court,
                    CaseType = item.CaseType,
                    CaseStyle = styleInfo,
                    Status = item.Data
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

            string fmt = GetParameterValue<string>(CommonKeyIndexes.HlinkUri);
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
            IWebElement criminalLink = TryFindElement(driver, By.XPath(CommonKeyIndexes.CriminalLinkXpath));
            IWebElement? elementCaseName = TryFindElement(driver, By.XPath(CommonKeyIndexes.CaseStlyeBoldXpath));
            if (criminalLink != null)
            {
                if (elementCaseName != null)
                {
                    linkData.CriminalCaseStyle = elementCaseName.Text;
                    linkData.IsCriminal = true;
                }
            }
            IWebElement probateLink = TryFindElement(driver, By.XPath(CommonKeyIndexes.ProbateLinkXpath));
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
    }
}