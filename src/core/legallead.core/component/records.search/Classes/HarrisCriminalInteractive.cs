using legallead.records.search.Models;
using legallead.records.search.Parsing;

namespace legallead.records.search.Classes
{
    public class HarrisCriminalInteractive : TarrantWebInteractive
    {
        private static CultureInfo GetCulture => CultureInfo.InvariantCulture;
        private static StringComparison Oic => StringComparison.OrdinalIgnoreCase;

        public HarrisCriminalInteractive(WebNavigationParameter parameters) : base(parameters)
        {
        }

        public HarrisCriminalInteractive(WebNavigationParameter parameters,
            DateTime startDate, DateTime endingDate) : base(parameters, startDate, endingDate)
        {
        }

        public HarrisCriminalInteractive()
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
            while (startingDate.CompareTo(endingDate) <= 0)
            {
                List<HLinkDataRow> cases = new();
                List<PersonAddress> people = new();
                List<HarrisCriminalDto> detailGroup = new();
                if (CaseStyleDbInspector.HasHeader(startingDate))
                {
                    List<harriscriminal.db.Tables.CaseStyleDb> db = Startup.CaseStyles.DataList;
                    string fileDate = startingDate.ToString("M/d/yyyy", GetCulture);
                    List<harriscriminal.db.Tables.CaseStyleDb> found = db.Where(f => f.FileDate.Equals(fileDate, Oic)).ToList();
                    List<harriscriminal.db.Tables.CaseStyleDb> group = found.GroupBy(x => x.CaseNumber).Select(y => y.First()).ToList();
                    cases.AddRange(group.ToHLinkData());
                }
                List<string> caseNumbers = cases.Select(c => c.Case.Split('-')[0].Trim()).Distinct().ToList();
                if (CaseStyleDbInspector.HasDetail(startingDate))
                {
                    List<HarrisCountyListDto> db = Startup.Downloads.DataList;
                    string fileDate = startingDate.ToString("yyyyMMdd", GetCulture);
                    List<HarrisCriminalDto> details = new();
                    foreach (HarrisCountyListDto dataset in db)
                    {
                        IEnumerable<HarrisCriminalDto> found = dataset.Data.Where(a => a.FilingDate.Equals(fileDate, Oic));
                        details.AddRange(found);
                    }
                    List<HarrisCriminalDto> group = details.GroupBy(x => x.CaseNumber).Select(y => y.First()).ToList();
                    detailGroup = group.FindAll(x => caseNumbers.Contains(x.CaseNumber));
                }

                foreach (HLinkDataRow item in cases)
                {
                    List<HarrisCriminalDto> smallerList = detailGroup.FindAll(x => x.CaseNumber.Trim().Equals(item.Case.Split('-')[0].Trim(), StringComparison.OrdinalIgnoreCase));
                    people.AddRange(item.ToPersonAddress(smallerList));
                }

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
            webFetch.Result = results.FileName;
            return webFetch;
        }
    }
}