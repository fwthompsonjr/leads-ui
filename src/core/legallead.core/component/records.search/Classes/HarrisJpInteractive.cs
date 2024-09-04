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
            List<PersonAddress> peopleList = new();
            WebFetchResult webFetch = new();
            XmlContentHolder results = new SettingsManager().GetOutput(this);

            // need to open the navigation file(s)
            List<NavigationStep> steps = new();
            string? navigationFile = GetParameterValue<string>(CommonKeyIndexes.NavigationControlFile);
            if (string.IsNullOrEmpty(navigationFile)) return webFetch;
            List<string> sources = navigationFile.Split(',').ToList();
            sources.ForEach(s => steps.AddRange(GetAppSteps(s).Steps));
            webFetch = SearchWeb(results, steps, startingDate, endingDate, peopleList);
            peopleList.ForEach(p =>
            {
                p = p.ToCalculatedNames();
                p.ToCalculatedZip();
            });
            webFetch.PeopleList = peopleList;
            webFetch.CaseList = peopleList.ToHtml();
            return webFetch;
        }

        private static WebFetchResult SearchWeb(XmlContentHolder results, List<NavigationStep> steps, DateTime startingDate, DateTime endingDate, List<PersonAddress> people)
        {
            IWebDriver driver = WebUtilities.GetWebDriver();

            try
            {
                ElementAssertion assertion = new(driver);
                string caseList = string.Empty;
                ElementActions.ForEach(x => x.GetAssertion = assertion);
                ElementActions.ForEach(x => x.GetWeb = driver);
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
                            navigation.Populate(steps);
                            var searchresults = PerformSearching(steps);
                            people.AddRange(searchresults);
                        }
                    });
                });

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
                driver?.Quit();
                driver?.Dispose();
            }
        }

        private static List<PersonAddress> PerformSearching(List<NavigationStep> steps)
        {
            var found = new List<PersonAddress>();
            foreach (NavigationStep item in steps)
            {
                IElementActionBase? action = ElementActions
                    .Find(x =>
                        x.ActionName.Equals(item.ActionName,
                        StringComparison.CurrentCultureIgnoreCase));
                if (action == null) continue;
                action.Act(item);
                if (action is HarrisJpSubmitForm submission)
                {
                    found.AddRange(submission.People);
                }
            }
            return found;
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