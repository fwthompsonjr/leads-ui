// ParseCaseOrderForForeclosure

using legallead.records.search.Dto;

namespace legallead.records.search.Parsing
{
    using CCulture = System.Globalization.CultureInfo;

    public class ParseCaseOrderForForeclosure : ICaseDataParser
    {
        private const System.StringComparison comparison =
            System.StringComparison.CurrentCultureIgnoreCase;

        private const string _searchKeyWord = @"in re: order for foreclosure concerning ";

        public virtual string SearchFor => _searchKeyWord;

        public string CaseData { get; set; }

        public virtual bool CanParse()
        {
            if (string.IsNullOrEmpty(CaseData))
            {
                return false;
            }

            if (!CaseData.ToLower(CCulture.CurrentCulture)
                .Contains(SearchFor))
            {
                return false;
            }

            return true;
        }

        public virtual ParseCaseDataResponseDto Parse()
        {
            const string petitioner = "Petitioner:";

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

            fullName = CaseData[SearchFor.Length..];
            int splitIndex = fullName.IndexOf(petitioner, comparison);
            fullName = fullName[..(splitIndex + petitioner.Length)];
            fullName = CaseData[SearchFor.Length..][fullName.Length..].Trim();
            splitIndex = fullName.LastIndexOf(':');
            if (splitIndex > 0)
            {
                fullName = fullName[..splitIndex].Trim();
                splitIndex = fullName.LastIndexOf(' ');
                if (splitIndex > 0)
                {
                    fullName = fullName[..splitIndex].Trim();
                }
            }
            response.Plantiff = fullName;
            fullName = CaseData[findItIndex..].Trim();
            splitIndex = fullName.LastIndexOf(':');
            response.Defendant = fullName[splitIndex..].Replace(":", "").Trim();
            return response;
        }
    }
}