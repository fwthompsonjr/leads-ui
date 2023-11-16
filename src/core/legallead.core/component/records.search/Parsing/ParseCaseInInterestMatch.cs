// ParseCaseInInterestMatch

using legallead.records.search.Dto;

namespace legallead.records.search.Parsing
{
    using CCulture = System.Globalization.CultureInfo;

    public class ParseCaseInInterestMatch : ICaseDataParser
    {
        private const System.StringComparison comparison =
            System.StringComparison.CurrentCultureIgnoreCase;

        // Name Change of:
        private const string _searchKeyWord = @"in the interest of ";

        public virtual string SearchFor => _searchKeyWord;

        public string CaseData { get; set; }

        public virtual bool CanParse()
        {
            if (string.IsNullOrEmpty(CaseData))
            {
                return false;
            }

            if (!CaseData.ToLower(CCulture.CurrentCulture)
                .StartsWith(SearchFor, comparison))
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

            string fullName = CaseData.ToLower(CCulture.CurrentCulture);
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
            response.Plantiff =
                CaseData[SearchFor.Length..].Trim();
            return response;
        }
    }
}