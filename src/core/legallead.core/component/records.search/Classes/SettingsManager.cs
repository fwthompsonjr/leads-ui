using legallead.records.search.Models;
using System.Reflection;
using System.Text;
using System.Xml;

namespace legallead.records.search.Classes
{
    public static class XmlDocProvider
    {
        public static XmlDocument GetDoc(string xml)
        {
            try
            {
                XmlDocument doc = new() { XmlResolver = null };
                System.IO.StringReader sreader = new(xml);
                XmlReader reader = XmlReader.Create(sreader, new XmlReaderSettings() { XmlResolver = null });
                doc.Load(reader);
                return doc;
            }
            catch (Exception)
            {
                return GetDocFromString(xml);
            }
        }

        private static XmlDocument GetDocFromString(string xml)
        {
            XmlDocument doc = new();
            doc.LoadXml(xml);
            return doc;
        }

        public static XmlDocument Load(string fileName)
        {
            using StreamReader reader = new(fileName);
            string content = reader.ReadToEnd();
            return GetDoc(content);
        }
    }

    /// <summary>Class definition for settings reader utility which reads the application settings xml file to map parameters to the search process.</summary>
    public class SettingsManager
    {
        #region Fields

        private string? _layoutContent;

        private string? _excelFileName;

        #endregion Fields

        #region Properties

        /// <summary>Gets the file name for the excel format file.</summary>
        /// <value>The excel format file.</value>
        public string ExcelFormatFile
        {
            get { return _excelFileName ??= GetExcelFileName(); }
        }

        /// <summary>
        /// Gets the content of the xml source file.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public static string Content
        {
            get { return LoadFile("settings.xml"); }
        }

        /// <summary>
        /// Gets the layout of the xml source file.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Layout
        {
            get { return _layoutContent ??= LoadFile("caselayout.xml"); }
        }

        #endregion Properties

        /// <summary>
        /// Reads the settings file to gets the navigation
        /// settings for record search processes.
        /// </summary>
        /// <returns></returns>
        public static List<WebNavigationParameter> GetNavigation()
        {
            string data = Content;

            XmlDocument doc = XmlDocProvider.GetDoc(data);
            if (doc.DocumentElement == null)
            {
                return new();
            }

            XmlNode? parent = doc.DocumentElement.SelectSingleNode("setting[@name='Websites']");
            if (parent == null) return new();
            List<WebNavigationParameter> response = new();
            foreach (object? node in parent.ChildNodes)
            {
                response.Add(MapFrom((XmlNode)node));
            }
            return response;
        }

        /// <summary>
        /// Gets the column layouts from settings file.
        /// </summary>
        /// <param name="id">The website identifier.</param>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns></returns>
        public static List<ExcelColumnLayout> GetColumnLayouts(int id, string sectionName)
        {
            string data = Content;
            XmlDocument doc = XmlDocProvider.GetDoc(data);
            if (doc.DocumentElement == null)
            {
                return new();
            }

            XmlNode? parent = doc.DocumentElement.SelectSingleNode(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "layouts/layout[@id='{0}' and @name='{1}']",
                id != 1 ? 1 : id,
                sectionName));
            if (id == 30)
            {
                parent = doc.DocumentElement.SelectSingleNode(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "layouts/layout[@id='{0}' and @name='{1}']",
                id,
                sectionName));
            }
            if (parent == null)
            {
                return new();
            }

            XmlNode? columnNode = parent.FirstChild;
            if (columnNode == null)
            {
                return new();
            }

            if (!columnNode.HasChildNodes)
            {
                return new();
            }

