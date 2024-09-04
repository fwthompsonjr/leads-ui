using legallead.records.search.Dto;
using legallead.records.search.Interfaces;
using legallead.records.search.Models;
using legallead.records.search.Web;
using OpenQA.Selenium;
using System.Diagnostics;

namespace legallead.records.search.Classes
{
    public class HarrisJpInteractive : TarrantWebInteractive
    {
        public HarrisJpInteractive(WebNavigationParameter parameters) : base(parameters)
        {
        }

        public HarrisJpInteractive(WebNavigationParameter parameters,
            DateTime startDate, DateTime endingDate) : base(parameters, startDate, endingDate)
        {
        }

        public HarrisJpInteractive()
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
            WebFetchResult webFetch = new();
            while (startingDate.CompareTo(endingDate) <= 0)
            {
                XmlContentHolder results = new SettingsManager().GetOutput(this);

                // need to open the navigation file(s)
                List<NavigationStep> steps = new();
                string? navigationFile = GetParameterValue<string>(CommonKeyIndexes.NavigationControlFile);
                if (string.IsNullOrEmpty(navigationFile)) break;
                List<string> sources = navigationFile.Split(',').ToList();
                List<HLinkDataRow> cases = new();
                sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));

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

                webFetch = SearchWeb(results, steps, startingDate, endingDate, ref cases, out var people);
                peopleList.AddRange(people);
                peopleList.ForEach(p =>
                {
                    p = p.ToCalculatedNames();
                    p.ToCalculatedZip();
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
                // iterate extract type
                var theCases = new List<HLinkDataRow>();
                var theCaseList = string.Empty;
                extractTypes.ForEach(searchtype =>
                {
                    var courtIndexes = extractCourtIndexes[searchtype].Split(',').ToList();
                    courtIndexes.ForEach(indx =>
                    {

                        var navigation = new JpNavigationParameters
                        {
                            ExtractType = searchtype,
                            ExtractToRequest = extractRequestIndexes[searchtype],
                            CourtIndex = indx,
                            CaseTypeIndex = extractCaseType[searchtype],
                            EndingDate = endingDate.ToString("MM/dd/yyyy"),
                            StartingDate = startingDate.ToString("MM/dd/yyyy")
                        };
                        if (navigation.IsValid())
                        {
                            /* 
                             * poulate the following step values
                             * 1. extract type
                             * 2. extract to request
                             * 3. court
                             * 4. case type
                             * 5. data format
                             * 6. from-date
                             * 7. to-date */
                            navigation.Populate(steps);
                            PerformSearching(results, steps, theCases, theCaseList);
                        }
                    });
                });

                people = ExtractPeople(cases);
                var items = people.GroupBy(
                    x => x.CaseNumber,
                    (nbr, persons) => { return new { nbr, Persons = persons.ToList() }; }).ToList();
                items.ForEach(pp =>
                {
                    var collection = pp.Persons;
                    _ = collection.Select((prsn, id) => { HarrisCivilAddressList.Map(prsn, id); return id; });
                });
                people = items.SelectMany(s => s.Persons).ToList();
                caseList = people.ToHtml();

                return new WebFetchResult
                {
                    Result = results.FileName,
                    CaseList = caseList,
                    PeopleList = people
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                driver?.Quit();
                driver?.Dispose();
                throw;
            }
            finally
            {
                HarrisCivilAddressList.Clear();
                driver?.Quit();
                driver?.Dispose();
            }
        }

        private void PerformSearching(XmlContentHolder results, List<NavigationStep> steps, List<HLinkDataRow> cases, string caseList)
        {
            foreach (NavigationStep item in steps)
            {
                string actionName = item.ActionName;
                IElementActionBase? action = ElementActions
                    .Find(x =>
                        x.ActionName.Equals(item.ActionName,
                        StringComparison.CurrentCultureIgnoreCase));
                if (action == null) continue;
                action.Act(item);
                cases = ExtractCaseData(results, cases, actionName, action);
                if (string.IsNullOrEmpty(caseList) && !string.IsNullOrEmpty(action.OuterHtml))
                {
                    // this value needs to be populated with table data
                    caseList = action.OuterHtml;
                }
            }
        }

        protected override List<PersonAddress> ExtractPeople(List<HLinkDataRow> cases)
        {
            if (cases == null || !cases.Any())
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
                var tmpAddress = CleanUpAddress(item.Address);
                item.Address = tmpAddress;
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
            List<string> indicators = new() { "DOB: ", "Retained", "Pro Se" };
            foreach (var indice in indicators) { if (uncleanAddress.Contains(indice)) return string.Empty; }
            return uncleanAddress;
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

        private static readonly List<string> extractTypes = new() { "criminal", "civil" };
        private static readonly Dictionary<string, string> extractRequestIndexes = new()
        {
            { "civil", "7" },
            { "criminal", "1" }
        };
        private static readonly Dictionary<string, string> extractCourtIndexes = new()
        {
            { "civil", "305,310,315,320,325,330,335,340,345,350,355,360,365,370,375,380" },
            { "criminal", "305,310,315,320,325,330,335,340,345,350,355,360,365,370,375,380" }
        };

        private static readonly Dictionary<string, string> extractCaseType = new()
        {
            { "civil", "ADMIN,BRSB,BOND,DD,CTA,DEBT,DCD,DLS,EV,FJ,HGL,ODL,OEPR,RR,SC,SSP,TAX,TOW,TURN,WRITG,WR,WRU,SEQ,WRJ" },
            { "criminal", "CRCIT,CRCOM" }
        };
        private sealed class JpNavigationParameters
        {
            private const string DataFormat = "xml";
            public string ExtractType { get; set; } = string.Empty;
            public string ExtractToRequest { get; set; } = string.Empty;
            public string CourtIndex { get; set; } = string.Empty;
            public string CaseTypeIndex { get; set; } = string.Empty;
            public string StartingDate { get; set; } = string.Empty;
            public string EndingDate { get; set; } = string.Empty;
            public bool IsValid()
            {
                if(string.IsNullOrEmpty(ExtractType)) return false;
                if (string.IsNullOrEmpty(ExtractToRequest)) return false;
                if (string.IsNullOrEmpty(CourtIndex)) return false;
                if (string.IsNullOrEmpty(CaseTypeIndex)) return false;
                if (string.IsNullOrEmpty(StartingDate)) return false;
                if (string.IsNullOrEmpty(EndingDate)) return false;
                return true;
            }
            public void Populate(List<NavigationStep> steps)
            {
                var radioButtn = steps.Find(x => x.ActionName == "js-check-radio-button") ?? new();
                var cboRequest = steps.Find(x => x.DisplayName == "set extract to request") ?? new();
                var cboCourt = steps.Find(x => x.DisplayName == "set court") ?? new();
                var cboCase = steps.Find(x => x.DisplayName == "set case type") ?? new();
                var cboFormat = steps.Find(x => x.DisplayName == "set data format") ?? new();
                var tbxStart = steps.Find(x => x.DisplayName == "startDate") ?? new();
                var tbxEnding = steps.Find(x => x.DisplayName == "endDate") ?? new();

                radioButtn.Locator.Query = $"#{ExtractType}";
                cboRequest.ExpectedValue = ExtractToRequest;
                cboCourt.ExpectedValue = CourtIndex;
                cboCase.ExpectedValue = CaseTypeIndex;
                cboFormat.ExpectedValue = DataFormat;
                tbxStart.ExpectedValue = StartingDate;
                tbxEnding.ExpectedValue = EndingDate;

            }
        }
    }
}