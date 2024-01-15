using legallead.records.search.Models;
using OpenQA.Selenium;

namespace legallead.records.search.Classes
{
    public static partial class WebUtilities
    {
        protected interface ICaseFetch
        {
            List<HLinkDataRow> GetLinkedCases();
        }

        protected class NonCriminalCaseFetch : ICaseFetch
        {
            protected WebInteractive Data { get; }

            public NonCriminalCaseFetch(WebInteractive data)
            {
                Data = data;
            }

            public virtual List<HLinkDataRow> GetLinkedCases()
            {
                if (DataRows == null)
                {
                    return new List<HLinkDataRow>();
                }
                WebNavigationKey? parameter = GetParameter(Data, CommonKeyIndexes.IsCriminalSearch);
                if (parameter != null)
                {
                    parameter.Value = CommonKeyIndexes.NumberZero;
                }
                List<HLinkDataRow> cases = Search(GetNavigationAddress(), DataRows);
                return cases;
            }

            protected List<HLinkDataRow> Search(string? navTo, List<HLinkDataRow> cases)
            {
                if (string.IsNullOrEmpty(navTo)) { return cases; }
                using (IWebDriver driver = GetWebDriver())
                {
                    try
                    {
                        cases ??= new List<HLinkDataRow>();

                        WebInteractive data = Data;
                        ElementAssertion helper = new(driver);
                        _ = GetCaseData(data, ref cases, navTo, helper);
                        GetPersonData(cases, driver, data);
                    }
                    catch
                    {
                        driver.Quit();
                        throw;
                    }
                    finally
                    {
                        driver.Close();
                        driver.Quit();
                    }
                }

                return cases;
            }

            protected virtual void GetPersonData(List<HLinkDataRow> cases, IWebDriver driver, WebInteractive data)
            {
                if (cases == null)
                {
                    throw new ArgumentNullException(nameof(cases));
                }

                if (driver == null)
                {
                    throw new ArgumentNullException(nameof(driver));
                }

                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data));
                }

                List<HLinkDataRow> people = cases.FindAll(x => !string.IsNullOrEmpty(x.WebAddress));
                people.ForEach(d => Find(driver, d));
            }

            protected List<HLinkDataRow>? DataRows
            {
                get
                {
                    if (_dataRows != null)
                    {
                        return _dataRows;
                    }

                    var navTo = GetNavigationAddress();
                    if (string.IsNullOrEmpty(navTo))
                    {
                        return _dataRows;
                    }

                    _dataRows = new List<HLinkDataRow>();
                    return _dataRows;
                }
            }

            protected bool IncludeCriminalRecords()
            {
                var criminalCase = GetParameter(Data, CommonKeyIndexes.CriminalCaseInclusion);
                if (criminalCase == null)
                {
                    return false;
                }

                if (!int.TryParse(criminalCase.Value, out int index))
                {
                    return false;
                }

                return (index == 1);
            }

            protected void ModifyInstructions(string keyName)
            {
                string searchLink = CommonKeyIndexes.SearchHyperlink;
                const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
                var key = GetParameter(Data, keyName);
                if (key == null)
                {
                    return;
                }

                List<WebNavInstruction> searchLinks =
                    Data.Parameters.Instructions
                    .FindAll(a => a.FriendlyName.Equals(searchLink, ccic));
                searchLinks.ForEach(s => s.Value = key.Value);
            }

            private WebNavigationKey GetBaseUri()
            {
                return GetParameter(Data, CommonKeyIndexes.BaseUri) ?? new();
            }

            private WebNavigationKey GetQuery()
            {
                return GetParameter(Data, CommonKeyIndexes.Query) ?? new();
            }

            protected string? GetNavigationAddress()
            {
                WebNavigationKey? target = GetBaseUri();
                WebNavigationKey? query = GetQuery();
                if (target == null || query == null)
                {
                    return null;
                }
                if (string.IsNullOrEmpty(target.Value) && string.IsNullOrEmpty(query.Value))
                {
                    return string.Empty;
                }
                if (string.IsNullOrEmpty(query.Value))
                {
                    return target.Value;
                }
                return string.Format(
                    CultureInfo.CurrentCulture,
                    CommonKeyIndexes.QueryString, target.Value, query.Value);
            }

            private List<HLinkDataRow>? _dataRows;
        }

        protected class CriminalCaseFetch : NonCriminalCaseFetch
        {
            // criminalCaseInclusion
            public CriminalCaseFetch(WebInteractive data) : base(data)
            {
            }

            public override List<HLinkDataRow> GetLinkedCases()
            {
                if (DataRows == null)
                {
                    return new List<HLinkDataRow>();
                }
                if (!IncludeCriminalRecords())
                {
                    return new List<HLinkDataRow>();
                }

                WebNavigationKey? parameter = GetParameter(Data, CommonKeyIndexes.IsCriminalSearch);
                if (parameter != null)
                {
                    parameter.Value = CommonKeyIndexes.NumberOne;
                }
                ModifyInstructions(CommonKeyIndexes.CriminalLinkQuery);
                List<HLinkDataRow> cases = Search(GetNavigationAddress() ?? string.Empty, DataRows);
                cases.ForEach(c => c.IsCriminal = true);
                return cases;
            }
        }

        internal static WebNavigationKey? GetParameter(
            WebInteractive data,
            string parameterName)
        {
            if (data == null)
            {
                return null;
            }

            if (data.Parameters == null)
            {
                return null;
            }

            if (data.Parameters.Keys == null)
            {
                return null;
            }

            if (!data.Parameters.Keys.Any())
            {
                return null;
            }

            const StringComparison ccic = StringComparison.CurrentCultureIgnoreCase;
            List<WebNavigationKey> keys = data.Parameters.Keys;
            return keys.Find(k => k.Name.Equals(parameterName, ccic));
        }
    }
}