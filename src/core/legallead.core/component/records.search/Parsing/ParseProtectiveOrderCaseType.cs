// ParseProtectiveOrderCaseType
using legallead.records.search.Dto;

namespace legallead.records.search.Parsing
{
    public class ParseProtectiveOrderCaseType : ICaseDataParser
    {
        private const System.StringComparison comparison = System.StringComparison.CurrentCultureIgnoreCase;

        // Name Change of:
        private const string _searchKeyWord = @"protective order| ";

        public virtual string SearchFor => _searchKeyWord;

        public string CaseData { get; set; }

        public virtual bool CanParse()
        {
            if (string.IsNullOrEmpty(CaseData))
            {
                return false;
            }

            if (!CaseData.ToLower(System.Globalization.CultureInfo.CurrentCulture)
                .StartsWith(SearchFor, comparison))
            {
                return false;
            }

            string lowered = CaseData.ToLower(System.Globalization.CultureInfo.CurrentCulture);
            int firstAnd = lowered[SearchFor.Length..].IndexOf(" and ", comparison);
            if (firstAnd < 0)
            {
                return false;
            }

            return true;
        }

        public virtual ParseCaseDataResponseDto Parse()
        {
            const string and = " and ";
            ParseCaseDataResponseDto response = new() { CaseData = CaseData };
            if (!CanParse())
            {
                return response;
            }

            if (string.IsNullOrEmpty(CaseData))
            {
                return response;
            }

            string fullName = CaseData.ToLower(System.Globalization.CultureInfo.CurrentCulture);
            if (!fullName.StartsWith(SearchFor, comparison))
            {
                return response;
            }

            int findItIndex = fullName.IndexOf(SearchFor, comparison);
            if (findItIndex < 0)
            {
                return response;
            }
            //response.Defendant = CaseData.Substring(findItIndex).Trim();
            fullName = CaseData[SearchFor.Length..].Trim();
            int splitIndex = fullName.IndexOf(and, comparison);
            if (splitIndex < 0)
            {
                response.Plantiff = fullName.Trim();
                return response;
            }
            response.Plantiff = fullName[fullName.IndexOf(and, comparison)..].Replace(and, string.Empty).Trim(); ;
            return response;
        }
    }
}