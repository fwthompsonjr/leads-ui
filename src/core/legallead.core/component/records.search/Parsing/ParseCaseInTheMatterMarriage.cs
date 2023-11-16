// ParseCaseInTheMatterMarriage

using legallead.records.search.Dto;

namespace legallead.records.search.Parsing
{
    using CCulture = System.Globalization.CultureInfo;

    public class ParseCaseInTheMatterMarriage : ICaseDataParser
    {
        private const System.StringComparison comparison =
            System.StringComparison.CurrentCultureIgnoreCase;

        private const string _searchKeyWord = @"in the matter of the marriage of ";

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

            string lowered = CaseData.ToLower(CCulture.CurrentCulture);
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

            CaseData = CaseData.Replace(" And ", and);
            fullName = CaseData[SearchFor.Length..];
            int splitIndex = fullName.IndexOf(and, comparison);
            if (splitIndex < 0)
            {
                response.Defendant = fullName.Trim();
                response.Plantiff = string.Empty;
                return response;
            }
            fullName = fullName[..splitIndex];
            response.Defendant = fullName.Trim();
            fullName = CaseData[SearchFor.Length..].Replace(fullName, "").Trim();
            fullName = GetWords(fullName, and);
            response.Plantiff = fullName;
            return response;
        }

        private static string GetWords(string fullName, string and)
        {
            string final = string.Empty;
            string[] pieces = fullName.Split(' ');
            foreach (string piece in pieces)
            {
                if (piece.Trim() == and.Trim())
                {
                    continue;
                }

                final = final + piece + " ";
            }
            return final.Trim();
        }
    }
}