            List<ExcelColumnLayout> layoutList = new();
            List<XmlNode> columnList = columnNode.ChildNodes.Cast<XmlNode>().ToList();
            foreach (XmlNode? column in columnList)
            {
                layoutList.Add(new ExcelColumnLayout
                {
                    Name = column.Attributes!.GetNamedItem("name")!.InnerText,
                    ColumnWidth = Convert.ToInt32(
                        column.Attributes.GetNamedItem("columnWidth")!.InnerText,
                        CultureInfo.CurrentCulture.NumberFormat)
                });
            }
            return layoutList;
        }

        /// <summary>
        /// Generates the xml output file to hold record search results.
        /// </summary>
        /// <param name="settingFile">The setting file.</param>
        /// <returns></returns>
        public XmlContentHolder GetOutput(WebInteractive settingFile)
        {
            if (settingFile == null)
            {
                throw new ArgumentNullException(nameof(settingFile));
            }

            const string dfmt = "MMddyyyy";
            string fileName = GetFileName(settingFile);
            string targetFile = fileName;
            int idx = 0;
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            NumberFormatInfo numberInfo = cultureInfo.NumberFormat;
            while (File.Exists(targetFile))
            {
                idx += 1;
                string cleaned = Path.GetFileNameWithoutExtension(fileName);
                cleaned = string.Format(cultureInfo, "{0}_{1}.xml", cleaned, idx.ToString("D9", numberInfo));
                targetFile = string.Format(cultureInfo, "{0}/{1}", Path.GetDirectoryName(fileName), cleaned);
            }
            fileName = targetFile;
            using (StreamWriter sw = new(fileName))
            {
                sw.Write(Layout);
                sw.Close();
            }
            string content = string.Empty;
            using (StreamReader reader = new(fileName))
            {
                content = reader.ReadToEnd();
            }
            XmlDocument doc = XmlDocProvider.GetDoc(content);
            XmlNode? nde = doc.DocumentElement!.SelectSingleNode(@"parameters");
            List<XmlNode> nds = new(nde!.ChildNodes.Cast<XmlNode>());
            foreach (XmlNode item in nds)
            {
                string? attrName = item.Attributes?.GetNamedItem("name")?.Value;
                switch (attrName)
                {
                    case "Website":
                        item.InnerText = settingFile.Parameters.Name;
                        break;

                    case "StartDate":
                        item.InnerText = settingFile.StartDate.ToString(dfmt, CultureInfo.CurrentCulture.DateTimeFormat);
                        break;

                    case "EndDate":
                        item.InnerText = settingFile.EndingDate.ToString(dfmt, CultureInfo.CurrentCulture.DateTimeFormat);
                        break;

                    case "SearchDate":
                        item.InnerText = DateTime.Now.ToString(dfmt, CultureInfo.CurrentCulture.DateTimeFormat);
                        break;

                    default:
                        break;
                }
            }

            doc.Save(fileName);

            // check for null
            return new XmlContentHolder
            {
                Id = settingFile.Parameters.Id,
                FileName = fileName,
                Document = doc
            };
        }

        private static WebNavigationParameter MapFrom(XmlNode node)
        {
            WebNavigationParameter parameter = new()
            {
                Id = Convert.ToInt32(
                    node.Attributes!.GetNamedItem("id")!.Value,
                    CultureInfo.CurrentCulture.NumberFormat),
                Name = node.Attributes?.GetNamedItem("name")?.Value ?? string.Empty,
                Keys = new List<WebNavigationKey>(),
                Instructions = new List<WebNavInstruction>(),
                CaseInstructions = new List<WebNavInstruction>()
            };
            foreach (object? item in node.ChildNodes)
            {
                XmlNode nde = (XmlNode)item;
                parameter.Keys.Add(new WebNavigationKey
                {
                    Name = nde.Attributes?.GetNamedItem("name")?.Value ?? string.Empty,
                    Value = ((XmlCDataSection)nde.FirstChild!).Data
                });
            }
            string qpath = string.Format(
                CultureInfo.CurrentCulture,
                "directions/instructions[@id={0}]", parameter.Id);

            XmlNode? instructions = node.OwnerDocument?.DocumentElement?.SelectSingleNode(qpath);
            if (instructions == null)
            {
                return parameter;
            }

            foreach (object? item in instructions.ChildNodes)
            {
                XmlNode nde = (XmlNode)item;
                parameter.Instructions.Add(new WebNavInstruction
                {
                    Name = nde.Attributes?.GetNamedItem("name")?.Value ?? string.Empty,
                    By = nde.Attributes?.GetNamedItem("By")?.Value ?? string.Empty,
                    CommandType = nde.Attributes?.GetNamedItem("type")?.Value ?? string.Empty,
                    FriendlyName = nde.Attributes?.GetNamedItem("FriendlyName")?.Value ?? string.Empty,
                    Value = ((XmlCDataSection)nde.FirstChild!).Data
                });
            }

            qpath = string.Format(
                CultureInfo.CurrentCulture,
                "directions/caseInspection[@id={0}]", parameter.Id);
            instructions = node.OwnerDocument?.DocumentElement?.SelectSingleNode(qpath);
            if (instructions == null)
            {
                return parameter;
            }

            foreach (object? item in instructions.ChildNodes)
            {
                XmlNode nde = (XmlNode)item;
                parameter.CaseInstructions.Add(new WebNavInstruction
                {
                    Name = nde.Attributes?.GetNamedItem("name")?.Value ?? string.Empty,
                    By = nde.Attributes?.GetNamedItem("By")?.Value ?? string.Empty,
                    CommandType = nde.Attributes?.GetNamedItem("type")?.Value ?? string.Empty,
                    FriendlyName = nde.Attributes?.GetNamedItem("FriendlyName")?.Value ?? string.Empty,
                    Value = ((XmlCDataSection)nde.FirstChild!).Data
                });
            }

            return parameter;
        }

        /// <summary>
        /// Loads the resource xml file from data folder.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>the contents of specified file as string</returns>
        private static string LoadFile(string fileName)
        {
            string? execName = new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            string targetFile = new Uri(string.Format(
                CultureInfo.CurrentCulture,
                @"{0}\xml\{1}", execName, fileName)).AbsolutePath;
            var backup = GetXmlContent(fileName);
            var content = !File.Exists(targetFile) ? backup : File.ReadAllText(targetFile);
            if (string.IsNullOrWhiteSpace(content)) { return backup; }
            return content;
        }

        public static string GetXmlContent(string fileName)
        {
            const string tilde = "~";
            string quote = '"'.ToString();

            switch (fileName)
            {
                case "settings.xml":
                    StringBuilder sb = new();
                    sb.AppendLine("<?xml version=~1.0~ encoding=~utf-8~ ?>");
                    sb.AppendLine("<settings>");
                    sb.AppendLine("  <setting name=~Websites~>");
                    sb.AppendLine("    <keys id=~1~ name=~Denton County~>");
                    sb.AppendLine("      <key name=~baseUri~><![CDATA[http://justice1.dentoncounty.com/PublicAccess/Search.aspx]]></key>");
                    sb.AppendLine("      <key name=~query~><![CDATA[ID=200&NodeID=1251,1252,1253,1254,1255,1256,1260,1270,1280,1265&NodeDesc=------%20All%20District%20Courts%20------]]></key>");
                    sb.AppendLine("      <key name=~hlinkUri--inactive~><![CDATA[http://justice1.dentoncounty.com/PublicAccessDC/]]></key>");
                    sb.AppendLine("      <key name=~dateRangeMaxDays~><![CDATA[5]]></key>");
                    sb.AppendLine("      <key name=~startDate~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~endDate~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~isCriminalSearch~><![CDATA[0]]></key>");
                    sb.AppendLine("      <key name=~criminalCaseInclusion~><![CDATA[0]]></key>");
                    sb.AppendLine("      <key name=~criminalLinkQuery~><![CDATA[//a[text()='JP & County Court: Criminal Case Records']]]></key>");
                    sb.AppendLine("      <key name=~nonCriminalLinkQuery~><![CDATA[//a[text()='JP & County Court: Civil, Family & Probate Case Records']]]></key>");
                    sb.AppendLine("    </keys>");
                    sb.AppendLine("    <keys id=~10~ name=~Tarrant County~>");
                    sb.AppendLine("      <key name=~baseUri~><![CDATA[https://odyssey.tarrantcounty.com/PublicAccess/default.aspx]]></key>");
                    sb.AppendLine("      <key name=~query~><![CDATA[ID=200&NodeID=1251,1252,1253,1254,1255,1256,1260,1270,1280,1265&NodeDesc=------%20All%20District%20Courts%20------]]></key>");
                    sb.AppendLine("      <key name=~dateRangeMaxDays~><![CDATA[5]]></key>");
                    sb.AppendLine("      <key name=~startDate~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~endDate~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~navigation.control.file~><![CDATA[tarrantCountyMapping_1]]></key>");
                    sb.AppendLine("      <key name=~navigation.control.alternate.file~><![CDATA[tarrantCountyMapping_2]]></key>");
                    sb.AppendLine("      <key name=~hlinkUri~><![CDATA[https://odyssey.tarrantcounty.com/PublicAccess/{0}]]></key>");
                    sb.AppendLine("      <key name=~caseTypeSelectedIndex~><![CDATA[0]]></key>");
                    sb.AppendLine("      <key name=~criminalCaseInclusion~><![CDATA[1]]></key>");
                    sb.AppendLine("      <key name=~personNodeXpath~><![CDATA[//*[@id='PIr11']]]></key>");
                    sb.AppendLine("    </keys>");
                    sb.AppendLine("    <keys id=~20~ name=~Collin County~>");
                    sb.AppendLine("      <key name=~baseUri~><![CDATA[http://cijspub.co.collin.tx.us/SecurePA/login.aspx]]></key>");
                    sb.AppendLine("      <key name=~query~><![CDATA[ID=200&NodeID=1251,1252,1253,1254,1255,1256,1260,1270,1280,1265&NodeDesc=------%20All%20District%20Courts%20------]]></key>");
                    sb.AppendLine("      <key name=~dateRangeMaxDays~><![CDATA[5]]></key>");
                    sb.AppendLine("      <key name=~startDate~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~endDate~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~navigation.control.file~><![CDATA[collinCountyMapping_1]]></key>");
                    sb.AppendLine("      <key name=~hlinkUri~><![CDATA[http://cijspub.co.collin.tx.us/SecurePA/{0}]]></key>");
                    sb.AppendLine("      <key name=~personNodeXpath~><![CDATA[//*[@id='PIr11']]]></key>");
                    sb.AppendLine("      <key name=~personParentXath~><![CDATA[./..]]></key>");
                    sb.AppendLine("      <key name=~childTdXath~><![CDATA[/html/body/table[4]/tbody/tr[{0}]/td]]></key>");
                    sb.AppendLine("      <key name=~caseTypeSelectedIndex~><![CDATA[0]]></key>");
                    sb.AppendLine("      <key name=~searchTypeSelectedIndex~><![CDATA[0]]></key>");
                    sb.AppendLine("      <key name=~criminalCaseInclusion~><![CDATA[1]]></key>");
                    sb.AppendLine("    </keys>");
                    sb.AppendLine("    <keys id=~30~ name=~Harris County Civil~>");
                    sb.AppendLine("      <key name=~baseUri~>");
                    sb.AppendLine("        <![CDATA[https://www.cclerk.hctx.net/Applications/WebSearch/CourtSearch.aspx?CaseType=Civil]]></key>");
                    sb.AppendLine("      <key name=~query~><![CDATA[ID=200&NodeID=1251,1252,1253,1254,1255,1256,1260,1270,1280,1265&NodeDesc=------%20All%20District%20Courts%20------]]></key>");
                    sb.AppendLine("      <key name=~dateRangeMaxDays~><![CDATA[5]]></key>");
                    sb.AppendLine("      <key name=~startDate~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~endDate~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~courtIndex~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~caseStatusIndex~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~navigation.control.file~><![CDATA[harrisCivilMapping]]></key>");
                    sb.AppendLine("      <key name=~hlinkUri~><![CDATA[https://www.cclerk.hctx.net/Applications/WebSearch/CourtSearch.aspx?CaseType=Civil]]></key>");
                    sb.AppendLine("      <key name=~personNodeXpath~><![CDATA[//*[@id='PIr11']]]></key>");
                    sb.AppendLine("      <key name=~personParentXath~><![CDATA[./..]]></key>");
                    sb.AppendLine("      <key name=~childTdXath~><![CDATA[/html/body/table[4]/tbody/tr[{0}]/td]]></key>");
                    sb.AppendLine("      <key name=~caseTypeSelectedIndex~><![CDATA[0]]></key>");
                    sb.AppendLine("      <key name=~searchTypeSelectedIndex~><![CDATA[0]]></key>");
                    sb.AppendLine("      <key name=~criminalCaseInclusion~><![CDATA[1]]></key>");
                    sb.AppendLine("    </keys>");
                    sb.AppendLine("    <keys id=~40~ name=~Harris County Criminal~>");
                    sb.AppendLine("      <key name=~baseUri~>");
                    sb.AppendLine("        <![CDATA[https://www.cclerk.hctx.net/Applications/WebSearch/CourtSearch.aspx?CaseType=Civil]]>");
                    sb.AppendLine("      </key>");
                    sb.AppendLine("      <key name=~query~><![CDATA[ID=200&NodeID=1251,1252,1253,1254,1255,1256,1260,1270,1280,1265&NodeDesc=------%20All%20District%20Courts%20------]]></key>");
                    sb.AppendLine("      <key name=~dateRangeMaxDays~><![CDATA[7]]></key>");
                    sb.AppendLine("      <key name=~startDate~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~endDate~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~courtIndex~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~caseStatusIndex~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~navigation.control.file~><![CDATA[harrisCivilMapping]]></key>");
                    sb.AppendLine("      <key name=~hlinkUri~><![CDATA[https://www.cclerk.hctx.net/Applications/WebSearch/CourtSearch.aspx?CaseType=Civil]]></key>");
                    sb.AppendLine("      <key name=~personNodeXpath~><![CDATA[//*[@id='PIr11']]]></key>");
                    sb.AppendLine("      <key name=~personParentXath~><![CDATA[./..]]></key>");
                    sb.AppendLine("      <key name=~childTdXath~><![CDATA[/html/body/table[4]/tbody/tr[{0}]/td]]></key>");
                    sb.AppendLine("      <key name=~caseTypeSelectedIndex~><![CDATA[0]]></key>");
                    sb.AppendLine("      <key name=~searchTypeSelectedIndex~><![CDATA[0]]></key>");
                    sb.AppendLine("      <key name=~criminalCaseInclusion~><![CDATA[1]]></key>");
                    sb.AppendLine("    </keys>");
                    sb.AppendLine("  </setting>");
                    sb.AppendLine("  <directions>");
                    sb.AppendLine("    <instructions id=~1~>");
                    sb.AppendLine("      <command type=~GetCases~ name=~WaitForElementExist~ By=~Id~ FriendlyName=~Court-Type-Selection~><![CDATA[sbxControlID2]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~SetComboIndex~ By=~Id~ FriendlyName=~Court-Type-Selection~><![CDATA[sbxControlID2,?SetComboIndex]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~WaitForElementExist~ By=~XPath~ FriendlyName=~Search-Hyperlink~><![CDATA[//a[@class='ssSearchHyperlink'][contains(text(),'County Court: Civil, Family')]]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~Click~ By=~XPath~ FriendlyName=~Search-Hyperlink~><![CDATA[//a[@class='ssSearchHyperlink'][contains(text(),'County Court: Civil, Family')]]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~WaitForNavigation~ By=~~ FriendlyName=~~><![CDATA[ ]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~WaitForElementExist~ By=~Id~ FriendlyName=~District-Type-Selection~><![CDATA[sbxControlID2]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~SetComboIndex~ By=~Id~ FriendlyName=~District-Type-Selection~><![CDATA[sbxControlID2,?SetComboIndex]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~WaitForElementExist~ By=~XPath~ FriendlyName=~District-Hyperlink~><![CDATA[//a[@class='ssSearchHyperlink'][contains(text(),'District Clerk Civil')]]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~Click~ By=~XPath~ FriendlyName=~District-Hyperlink~><![CDATA[//a[@class='ssSearchHyperlink'][contains(text(),'District Clerk Civil')]]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~Click~ By=~Id~ FriendlyName=~Date-Filed-Radio-Button~><![CDATA[DateFiled]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~SetControlValue~ By=~Id~ FriendlyName=~Date-Filed-On-TextBox~><![CDATA[DateFiledOnAfter,?StartDate]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~SetControlValue~ By=~Id~ FriendlyName=~Date-Filed-Before-TextBox~><![CDATA[DateFiledOnBefore,?EndingDate]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~ClickElement~ By=~Id~ FriendlyName=~Search-Records-Submit~><![CDATA[SearchSubmit]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~WaitForNavigation~ By=~~ FriendlyName=~~><![CDATA[ ]]></command>");
                    sb.AppendLine("      <command type=~GetCases~ name=~GetElement~ By=~XPath~ FriendlyName=~Search-Records-Result-Table~><![CDATA[/html/body/table[4]]]></command>");
                    sb.AppendLine("    </instructions>");
                    sb.AppendLine("    <caseInspection id=~1~ type=~normal~>");
                    sb.AppendLine("      <command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[3]/div[1]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[1]/a]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[3]/div[2]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[4]/div[1]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[2]]]></command>");
                    sb.AppendLine("    </caseInspection>");
                    sb.AppendLine("    <caseInspection id=~10~ type=~normal~>");
                    sb.AppendLine("      <command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[4]/div[1]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[1]/a]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[4]/div[2]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[5]/div[1]]]></command>");
                    sb.AppendLine("    </caseInspection>");
                    sb.AppendLine("    <caseInspection id=~10~ type=~probate~>");
                    sb.AppendLine("      <command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[3]/div[1]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[1]/a]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[3]/div[2]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[4]/div[1]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Style-Selection~><![CDATA[td[2]]]></command>");
                    sb.AppendLine("    </caseInspection>");
                    sb.AppendLine("    <caseInspection id=~20~ type=~normal~>");
                    sb.AppendLine("      <command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[4]/div[1]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[1]/a]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[4]/div[2]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[5]/div[1]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Style-Selection~><![CDATA[td[3]/div[1]]]></command>");
                    sb.AppendLine("    </caseInspection>");
                    sb.AppendLine("    <caseInspection id=~20~ type=~probate~>");
                    sb.AppendLine("      <command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[3]/div[1]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[1]/a]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[3]/div[2]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[4]/div[1]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Style-Selection~><![CDATA[td[2]]]></command>");
                    sb.AppendLine("    </caseInspection>");
                    sb.AppendLine("    <caseInspection id=~20~ type=~justice~>");
                    sb.AppendLine("      <command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[3]/div[1]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[1]/a]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[3]/div[2]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[4]/div[1]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Style-Selection~><![CDATA[td[2]]]></command>");
                    sb.AppendLine("    </caseInspection>");
                    sb.AppendLine("    <caseInspection id=~30~ type=~normal~>");
                    sb.AppendLine("      <command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[2]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[0]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[3]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[4]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[1]]]></command>");
                    sb.AppendLine("    </caseInspection>");
                    sb.AppendLine("    <caseInspection id=~40~ type=~normal~>");
                    sb.AppendLine("      <command type=~string~ name=~DateFiled~ By=~XPath~ FriendlyName=~Filing-Date-Selection~><![CDATA[td[2]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Case~ By=~XPath~ FriendlyName=~Case-Number-Selection~><![CDATA[td[0]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~Court~ By=~XPath~ FriendlyName=~Court-Name-Selection~><![CDATA[td[3]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseType~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[4]]]></command>");
                    sb.AppendLine("      <command type=~string~ name=~CaseStyle~ By=~XPath~ FriendlyName=~Case-Type-Selection~><![CDATA[td[1]]]></command>");
                    sb.AppendLine("    </caseInspection>");
                    sb.AppendLine("  </directions>");
                    sb.AppendLine("  <layouts>");
                    sb.AppendLine("    <layout id=~1~ name=~caselayout~>");
                    sb.AppendLine("      <columns>");
                    sb.AppendLine("        <column name=~Case Number~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Name~ columnWidth=~80~/>");
                    sb.AppendLine("        <column name=~Filed/Location/Judical Officer~ columnWidth=~40~/>");
                    sb.AppendLine("        <column name=~Type/Status~ columnWidth=~40~/>");
                    sb.AppendLine("        <column name=~Style~ columnWidth=~80~/>");
                    sb.AppendLine("      </columns>");
                    sb.AppendLine("    </layout>");
                    sb.AppendLine("    <layout id=~1~ name=~people~>");
                    sb.AppendLine("      <columns>");
                    sb.AppendLine("        <column name=~Name~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~FirstName~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~LastName~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~Zip~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~Address1~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Address2~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Address3~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~CaseNumber~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Date Filed~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Court~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Type~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Style~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Plantiff~ columnWidth=~30~/>");
                    sb.AppendLine("      </columns>");
                    sb.AppendLine("    </layout>");
                    sb.AppendLine("    <layout id=~10~ name=~caselayout~>");
                    sb.AppendLine("      <columns>");
                    sb.AppendLine("        <column name=~Case Number~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Name~ columnWidth=~80~/>");
                    sb.AppendLine("        <column name=~Filed/Location/Judical Officer~ columnWidth=~40~/>");
                    sb.AppendLine("        <column name=~Type/Status~ columnWidth=~40~/>");
                    sb.AppendLine("        <column name=~Style~ columnWidth=~80~/>");
                    sb.AppendLine("      </columns>");
                    sb.AppendLine("    </layout>");
                    sb.AppendLine("    <layout id=~10~ name=~people~>");
                    sb.AppendLine("      <columns>");
                    sb.AppendLine("        <column name=~Name~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~FirstName~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~LastName~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~Zip~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~Address1~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Address2~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Address3~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~CaseNumber~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Style~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Date Filed~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Court~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Type~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Style~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Plantiff~ columnWidth=~30~/>");
                    sb.AppendLine("      </columns>");
                    sb.AppendLine("    </layout>");
                    sb.AppendLine("    <layout id=~20~ name=~caselayout~>");
                    sb.AppendLine("      <columns>");
                    sb.AppendLine("        <column name=~Case Number~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Name~ columnWidth=~80~/>");
                    sb.AppendLine("        <column name=~Filed/Location/Judical Officer~ columnWidth=~40~/>");
                    sb.AppendLine("        <column name=~Type/Status~ columnWidth=~40~/>");
                    sb.AppendLine("        <column name=~Style~ columnWidth=~80~/>");
                    sb.AppendLine("      </columns>");
                    sb.AppendLine("    </layout>");
                    sb.AppendLine("    <layout id=~20~ name=~people~>");
                    sb.AppendLine("      <columns>");
                    sb.AppendLine("        <column name=~Name~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~FirstName~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~LastName~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~Zip~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~Address1~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Address2~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Address3~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~CaseNumber~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Date Filed~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Court~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Type~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Style~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Plantiff~ columnWidth=~30~/>");
                    sb.AppendLine("      </columns>");
                    sb.AppendLine("    </layout>");
                    sb.AppendLine("    <layout id=~30~ name=~caselayout~>");
                    sb.AppendLine("      <columns>");
                    sb.AppendLine("        <column name=~Case Number~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Style~ columnWidth=~80~/>");
                    sb.AppendLine("        <column name=~Filed Date~ columnWidth=~40~/>");
                    sb.AppendLine("        <column name=~Court~ columnWidth=~15~/>");
                    sb.AppendLine("        <column name=~Case Type~ columnWidth=~80~/>");
                    sb.AppendLine("        <column name=~Status~ columnWidth=~20~/>");
                    sb.AppendLine("      </columns>");
                    sb.AppendLine("    </layout>");
                    sb.AppendLine("    <layout id=~30~ name=~people~>");
                    sb.AppendLine("      <columns>");
                    sb.AppendLine("        <column name=~Name~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~FirstName~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~LastName~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~Zip~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~Address1~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Address2~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Address3~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~CaseNumber~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Date Filed~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Court~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Type~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Style~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Plantiff~ columnWidth=~30~/>");
                    sb.AppendLine("      </columns>");
                    sb.AppendLine("    </layout>");
                    sb.AppendLine("    <layout id=~40~ name=~caselayout~>");
                    sb.AppendLine("      <columns>");
                    sb.AppendLine("        <column name=~Case Number~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Style~ columnWidth=~80~/>");
                    sb.AppendLine("        <column name=~Filed Date~ columnWidth=~40~/>");
                    sb.AppendLine("        <column name=~Court~ columnWidth=~15~/>");
                    sb.AppendLine("        <column name=~Case Type~ columnWidth=~80~/>");
                    sb.AppendLine("        <column name=~Status~ columnWidth=~20~/>");
                    sb.AppendLine("      </columns>");
                    sb.AppendLine("    </layout>");
                    sb.AppendLine("    <layout id=~40~ name=~people~>");
                    sb.AppendLine("      <columns>");
                    sb.AppendLine("        <column name=~Name~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~FirstName~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~LastName~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~Zip~ columnWidth=~20~/>");
                    sb.AppendLine("        <column name=~Address1~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Address2~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Address3~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~CaseNumber~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Date Filed~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Court~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Type~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Case Style~ columnWidth=~30~/>");
                    sb.AppendLine("        <column name=~Plantiff~ columnWidth=~30~/>");
                    sb.AppendLine("      </columns>");
                    sb.AppendLine("    </layout>");
                    sb.AppendLine("  </layouts>");
                    sb.AppendLine("  <inactive>");
                    sb.AppendLine("    <!-- ");
                    sb.AppendLine("    <keys id=~30~ name=~Dallas County~>");
                    sb.AppendLine("      <key name=~baseUri~><![CDATA[https://courtsportal.dallascounty.org/DALLASPROD/Home/Dashboard/26]]></key>");
                    sb.AppendLine("      <key name=~query~><![CDATA[ID=200&NodeID=1251,1252,1253,1254,1255,1256,1260,1270,1280,1265&NodeDesc=%20All%20District%20Courts%20]]></key>");
                    sb.AppendLine("      <key name=~hlinkUri-inactive~><![CDATA[http://justice1.dentoncounty.com/PublicAccessDC/]]></key>");
                    sb.AppendLine("      <key name=~dateRangeMaxDays~><![CDATA[31]]></key>");
                    sb.AppendLine("      <key name=~startDate~><![CDATA[]]></key>");
                    sb.AppendLine("      <key name=~endDate~><![CDATA[]]></key>");
                    sb.AppendLine("    </keys>");
                    sb.AppendLine("    -->");
                    sb.AppendLine("  </inactive>");
                    sb.AppendLine("</settings>");
                    sb.Replace(tilde, quote);

                    return sb.ToString();

                case "caselayout.xml":
                    StringBuilder sbb = new();
                    sbb.AppendLine("<?xml version=~1.0~ encoding=~utf-8~?>");
                    sbb.AppendLine("<search>");
                    sbb.AppendLine("  <parameters>");
                    sbb.AppendLine("    <parameter name=~Website~/>");
                    sbb.AppendLine("    <parameter name=~StartDate~/>");
                    sbb.AppendLine("    <parameter name=~EndDate~/>");
                    sbb.AppendLine("    <parameter name=~SearchDate~/>");
                    sbb.AppendLine("    <parameter name=~SearchComplete~>false</parameter>");
                    sbb.AppendLine("  </parameters>");
                    sbb.AppendLine("  <!-- Name	Zip	Address1	Address2	Address3 -->");
                    sbb.AppendLine("  <results>");
                    sbb.AppendLine("    <result name=~casedata~>");
                    sbb.AppendLine("      <![CDATA[");
                    sbb.AppendLine("      ]]>");
                    sbb.AppendLine("    </result>");
                    sbb.AppendLine("    <result name=~peopledata~>");
                    sbb.AppendLine("      <![CDATA[");
                    sbb.AppendLine("      ]]>");
                    sbb.AppendLine("    </result>");
                    sbb.AppendLine("    <result name=~person~>");
                    sbb.AppendLine("      <people>");
                    sbb.AppendLine("        <person>");
                    sbb.AppendLine("          <name/>");
                    sbb.AppendLine("          <address>");
                    sbb.AppendLine("            <![CDATA[]]>");
                    sbb.AppendLine("            <addressA/>");
                    sbb.AppendLine("            <addressB/>");
                    sbb.AppendLine("            <addressC/>");
                    sbb.AppendLine("            <zip/>");
                    sbb.AppendLine("          </address>");
                    sbb.AppendLine("          <case>");
                    sbb.AppendLine("            <![CDATA[]]>");
                    sbb.AppendLine("          </case>");
                    sbb.AppendLine("          <dateFiled>");
                    sbb.AppendLine("            <![CDATA[]]>");
                    sbb.AppendLine("          </dateFiled>");
                    sbb.AppendLine("          <court>");
                    sbb.AppendLine("            <![CDATA[]]>");
                    sbb.AppendLine("          </court>");
                    sbb.AppendLine("          <caseType>");
                    sbb.AppendLine("            <![CDATA[]]>");
                    sbb.AppendLine("          </caseType>");
                    sbb.AppendLine("          <caseStyle>");
                    sbb.AppendLine("            <![CDATA[]]>");
                    sbb.AppendLine("          </caseStyle>");
                    sbb.AppendLine("        </person>");
                    sbb.AppendLine("      </people>");
                    sbb.AppendLine("    </result>");
                    sbb.AppendLine("  </results>");
                    sbb.AppendLine("</search>");
                    sbb.Replace(tilde, quote);

                    return sbb.ToString();

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Generates a unique file name for reasrch results file.
        /// </summary>
        /// <param name="settingFile">The setting file.</param>
        /// <returns></returns>
        private static string GetFileName(WebInteractive settingFile)
        {
            // settingFile.StartDate.ToString("MMddyyyy")
            const string dfmt = "MMddyyyy";
            string dteStart = settingFile.StartDate.ToString(dfmt, CultureInfo.CurrentCulture.DateTimeFormat);
            string dteEnding = settingFile.EndingDate.ToString(dfmt, CultureInfo.CurrentCulture.DateTimeFormat);
            string fileName = string.Format(
                CultureInfo.CurrentCulture,
                "data_rqst_{2}_{0}_{1}.xml",
                dteStart,
                dteEnding,
                settingFile.Parameters.Name.Replace(" ", "").ToLower(CultureInfo.CurrentCulture));
            string? execName = new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            string targetPath = new Uri(string.Format(
                CultureInfo.CurrentCulture,
                @"{0}\xml\", execName)).AbsolutePath;
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            targetPath = string.Format(
                CultureInfo.CurrentCulture,
                @"{0}\data\", targetPath);
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            string targetFile = new Uri(string.Format(
                CultureInfo.CurrentCulture,
                @"{0}\xml\data\{1}", execName, fileName)).AbsolutePath;
            return targetFile;
        }

        /// <summary>
        /// Gets the name of the excel file.
        /// </summary>
        /// <returns></returns>
        private static string GetExcelFileName()
        {
            string? execName = new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath;
            execName = Path.GetDirectoryName(execName);
            string targetFile = new Uri(string.Format(
                CultureInfo.CurrentCulture,
                @"{0}\Utilities\CourtRecordSearch.xlsm", execName)).AbsolutePath;
            return targetFile;
        }

        /// <summary>
        /// Gets the name of the application directory.
        /// </summary>
        /// <returns></returns>
        public static string GetAppFolderName()
        {
            string execName = new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath;
            return Path.GetDirectoryName(execName) ?? string.Empty;
        }

        public static List<WebNavInstruction> GetInstructions(int siteId)
        {
            List<WebNavInstruction> instructions = new();
            string content = Content;
            XmlDocument doc = XmlDocProvider.GetDoc(content);

            string qpath = string.Format(
                CultureInfo.CurrentCulture,
                "directions/instructions[@id={0}]", siteId);
            XmlNode? data = doc.DocumentElement?.SelectSingleNode(qpath);
            if (data == null)
            {
                return instructions;
            }

            foreach (object? item in data.ChildNodes)
            {
                XmlNode nde = (XmlNode)item;
                instructions.Add(new WebNavInstruction
                {
                    Name = nde.Attributes?.GetNamedItem("name")?.Value ?? string.Empty,
                    By = nde.Attributes?.GetNamedItem("By")?.Value ?? string.Empty,
                    CommandType = nde.Attributes?.GetNamedItem("type")?.Value ?? string.Empty,
                    FriendlyName = nde.Attributes?.GetNamedItem("FriendlyName")?.Value ?? string.Empty,
                    Value = ((XmlCDataSection)nde.FirstChild!).Data
                });
            }
            return instructions;
        }
    }
}