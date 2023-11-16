using legallead.records.search.Dto;

namespace legallead.records.search.Parsing
{
    public class CaseStyleDbParser
    {
        private const string _searchKeyWord = @" vs.";
        private const StringComparison Oic = StringComparison.OrdinalIgnoreCase;

        private enum DataExtractType
        {
            CaseData,
            Defendant,
            Plantiff
        }

        /// <summary>
        /// Gets the specific keyword used to parse plantiff from defendant.
        /// </summary>
        /// <value>
        /// The search keyword used by this parsing strategy.
        /// </value>
        public virtual string SearchFor => _searchKeyWord;

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string Data { get; set; }

        /// <summary>
        /// Determines whether this instance can parse.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can parse; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanParse()
        {
            if (string.IsNullOrEmpty(Data))
            {
                return false;
            }

            if (!Data.ToLower(CultureInfo.CurrentCulture).Contains(SearchFor))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <returns></returns>
        public virtual ParseCaseStyleDbDto Parse()
        {
            ParseCaseStyleDbDto response = new() { Data = Data };
            if (!CanParse())
            {
                return response;
            }

            if (string.IsNullOrEmpty(Data))
            {
                return response;
            }

            string caseData = ExtractField(DataExtractType.CaseData, Data);
            response.CaseData = caseData;
            response.Defendant = ExtractField(DataExtractType.Defendant, caseData);
            response.Plantiff = ExtractField(DataExtractType.Plantiff, caseData);
            return response;
        }

        private static string ExtractField(DataExtractType extractType, string data)
        {
            string response = string.Empty;
            if (string.IsNullOrEmpty(data))
            {
                return response;
            }

            switch (extractType)
            {
                case DataExtractType.CaseData:
                    int a = data.IndexOf("(", Oic);
                    if (a < 0)
                    {
                        return data.Trim();
                    }

                    response = data[..a].Trim();
                    return response;

                case DataExtractType.Defendant:
                case DataExtractType.Plantiff:
                    int b = data.IndexOf(_searchKeyWord, Oic);
                    if (b < 0)
                    {
                        return data;
                    }

                    if (extractType == DataExtractType.Plantiff)
                    {
                        return data[..b].Trim();
                    }
                    response = data[(b + _searchKeyWord.Length)..].Trim();
                    return response;

                default:
                    return response;
            }
        }
    }
}