﻿// ParseCaseInTheMatterMarriage

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

            var lowered = CaseData.ToLower(CCulture.CurrentCulture);
            var firstAnd = lowered.Substring(SearchFor.Length).IndexOf(" and ", comparison);
            if (firstAnd < 0)
            {
                return false;
            }

            return true;
        }

        public virtual ParseCaseDataResponseDto Parse()
        {
            const string and = " and ";
            var response = new ParseCaseDataResponseDto { CaseData = CaseData };
            if (!CanParse())
            {
                return response;
            }

            if (string.IsNullOrEmpty(CaseData))
            {
                return response;
            }

            var fullName = CaseData.ToLower(CCulture.CurrentCulture);
            if (!fullName.StartsWith(SearchFor, comparison))
            {
                return response;
            }

            var findItIndex = fullName.IndexOf(SearchFor, comparison);
            if (findItIndex < 0)
            {
                return response;
            }

            CaseData = CaseData.Replace(" And ", and);
            fullName = CaseData.Substring(SearchFor.Length);
            var splitIndex = fullName.IndexOf(and, comparison);
            if (splitIndex < 0)
            {
                response.Defendant = fullName.Trim();
                response.Plantiff = string.Empty;
                return response;
            }
            fullName = fullName.Substring(0, splitIndex);
            response.Defendant = fullName.Trim();
            fullName = CaseData.Substring(SearchFor.Length).Replace(fullName, "").Trim();
            fullName = GetWords(fullName, and);
            response.Plantiff = fullName;
            return response;
        }

        private string GetWords(string fullName, string and)
        {
            var final = string.Empty;
            var pieces = fullName.Split(' ');
            foreach (var piece in pieces)
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