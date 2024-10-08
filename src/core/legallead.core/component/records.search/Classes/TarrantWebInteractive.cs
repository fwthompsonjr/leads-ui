﻿using legallead.records.search.Addressing;
using legallead.records.search.Dto;
using legallead.records.search.Interfaces;
using legallead.records.search.Models;
using legallead.records.search.Web;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System.Text;
using System.Xml;

namespace legallead.records.search.Classes
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
    public partial class TarrantWebInteractive : WebInteractive
    {
        protected const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;

        #region Constructors

        public TarrantWebInteractive()
        { }

        public TarrantWebInteractive(WebNavigationParameter parameters) : base(parameters)
        {
        }

        public TarrantWebInteractive(WebNavigationParameter parameters, DateTime startDate, DateTime endingDate) : base(parameters, startDate, endingDate)
        {
            DateTimeFormatInfo formatDate = CultureInfo.CurrentCulture.DateTimeFormat;
            SetParameterValue(CommonKeyIndexes.StartDate,
                startDate.ToString(CommonKeyIndexes.DateTimeShort, formatDate));
            SetParameterValue(CommonKeyIndexes.EndDate,
                endingDate.ToString(CommonKeyIndexes.DateTimeShort, formatDate));
        }

        #endregion Constructors

        public override WebFetchResult Fetch()
        {
            // settings have been retrieved from the constructor
            // get any output file to store data from extract
            DateTime startingDate = GetParameterValue<DateTime>(CommonKeyIndexes.StartDate);
            DateTime endingDate = GetParameterValue<DateTime>(CommonKeyIndexes.EndDate);
            int customSearch = GetParameterValue<int>(CommonKeyIndexes.CriminalCaseInclusion);
            List<PersonAddress> peopleList = new();
            WebFetchResult webFetch = new();
            List<ITarrantWebFetch> fetchers = (new FetchProvider(this)).GetFetches(customSearch);
            DateTimeFormatInfo formatDate = CultureInfo.CurrentCulture.DateTimeFormat;
            if (!string.IsNullOrEmpty(Result))
            {
                _ = Persistence?.Add(UniqueId, "data-output-file-name", Result);
            }
            while (startingDate.CompareTo(endingDate) <= 0)
            {
                var dte = startingDate.ToString(CommonKeyIndexes.DateTimeShort, formatDate);
                _ = Persistence?.Add(UniqueId, "data-fetch-date", dte);
                SetParameterValue(CommonKeyIndexes.StartDate, dte);
                SetParameterValue(CommonKeyIndexes.EndDate, dte);
                foreach (ITarrantWebFetch obj in fetchers)
                {
                    obj.Fetch(startingDate, out webFetch, out List<PersonAddress> people);
                    peopleList.AddRange(people);
                    var addressobject = JsonConvert.SerializeObject(peopleList);
                    _ = Persistence?.Add(UniqueId, "data-output-person-address", addressobject);
                    _ = Persistence?.Add(UniqueId, "data-record-count", peopleList.Count.ToString());
                    webFetch.PeopleList = peopleList;
                }
                startingDate = startingDate.AddDays(1);
            }
            return webFetch;
        }

        private WebFetchResult SearchWeb(XmlContentHolder results,
            List<NavigationStep> steps,
            DateTime startingDate,
            DateTime endingDate,
            ref List<HLinkDataRow> cases,
            out List<PersonAddress> people)
        {
            IWebDriver driver = WebUtilities.GetWebDriver();

            try
            {
                return Search(results, steps, startingDate, endingDate, ref cases, out people, driver);
            }
            catch (Exception)
            {
                driver.Quit();
                driver.Dispose();
                throw;
            }
            finally
            {
                driver?.Quit();
            }
        }

        private WebFetchResult SearchWeb(
            int customSearchType,
            XmlContentHolder results,
            List<NavigationStep> steps,
            DateTime startingDate,
            DateTime endingDate,
            ref List<HLinkDataRow> cases,
            out List<PersonAddress> people)
        {
            IWebDriver driver = WebUtilities.GetWebDriver();

            try
            {
                WebFetchResult fetched = Search(results, steps, startingDate, endingDate, ref cases, out people, driver);
                if (customSearchType != 2)
                {
                    return fetched;
                }

                List<HLinkDataRow> caseList = cases.ToList();
                people = fetched.PeopleList;
                people.ForEach(p =>
                {
                    HLinkDataRow? source = caseList.Find(c => c.Case.Equals(p.CaseNumber, StringComparison.CurrentCultureIgnoreCase));
                    if (source == null)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(source.PageHtml))
                    {
                        return;
                    }

                    DataPointLocatorDto dto = DataPointLocatorDto.Load(source.PageHtml);
                    p.CaseStyle = dto.DataPoints
                        .First(f =>
                            f.Name.Equals(CommonKeyIndexes.CaseStyle,
                            StringComparison.CurrentCultureIgnoreCase)).Result;
                });

                return new WebFetchResult
                {
                    Result = results.FileName,
                    CaseList = fetched.CaseList,
                    PeopleList = people
                };
            }
            catch (Exception)
            {
                driver?.Quit();
                driver?.Dispose();
                throw;
            }
            finally
            {
                driver?.Quit();
            }
        }

        private WebFetchResult Search(XmlContentHolder results,
            List<NavigationStep> steps, DateTime startingDate,
            DateTime endingDate,
            ref List<HLinkDataRow> cases,
            out List<PersonAddress> people,
            IWebDriver driver)
        {
            ElementAssertion assertion = new(driver);
            string caseList = string.Empty;
            DateTimeFormatInfo formatDate = CultureInfo.CurrentCulture.DateTimeFormat;
            ElementActions.ForEach(x => x.GetAssertion = assertion);
            ElementActions.ForEach(x => x.GetWeb = driver);

            foreach (NavigationStep item in steps)
            {
                // if item action-name = 'set-text'
                string actionName = item.ActionName;
                if (item.ActionName.Equals(CommonKeyIndexes.SetText,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    if (item.DisplayName.Equals(CommonKeyIndexes.StartDate, //"startDate",
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        item.ExpectedValue =
                            startingDate.Date.ToString(CommonKeyIndexes.DateTimeShort, formatDate);
                    }

                    if (item.DisplayName.Equals(CommonKeyIndexes.EndDate,
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        item.ExpectedValue =
                            endingDate.Date.ToString(CommonKeyIndexes.DateTimeShort, formatDate);
                    }
                }
                IElementActionBase? action = ElementActions
                    .Find(x =>
                    x.ActionName.Equals(item.ActionName,
                    StringComparison.CurrentCultureIgnoreCase));
                if (action == null)
                {
                    continue;
                }

                action.Act(item);
                cases = ExtractCaseData(results, cases, actionName, action);
                if (string.IsNullOrEmpty(caseList) && !string.IsNullOrEmpty(action.OuterHtml))
                {
                    caseList = action.OuterHtml;
                }
            }
            cases.FindAll(c => string.IsNullOrEmpty(c.Address))
                .ForEach(c => GetAddressInformation(driver, this, c));
            people = ExtractPeople(cases);

            return new WebFetchResult
            {
                Result = results.FileName,
                CaseList = caseList,
                PeopleList = people
            };
        }

        protected virtual List<PersonAddress> ExtractPeople(List<HLinkDataRow> cases)
        {
            if (cases == null || !cases.Any())
            {
                return new List<PersonAddress>();
            }

            List<PersonAddress> list = new();
            foreach (HLinkDataRow item in cases)
            {
                string styleInfo = GetCaseStyle(item);
                PersonAddress person = new()
                {
                    Name = item.Defendant,
                    CaseNumber = item.Case,
                    DateFiled = item.DateFiled,
                    Court = item.Court,
                    CaseType = item.CaseType,
                    CaseStyle = styleInfo
                };
                person = ParseAddress(item.Address, person);
                list.Add(person);
            }
            return list;
        }

        /// <summary>
        /// Extracts the case data.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="cases">The cases.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        protected virtual List<HLinkDataRow> ExtractCaseData(XmlContentHolder results,
            List<HLinkDataRow> cases,
            string actionName, IElementActionBase action)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            if (cases == null)
            {
                throw new ArgumentNullException(nameof(cases));
            }

            if (actionName == null)
            {
                throw new ArgumentNullException(nameof(actionName));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (!actionName.Equals("get-table-html", comparison))
            {
                return cases;
            }

            if (string.IsNullOrEmpty(action.OuterHtml))
            {
                return cases;
            }

            ElementGetHtmlAction htmlAction = (ElementGetHtmlAction)action;
            bool isProbate = htmlAction.IsProbateSearch;
            bool isJustice = htmlAction.IsJusticeSearch;

            // create a list of hlinkdatarows from table
            string caseData = RemoveElement(action.OuterHtml, "<img");
            // remove colspan? <colgroup>
            caseData = RemoveTag(caseData, "colgroup");
            // load cases into [cases] object

            List<HLinkDataRow> newcases = LoadFromHtml(caseData);
            newcases.FindAll(x => !x.IsProbate).ForEach(c => c.IsProbate = isProbate);
            newcases.FindAll(x => !x.IsJustice).ForEach(c => c.IsJustice = isJustice);
            // map case information using file xpath
            newcases = AppendCourtInformation(newcases);

            // add this to the result file
            AppendToResult(results.FileName, caseData, "results/result[@name='casedata']");
            cases.AddRange(newcases);
            return cases;
        }

        protected static string GetCaseStyle(HLinkDataRow item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            XmlDocument doc = XmlDocProvider.GetDoc(item.Data);
            if (!doc.FirstChild!.HasChildNodes)
            {
                return string.Empty;
            }

            if (doc.FirstChild.ChildNodes.Count < 6)
            {
                return string.Empty;
            }

            int colIndex = 2;
            XmlNode? node = doc.FirstChild.ChildNodes[colIndex];
            if (node == null)
            {
                return string.Empty;
            }

            return node.InnerText;
        }

        protected static PersonAddress ParseAddress(string address, PersonAddress person)
        {
            string separator = @"<br/>";
            char pipe = '|';
            string pipeString = "|";
            const string noMatch = "No Match Found|Not Matched 00000";
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            if (string.IsNullOrEmpty(address)) { address = noMatch; }
            address = new StringBuilder(address.Trim()).Replace(separator, pipeString).ToString();
            if (address.EndsWith(pipeString, StringComparison.CurrentCultureIgnoreCase))
            {
                address = address[..^1];
            }
            List<string> pieces = address.Split(pipe)
                .ToList().FindAll(s => !string.IsNullOrEmpty(s));
            if (!pieces.Any())
            {
                return person;
            }
            person.Address1 = pieces[0].Trim();
            person.Address3 = pieces[^1].Trim();
            if (pieces.Count > 2)
            {
                int mx = pieces.Count - 1;
                person.Address2 = string.Empty;
                for (int i = 1; i < mx; i++)
                {
                    person.Address2 = string.Format(
                        CultureInfo.CurrentCulture,
                        "{0} {1}",
                        person.Address2, pieces[i].Trim()).Trim();
                }
            }
            List<string> zipPart = person.Address3.Split(' ').ToList();
            person.Zip = zipPart[^1];

            return person;
        }

        private static void GetAddressInformation(IWebDriver driver, TarrantWebInteractive jsonWebInteractive, HLinkDataRow linkData)
        {
            string? fmt = jsonWebInteractive.GetParameterValue<string>(CommonKeyIndexes.HlinkUri);
            string? xpath = jsonWebInteractive.GetParameterValue<string>(CommonKeyIndexes.PersonNodeXpath);
            if (string.IsNullOrEmpty(fmt) || string.IsNullOrEmpty(xpath)) return;
            ElementAssertion helper = new(driver);
            helper.Navigate(string.Format(CultureInfo.CurrentCulture,
                fmt, linkData.WebAddress));
            driver.WaitForNavigation();
            IWebElement? tdName = TryFindElement(driver, By.XPath(xpath));
            if (tdName == null)
            {
                return;
            }

            linkData.Defendant = tdName.GetAttribute(CommonKeyIndexes.InnerText);
            IWebElement parent = tdName.FindElement(By.XPath(CommonKeyIndexes.ParentElement));
            linkData.Address = parent.Text;
            try
            {
                // check or set IsCriminal attribute of linkData object
                IWebElement? criminalLink = TryFindElement(driver, By.XPath(CommonKeyIndexes.CriminalLinkXpath));
                if (criminalLink != null) { linkData.IsCriminal = true; }
                // get row index of this element ... and then go one row beyond...
                string ridx = parent.GetAttribute(CommonKeyIndexes.RowIndex);
                IWebElement table = parent.FindElement(By.XPath(CommonKeyIndexes.ParentElement));
                System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> trCol = table.FindElements(By.TagName(CommonKeyIndexes.TrElement));
                if (!int.TryParse(ridx, out int r))
                {
                    return;
                }

                parent = GetAddressRow(parent, trCol); // put this row-index into config... it can change
                linkData.Address = new StringBuilder(parent.Text)
                    .Replace(Environment.NewLine, "<br/>").ToString();
                FindCaseDataPoint findCase = new();
                findCase.Find(driver, linkData);
                driver.Navigate().Back();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void AppendToResult(string fileName, string caseData, string xpath)
        {

            XmlDocument doc = XmlDocProvider.Load(fileName);
            XmlNode? ndeCase = doc.DocumentElement?.SelectSingleNode(xpath);
            if (ndeCase == null)
            {
                return;
            }

            if (!ndeCase.HasChildNodes)
            {
                return;
            } ((XmlCDataSection)ndeCase.ChildNodes[0]!).Data = caseData;
            var html = doc.OuterXml;
            var expiration = TimeSpan.FromMinutes(15);
            ResourceFileService.AddOrUpdate(fileName, html, expiration);
        }

        private static List<HLinkDataRow> LoadFromHtml(string caseData)
        {
            List<HLinkDataRow> caseList = new();
            XmlDocument doc = XmlDocProvider.GetDoc(caseData);
            List<XmlNode>? trElements = doc.FirstChild?.ChildNodes[0]?.SelectNodes("tr")?.Cast<XmlNode>().ToList();
            if (trElements == null) return new();
            foreach (XmlNode? trow in trElements)
            {
                XmlNode? link = trow.SelectSingleNode("td/a");
                if (link == null)
                {
                    continue;
                }

                XmlNode? href = link.Attributes?.GetNamedItem("href");
                if (href == null)
                {
                    continue;
                }

                caseList.Add(new HLinkDataRow
                {
                    WebAddress = href.InnerText,
                    Data = trow.OuterXml
                });
            }

            return caseList;
        }

        private List<HLinkDataRow> AppendCourtInformation(List<HLinkDataRow> caseList
            )
        {
            int parameterId = Parameters.Id;
            string contents = SettingsManager.Content;
            XmlDocument doc = XmlDocProvider.GetDoc(contents);
            List<XmlNode> caseInspetor = GetCaseInspector(parameterId, doc);
            List<XmlNode> probateInspector = GetCaseInspector(parameterId, doc, "probate");
            List<XmlNode> justiceInspector = GetCaseInspector(parameterId, doc, "justice");

            foreach (HLinkDataRow item in caseList)
            {
                string data = item.Data;
                XmlDocument dcc = XmlDocProvider.GetDoc(data);
                XmlNode? trow = dcc.ChildNodes[0];
                List<XmlNode> inspector = item.IsProbate ? probateInspector : caseInspetor;
                if (item.IsJustice) { inspector = justiceInspector; }
                if (!MapCourtAttributes(item, trow, inspector))
                {
                    MapCourtAttributes(item, trow, probateInspector);
                    item.IsProbate = true;
                    item.CriminalCaseStyle = item.CaseStyle;
                }
            }
            return caseList;
        }

        private static bool MapCourtAttributes(HLinkDataRow item, XmlNode? trow, List<XmlNode> inspector)
        {
            if (trow == null) return false;
            foreach (XmlNode search in inspector)
            {
                XmlNode? node = trow.SelectSingleNode(search.InnerText);
                string? keyName = search.Attributes?.GetNamedItem("name")?.InnerText;
                if (node == null || string.IsNullOrEmpty(keyName)) return false;
                item[keyName] = node.InnerText;
            }
            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Cube",
            "S2589:Boolean expressions should not be gratuitous",
            Justification = "This method is legacy code to be refactored at later date")]
        private static List<XmlNode> GetCaseInspector(int parameterId, XmlDocument doc, string typeName = "normal")
        {
            try
            {
                var pid = parameterId.ToString(CultureInfo.CurrentCulture.NumberFormat);
                var nodes = doc.DocumentElement?
                    .SelectSingleNode("directions")?
                    .SelectNodes("caseInspection")?
                    .Cast<XmlNode>();
                if (nodes == null) return new();
                var inspector = nodes
                    .FirstOrDefault(x => GetAttributeValue(x, "id").Equals(pid) && GetAttributeValue(x, "type").Equals(typeName))
                    ?.ChildNodes
                    ?.Cast<XmlNode>()
                    ?.ToList();
                return inspector ?? new();
            }
            catch
            {
                return new();
            }
        }

        private static string GetAttributeValue(XmlNode node, string named)
        {
            if (node.Attributes == null) return string.Empty;
            if (node.Attributes.GetNamedItem(named) == null) return string.Empty;
            return node.Attributes.GetNamedItem(named)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Locates the address element from the case-detail drill down page.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="trCol">The tr col.</param>
        /// <returns></returns>
        private static IWebElement GetAddressRow(IWebElement parent,
            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> trCol)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            int colIndex = 3;
            parent = trCol[colIndex];
            string txt = string.IsNullOrEmpty(parent.Text) ? "" : parent.Text.Trim();
            if (string.IsNullOrEmpty(txt))
            {
                parent = trCol[colIndex - 1];
            }

            return parent;
        }

        /// <summary>Tries the find element on a specfic web page using the By condition supplied.</summary>
        /// <param name="parent">The parent web browser instance.</param>
        /// <param name="by">The by condition used to locate the element</param>
        /// <returns></returns>
        internal static IWebElement? TryFindElement(IWebDriver parent, By by)
        {
            try
            {
                return parent.FindElement(by);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region Element Action Helpers

        private static List<IElementActionBase>? elementActions;

        protected static List<IElementActionBase> ElementActions
        {
            get { return elementActions ??= GetActions(); }
        }

        protected static List<IElementActionBase> GetActions()
        {
            StructureMap.Container container =
            ActionElementContainer.GetContainer;
            return container.GetAllInstances<IElementActionBase>().ToList();
        }

        protected static NavigationInstructionDto GetAppSteps(string suffix = "")
        {
            const string dataFormat = @"{0}\xml\{1}.json";
            string appDirectory = ContextManagment.AppDirectory;
            string dataFile = string.Format(
                CultureInfo.CurrentCulture,
                dataFormat,
                appDirectory,
                suffix);

            var fallback = GetFallbackContent(suffix);
            var data = SettingFileService.GetContentOrDefault(dataFile, fallback);
            if (string.IsNullOrEmpty(data))
            {
                throw new FileNotFoundException(
                    CommonKeyIndexes.NavigationFileNotFound);
            }
            return JsonConvert.DeserializeObject<NavigationInstructionDto>(data) ?? new();
        }

        private static TarrantCourtDropDownDto? _tarrantComboBxValue;

        protected static TarrantCourtDropDownDto TarrantComboBxValue => _tarrantComboBxValue ??= GetComboBoxValues();

        public static TarrantCourtDropDownDto GetComboBoxValues()
        {
            const string suffix = "tarrantCourtSearchDropDown";
            var resourceText = Properties.Resources.xml_tarrantCourtSearchDropDown_json;
            var fallback = GetFallbackContent(suffix);
            var data = !string.IsNullOrEmpty(resourceText) ? resourceText : fallback;
            if (string.IsNullOrEmpty(data))
            {
                throw new FileNotFoundException(
                    CommonKeyIndexes.NavigationFileNotFound);
            }
            return JsonConvert.DeserializeObject<TarrantCourtDropDownDto>(data) ?? new();
        }

        #endregion Element Action Helpers

        #region Fallback Content

        private static string GetFallbackContent(string fileName)
        {
            StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var sbb = new StringBuilder();
            const char tilde = '~';
            const char qte = '"';
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            if (fileName.Equals("tarrantCourtSearchDropDown", oic))
            {
                // tarrantCourtSearchDropDown.json
                sbb.AppendLine("{");
                sbb.AppendLine("	~name~: ~court type selections~,");
                sbb.AppendLine("	~step~: {");
                sbb.AppendLine("		~actionName~: ~set-select-value~,");
                sbb.AppendLine("		~displayName~: ~case-type-selector~,");
                sbb.AppendLine("		~expectedValue~: ~1~,");
                sbb.AppendLine("		~locator~: {");
                sbb.AppendLine("			~find~: ~css~,");
                sbb.AppendLine("			~query~: ~#sbxControlID2~");
                sbb.AppendLine("		}");
                sbb.AppendLine("	},");
                sbb.AppendLine("	~courtMap~: [");
                sbb.AppendLine("		{");
                sbb.AppendLine("			~id~: 1,");
                sbb.AppendLine("			~name~: ~Justice of Peace~");
                sbb.AppendLine("		},");
                sbb.AppendLine("		{");
                sbb.AppendLine("			~id~: 2,");
                sbb.AppendLine("			~name~: ~Court Court at Law~");
                sbb.AppendLine("		},");
                sbb.AppendLine("		{");
                sbb.AppendLine("			~id~: 0,");
                sbb.AppendLine("			~name~: ~Probate~");
                sbb.AppendLine("		}");
                sbb.AppendLine("	]");
                sbb.AppendLine("}");
            }
            if (fileName.Equals("tarrantCountyMapping_1", oic))
            {
                sbb.AppendLine("{");
                sbb.AppendLine("  ~steps~: [");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~navigate~,");
                sbb.AppendLine("      ~displayName~: ~open-website-base-uri~,");
                sbb.AppendLine("      ~wait~: 1200,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~page~,");
                sbb.AppendLine("        ~query~: ~https://odyssey.tarrantcounty.com/PublicAccess/default.aspx~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~exists~,");
                sbb.AppendLine("      ~displayName~: ~case-type-selector~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#sbxControlID2~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-select-value~,");
                sbb.AppendLine("      ~displayName~: ~case-type-selector~,");
                sbb.AppendLine("      ~expectedValue~: ~1~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#sbxControlID2~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~case-records-hyperlink~,");
                sbb.AppendLine("      ~expectedValue~: ~Case Records Search~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~/html/body/table/tbody/tr[2]/td/table/tbody/tr[1]/td[2]/a[2]~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~exists~,");
                sbb.AppendLine("      ~displayName~: ~search-by-selector~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-select-value~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~expectedValue~: ~3~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~send-key~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~expaectedValue~: ~K~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-text~,");
                sbb.AppendLine("      ~displayName~: ~startDate~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#DateFiledOnAfter~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-text~,");
                sbb.AppendLine("      ~displayName~: ~endDate~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#DateFiledOnBefore~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~submit-button~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchSubmit~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~break-point-ignore~,");
                sbb.AppendLine("      ~displayName~: ~submit-button~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchSubmit~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~get-record-count~,");
                sbb.AppendLine("      ~displayName~: ~record-count~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~/html/body/table[3]/tbody/tr[1]/td[2]/b~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~get-table-html~,");
                sbb.AppendLine("      ~displayName~: ~get-case-data-table~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~/html/body/table[4]~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    }");
                sbb.AppendLine("  ]");
                sbb.AppendLine("}");
            }
            if (fileName.Equals("tarrantCountyMapping_2", oic))
            {
                // tarrantCountyMapping_2
                sbb.AppendLine("{");
                sbb.AppendLine("  ~steps~: [");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~navigate~,");
                sbb.AppendLine("      ~displayName~: ~open-website-base-uri~,");
                sbb.AppendLine("      ~wait~: 1200,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~page~,");
                sbb.AppendLine("        ~query~: ~https://odyssey.tarrantcounty.com/PublicAccess/default.aspx~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~exists~,");
                sbb.AppendLine("      ~displayName~: ~case-type-selector~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#sbxControlID2~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-select-value~,");
                sbb.AppendLine("      ~displayName~: ~case-type-selector~,");
                sbb.AppendLine("      ~expectedValue~: ~1~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#sbxControlID2~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~criminal-records-hyperlink~,");
                sbb.AppendLine("      ~expectedValue~: ~Criminal Case Records~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~//a[@class='ssSearchHyperlink'][contains(text(),'Misdemeanors')]~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~exists~,");
                sbb.AppendLine("      ~displayName~: ~search-by-selector~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-dropdown-value~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~expectedValue~: ~5~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~send-key~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~expaectedValue~: ~K~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchBy~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-text~,");
                sbb.AppendLine("      ~displayName~: ~startDate~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#DateFiledOnAfter~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-text~,");
                sbb.AppendLine("      ~displayName~: ~endDate~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#DateFiledOnBefore~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~submit-button~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchSubmit~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~break-point-ignore~,");
                sbb.AppendLine("      ~displayName~: ~submit-button~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchSubmit~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~get-record-count~,");
                sbb.AppendLine("      ~displayName~: ~record-count~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~/html/body/table[3]/tbody/tr[1]/td[2]/b~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~get-table-html~,");
                sbb.AppendLine("      ~displayName~: ~get-case-data-table~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~/html/body/table[4]~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    }");
                sbb.AppendLine("  ]");
                sbb.AppendLine("}");
            }
            if (fileName.Equals("collinCountyMapping_1", oic))
            {
                sbb.AppendLine("{");
                sbb.AppendLine("  ~steps~: [");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~navigate~,");
                sbb.AppendLine("      ~displayName~: ~open-website-base-uri~,");
                sbb.AppendLine("      ~wait~: 1200,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~page~,");
                sbb.AppendLine("        ~query~: ~https://cijspub.co.collin.tx.us/SecurePA/login.aspx~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~login~,");
                sbb.AppendLine("      ~displayName~: ~login-to-website~,");
                sbb.AppendLine("      ~expectedValue~: ~collinCountyUserMap~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#UserName | #Password~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~login-collin-county~,");
                sbb.AppendLine("      ~displayName~: ~login-submit~,");
                sbb.AppendLine("      ~expectedValue~: ~collinCountyUserMap~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#UserName | #Password~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-select-value~,");
                sbb.AppendLine("      ~displayName~: ~sbxControlID2~,");
                sbb.AppendLine("      ~expectedValue~: ~1~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#sbxControlID2~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~break-point-ignore~,");
                sbb.AppendLine("      ~displayName~: ~login-submit~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~input[type='submit']~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~search-type-hyperlink~,");
                sbb.AppendLine("      ~expectedValue~: ~1~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#tobesetdynamically~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~date-filed-option~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#DateFiled~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~case-status-option~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#OpenOption~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-text~,");
                sbb.AppendLine("      ~displayName~: ~startDate~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#DateFiledOnAfter~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~set-text~,");
                sbb.AppendLine("      ~displayName~: ~endDate~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#DateFiledOnBefore~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~click~,");
                sbb.AppendLine("      ~displayName~: ~case-type-search~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchSubmit~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~break-point-ignore~,");
                sbb.AppendLine("      ~displayName~: ~submit-button~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#SearchSubmit~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~get-record-count~,");
                sbb.AppendLine("      ~displayName~: ~record-count~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~/html/body/table[3]/tbody/tr[1]/td[2]/b~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~get-table-html~,");
                sbb.AppendLine("      ~displayName~: ~get-case-data-table~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~xpath~,");
                sbb.AppendLine("        ~query~: ~/html/body/table[4]~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    }");
                sbb.AppendLine("  ]");
                sbb.AppendLine("}");
            }
            if (fileName.Equals("collinCountyCaseType", oic))
            {
                sbb.AppendLine("{");
                sbb.AppendLine("  ~dropDowns~: [");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 0,");
                sbb.AppendLine("      ~name~: ~criminal courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~criminal case records~,");
                sbb.AppendLine("          ~query~: ~#divOption1 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 1,");
                sbb.AppendLine("          ~name~: ~probate case records~,");
                sbb.AppendLine("          ~query~: ~#divOption2 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 2,");
                sbb.AppendLine("          ~name~: ~magistrate case records~,");
                sbb.AppendLine("          ~query~: ~#divOption3 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 3,");
                sbb.AppendLine("          ~name~: ~civil and family case records~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 4,");
                sbb.AppendLine("          ~name~: ~justice of the peace case records~,");
                sbb.AppendLine("          ~query~: ~#divOption5 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 1,");
                sbb.AppendLine("      ~name~: ~probate courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~probate case records~,");
                sbb.AppendLine("          ~query~: ~#divOption2 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 2,");
                sbb.AppendLine("      ~name~: ~magistrate courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~magistrate case records~,");
                sbb.AppendLine("          ~query~: ~#divOption3 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 3,");
                sbb.AppendLine("      ~name~: ~civil and family courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~civil and family case records~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 4,");
                sbb.AppendLine("      ~name~: ~justice courts~,");
                sbb.AppendLine("      ~query~: ~#sbxControlID2~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~justice of the peace case records~,");
                sbb.AppendLine("          ~query~: ~#divOption5 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    }");
                sbb.AppendLine("  ]");
                sbb.AppendLine("}");
            }
            if (fileName.Equals("harrisCivilMapping", oic))
            {
                sbb.AppendLine("{");
                sbb.AppendLine("  ~steps~: [");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~navigate~,");
                sbb.AppendLine("      ~displayName~: ~open-website-base-uri~,");
                sbb.AppendLine("      ~wait~: 1200,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~page~,");
                sbb.AppendLine("        ~query~: ~https://www.cclerk.hctx.net/Applications/WebSearch/CourtSearch.aspx?CaseType=Civil~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~jquery-set-selected-index~,");
                sbb.AppendLine("      ~displayName~: ~court-drop-down list~,");
                sbb.AppendLine("      ~expectedValue~: ~1~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#ctl00_ContentPlaceHolder1_ddlCourt~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~jquery-set-selected-index~,");
                sbb.AppendLine("      ~displayName~: ~case-status-drop-down list~,");
                sbb.AppendLine("      ~expectedValue~: ~1~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#ctl00_ContentPlaceHolder1_DropDownListStatus~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~jquery-set-text~,");
                sbb.AppendLine("      ~displayName~: ~startDate~,");
                sbb.AppendLine("      ~expectedValue~: ~1~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#ctl00_ContentPlaceHolder1_txtFrom~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~jquery-set-text~,");
                sbb.AppendLine("      ~displayName~: ~endDate~,");
                sbb.AppendLine("      ~expectedValue~: ~1~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#ctl00_ContentPlaceHolder1_txtTo~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~harris-civil-search-click~,");
                sbb.AppendLine("      ~displayName~: ~login-submit~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~#ctl00_ContentPlaceHolder1_btnSearchCase~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~actionName~: ~harris-civil-read-table~,");
                sbb.AppendLine("      ~displayName~: ~read-result~,");
                sbb.AppendLine("      ~locator~: {");
                sbb.AppendLine("        ~find~: ~css~,");
                sbb.AppendLine("        ~query~: ~harris-civil-reader~");
                sbb.AppendLine("      }");
                sbb.AppendLine("    }");
                sbb.AppendLine("  ]");
                sbb.AppendLine("}");
            }
            if (fileName.Equals("harrisCivilCaseType", oic))
            {
                sbb.AppendLine("{");
                sbb.AppendLine("  ~dropDowns~: [");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 0,");
                sbb.AppendLine("      ~name~: ~COUNTY CIVIL COURTS~,");
                sbb.AppendLine("      ~query~: ~#ctl00_ContentPlaceHolder1_ddlCourt~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~All~,");
                sbb.AppendLine("          ~query~: ~#divOption1 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 1,");
                sbb.AppendLine("          ~name~: ~1~,");
                sbb.AppendLine("          ~query~: ~#divOption2 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 2,");
                sbb.AppendLine("          ~name~: ~2~,");
                sbb.AppendLine("          ~query~: ~#divOption3 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 3,");
                sbb.AppendLine("          ~name~: ~3~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 4,");
                sbb.AppendLine("          ~name~: ~4~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 1,");
                sbb.AppendLine("      ~name~: ~COUNTY CASE STATUSES~,");
                sbb.AppendLine("      ~query~: ~~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~-All~,");
                sbb.AppendLine("          ~query~: ~#divOption1 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 1,");
                sbb.AppendLine("          ~name~: ~Closed~,");
                sbb.AppendLine("          ~query~: ~#divOption2 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 2,");
                sbb.AppendLine("          ~name~: ~Dismissed~,");
                sbb.AppendLine("          ~query~: ~#divOption3 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 3,");
                sbb.AppendLine("          ~name~: ~Inactive~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 4,");
                sbb.AppendLine("          ~name~: ~Open~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 5,");
                sbb.AppendLine("          ~name~: ~Open Garnishment~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 6,");
                sbb.AppendLine("          ~name~: ~Other~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 7,");
                sbb.AppendLine("          ~name~: ~Re-Opened~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 8,");
                sbb.AppendLine("          ~name~: ~Reopen Garnishment~,");
                sbb.AppendLine("          ~query~: ~#divOption4 > a~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    },");
                sbb.AppendLine("    {");
                sbb.AppendLine("      ~id~: 2,");
                sbb.AppendLine("      ~name~: ~DEFAULT INDEXES~,");
                sbb.AppendLine("      ~query~: ~#ctl00_ContentPlaceHolder1_ddlCourt~,");
                sbb.AppendLine("      ~options~: [");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 0,");
                sbb.AppendLine("          ~name~: ~ctl00$ContentPlaceHolder1$ddlCourt~,");
                sbb.AppendLine("          ~query~: ~0~");
                sbb.AppendLine("        },");
                sbb.AppendLine("        {");
                sbb.AppendLine("          ~id~: 1,");
                sbb.AppendLine("          ~name~: ~ctl00$ContentPlaceHolder1$DropDownListStatus~,");
                sbb.AppendLine("          ~query~: ~4~");
                sbb.AppendLine("        }");
                sbb.AppendLine("      ]");
                sbb.AppendLine("    }");
                sbb.AppendLine("  ]");
                sbb.AppendLine("}");
            }
            if (fileName.Equals("harrisJpMapping", oic))
            {
                var content = Properties.Resources.xml_harrisJpMapping_json;
                sbb.AppendLine(content);
            }
            sbb.Replace(tilde, qte);
            return sbb.ToString();
        }

        #endregion Fallback Content


    }
}