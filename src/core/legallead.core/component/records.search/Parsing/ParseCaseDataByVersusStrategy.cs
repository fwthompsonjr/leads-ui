using legallead.records.search.Dto;

namespace legallead.records.search.Parsing
{
    public class ParseCaseDataByVersusStrategy : ICaseDataParser
    {
        private const string _searchKeyWord = @"vs.";

        public virtual string SearchFor => _searchKeyWord;

        public string CaseData { get; set; }

        public virtual bool CanParse()
        {
            if (string.IsNullOrEmpty(CaseData))
            {
                return false;
            }

            if (!CaseData.ToLower(CultureInfo.CurrentCulture).Contains(SearchFor))
            {
                return false;
            }

            return true;
        }

        public virtual ParseCaseDataResponseDto Parse()
        {
            ParseCaseDataResponseDto response = new() { CaseData = CaseData };
            if (!CanParse())
            {
                return response;
            }

            if (string.IsNullOrEmpty(CaseData))
            {
                return response;
            }

            string fullName = CaseData.ToLower(CultureInfo.CurrentCulture);
            if (!fullName.Contains(SearchFor))
            {
                return response;
            }

            int findItIndex = fullName.IndexOf(SearchFor, System.StringComparison.CurrentCultureIgnoreCase);
            if (findItIndex < 0)
            {
                return response;
            }

            response.Defendant = CaseData[findItIndex..].Trim();
            response.Plantiff = CaseData[..findItIndex].Trim();
            return response;
        }
    }
}