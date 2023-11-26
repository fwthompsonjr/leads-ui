// ParseCaseDataByInGuardianshipStrategy
using legallead.records.search.Dto;

namespace legallead.records.search.Parsing
{
    public class ParseCaseDataByInGuardianshipStrategy : ICaseDataParser
    {
        private const string _searchKeyWord = @"in the guardianship of";

        public virtual string SearchFor => _searchKeyWord;

        public string CaseData { get; set; } = string.Empty;

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
            if (!fullName.StartsWith(SearchFor, System.StringComparison.CurrentCultureIgnoreCase))
            {
                return response;
            }

            int findItIndex = fullName.IndexOf(SearchFor, System.StringComparison.CurrentCultureIgnoreCase);
            if (findItIndex < 0)
            {
                return response;
            }

            response.Defendant = CaseData[findItIndex..].Trim();
            response.Plantiff =
                CaseData[SearchFor.Length..].Trim();
            return response;
        }
    }
}