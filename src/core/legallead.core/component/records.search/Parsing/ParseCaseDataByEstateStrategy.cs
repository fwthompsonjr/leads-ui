﻿using legallead.records.search.Dto;

namespace legallead.records.search.Parsing
{
    public class ParseCaseDataByEstateStrategy : ICaseDataParser
    {
        private const string _searchKeyWord = @"the estate of";

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
            var response = new ParseCaseDataResponseDto { CaseData = CaseData };
            if (!CanParse())
            {
                return response;
            }

            if (string.IsNullOrEmpty(CaseData))
            {
                return response;
            }

            var fullName = CaseData.ToLower(CultureInfo.CurrentCulture);
            if (!fullName.StartsWith(SearchFor, System.StringComparison.CurrentCultureIgnoreCase))
            {
                return response;
            }

            var findItIndex = fullName.IndexOf(SearchFor, System.StringComparison.CurrentCultureIgnoreCase);
            if (findItIndex < 0)
            {
                return response;
            }

            response.Defendant = CaseData.Substring(findItIndex).Trim();
            response.Plantiff =
                CaseData.Substring(SearchFor.Length).Trim();
            return response;
        }
    }
}