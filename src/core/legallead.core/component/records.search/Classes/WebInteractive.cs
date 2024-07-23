using legallead.records.search.Dto;
using legallead.records.search.Models;
using legallead.records.search.Tools;
using System.Xml;
using JsonConvert = Newtonsoft.Json.JsonConvert;
using Newt = Newtonsoft.Json;

namespace legallead.records.search.Classes
{
    /// <summary>
    /// Class defintion for WebInteractive class which is used to
    /// recieve in bound parameter collection and processes the request
    /// </summary>
    public class WebInteractive : BaseWebIneractive
    {
        public DentonTableRead? DentonContent { get; internal set; }
        #region Constructors

        public WebInteractive()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebInteractive"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public WebInteractive(WebNavigationParameter parameters)
        {
            Parameters = parameters;
            StartDate = GetParameterValue<DateTime>(CommonKeyIndexes.StartDate);
            EndingDate = GetParameterValue<DateTime>(CommonKeyIndexes.EndDate);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebInteractive"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endingDate">The ending date.</param>
        public WebInteractive(WebNavigationParameter parameters, DateTime startDate, DateTime endingDate)
        {
            Parameters = parameters;
            StartDate = startDate;
            EndingDate = endingDate;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Performs web scraping activities to fetches data from web source.
        /// </summary>
        /// <returns></returns>
        public override WebFetchResult Fetch()
        {
            XmlContentHolder results = new SettingsManager().GetOutput(this);
            List<HLinkDataRow> data = WebUtilities.GetCases(this);
            _ = Persistence?.Add(UniqueId, "data-record-count", data.Count.ToString());
            Result = results.FileName;
            data.ForEach(dta =>
            {
                if (DentonContent == null)
                    AppendExtraCaseInfo(dta);
                results.Append(dta);
            });
            // change output of this item.
            // return the person-address collection
            // and the case-list as html table
            //results.Document
            data.FindAll(x => x.IsCriminal && x.IsMapped && !string.IsNullOrEmpty(x.CriminalCaseStyle))
                .ForEach(y => y.CaseStyle = y.CriminalCaseStyle);
            // serialize and save this data object
            var dsobject = JsonConvert.SerializeObject(data, Newt.Formatting.Indented);
            _ = Persistence?.Add(UniqueId, "data-case-list-json", dsobject);
            _ = Persistence?.Add(UniqueId, "data-output-file-name", Result);
            var isDenton = DentonContent != null;
            string caseList = isDenton ? string.Empty : ReadFromFile(Result);
            List<PersonAddress> personAddresses = isDenton ?
                DentonLinkDataMapper.ConvertFrom(data) :
                results.GetPersonAddresses();
            if (!isDenton)
            {
                personAddresses = MapCaseStyle(data, personAddresses);
            }
            personAddresses = CleanUp(personAddresses);
            var addressobject = JsonConvert.SerializeObject(personAddresses, Newt.Formatting.Indented);
            _ = Persistence?.Add(UniqueId, "data-output-person-address", addressobject);

            return new WebFetchResult
            {
                Result = Result,
                CaseList = caseList,
                PeopleList = personAddresses
            };
        }

        private static List<PersonAddress> CleanUp(List<PersonAddress> personAddresses)
        {
            List<PersonAddress> found = personAddresses
                .FindAll(f =>
                f.Address1.Equals("Pro Se", StringComparison.CurrentCultureIgnoreCase));
            if (found.Any())
            {
                foreach (PersonAddress item in found)
                {
                    item.Zip = CommonKeyIndexes.NonAddressZipCode;
                    item.Address1 = CommonKeyIndexes.NonAddressLine1;
                    item.Address2 = string.Empty;
                    item.Address3 = CommonKeyIndexes.NonAddressLine2;
                }
            }
            found = personAddresses
                .FindAll(f =>
                f.Address1.Equals(CommonKeyIndexes.NonAddressLine1, StringComparison.CurrentCultureIgnoreCase) & string.IsNullOrEmpty(f.Zip));

            if (found.Any())
            {
                foreach (PersonAddress item in found)
                {
                    item.Zip = CommonKeyIndexes.NonAddressZipCode;
                }
            }
            return personAddresses;
        }

        /// <summary>
        /// Maps the case style.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="personAddresses">The person addresses.</param>
        /// <returns></returns>
        private static List<PersonAddress> MapCaseStyle(List<HLinkDataRow> data,
            List<PersonAddress> personAddresses)
        {
            if (personAddresses == null || data == null)
            {
                return new();
            }

            data = data.FindAll(x => !string.IsNullOrEmpty(x.Case));
            foreach (PersonAddress person in personAddresses)
            {
                string caseNumber = person.CaseNumber;
                if (string.IsNullOrEmpty(caseNumber))
                {
                    continue;
                }

                HLinkDataRow? dataRow = data.Find(x => x.Case.Equals(caseNumber,
                    StringComparison.CurrentCultureIgnoreCase));
                if (dataRow == null)
                {
                    continue;
                }

                person.CaseStyle = dataRow.CaseStyle;
            }

            return personAddresses;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Appends the extra case information to populate
        /// non-address fields in the PeopleData collection
        /// </summary>
        /// <param name="dta">The dta.</param>
        private void AppendExtraCaseInfo(HLinkDataRow dta)
        {
            string tableHtml = dta.Data;
            if (string.IsNullOrEmpty(tableHtml)) { return; }
            if (tableHtml.Contains(CommonKeyIndexes.ImageOpenTag))
            {
                tableHtml = RemoveElement(tableHtml, CommonKeyIndexes.ImageOpenTag);
            }
            XmlDocument doc = XmlDocProvider.GetDoc(tableHtml);
            List<WebNavInstruction> instructions = SearchSettingDto.GetNonCriminalMapping()
                    .NavInstructions
                    .ToList();
            // if doc.DocumentElement.ChildNodes.Count == 4
            if (dta.IsCriminal)
            {
                instructions =
                    SearchSettingDto.GetCriminalMapping()
                    .NavInstructions
                    .ToList();
            }
            string caseStyle = CommonKeyIndexes.CaseStyle.ToLower(CultureInfo.CurrentCulture);
            foreach (WebNavInstruction? item in instructions)
            {
                if (dta.IsCriminal & item.Value.Equals(caseStyle, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                XmlNode? node = TryFindNode(doc, item.Value);
                if (node == null)
                {
                    continue;
                }

                dta[item.Name] = node.InnerText;
                dta.IsMapped = true;
            }
        }

        /// <summary>
        /// Tries the find node.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="xpath">The xpath.</param>
        /// <returns></returns>
        private static XmlNode? TryFindNode(XmlDocument doc, string xpath)
        {
            try
            {
                XmlNode? node = doc.FirstChild?.SelectSingleNode(xpath);
                return node;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion Private Methods
    }
}