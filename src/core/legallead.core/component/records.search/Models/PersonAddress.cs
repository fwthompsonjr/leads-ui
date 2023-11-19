using legallead.records.search.Parsing;
using System.Xml;

namespace legallead.records.search.Models
{
    public class PersonAddress
    {
        #region FieldList Helpers

        private const string FieldNames = @"Name,FirstName,LastName,Zip," +
            @"Address1,Address2,Address3," +
            @"Case Number,Date Filed,Court," +
            @"Case Type,case style,plantiff,FName,LName";

        private string? _fieldNames;
        private List<string>? _fieldList;

        protected string LoweredFieldNames
        {
            get
            {
                return _fieldNames ??= FieldNames.ToLower(CultureInfo.CurrentCulture);
            }
        }

        public List<string> FieldList
        {
            get
            {
                return _fieldList ??= LoweredFieldNames.Split(',').ToList();
            }
        }

        #endregion FieldList Helpers

        #region Parse Plantiff Helpers

        private List<ICaseDataParser> GetCaseDataParses()
        {
            string caseType = string.IsNullOrEmpty(CaseType) ?
                string.Empty : CaseType.Trim();
            string caseStyle = string.IsNullOrEmpty(CaseStyle) ?
                string.Empty : CaseStyle.Trim();
            return new List<ICaseDataParser>
            {
                new ParseCaseDataByVersusStrategy{ CaseData = caseStyle },
                new ParseCaseDataByVsStrategy{ CaseData = caseStyle },
                new ParseCaseDataByEstateStrategy{ CaseData = caseStyle },
                new ParseCaseDataByInTheEstateStrategy{ CaseData = caseStyle },
                new ParseCaseOrderForForeclosure { CaseData = caseStyle },
                new ParseCaseInTheMatterMarriage { CaseData = caseStyle },
                new ParseCaseByVDot { CaseData = caseStyle },
                new ParseCaseNameChange { CaseData = caseStyle },
                new ParseMatterOfNameChange { CaseData = caseStyle },
                new ParseCaseInInterestMatch { CaseData = caseStyle },
                new ParseCaseExParteMatch { CaseData = caseStyle },
                new ParseNameChangeByCaseType { CaseData =
                string.Format(CultureInfo.CurrentCulture, "{0}| {1}", caseType, caseStyle) },
                new ParseProtectiveOrderCaseType { CaseData =
                string.Format(CultureInfo.CurrentCulture, "{0}| {1}", caseType, caseStyle) },
                new ParseCaseDataByInGuardianshipStrategy{ CaseData = caseStyle}
            };
        }

        #endregion Parse Plantiff Helpers

        #region Properties

        public string Name { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Address3 { get; set; } = string.Empty;
        public string CaseNumber { get; set; } = string.Empty;
        public string DateFiled { get; set; } = string.Empty;
        public string Court { get; set; } = string.Empty;
        public string CaseType { get; set; } = string.Empty;
        public string CaseStyle { get; set; } = string.Empty;

        public string FirstName
        {
            get
            {
                return ParseFromFullName(0);
            }
        }

        public string LastName
        {
            get
            {
                return ParseFromFullName(1);
            }
        }
        private string _plantiff = string.Empty;
        public string Plantiff
        {
            get
            {
                _plantiff = ParseFromCaseStyle(0);
                return _plantiff;
            }
            set { _plantiff = value; }
        }

        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(Zip))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(Address1))
                {
                    return false;
                }

                return true;
            }
        }

        public string Status { get; set; } = string.Empty;
        public string CalcFirstName { get; set; } = string.Empty;
        public string CalcLastName { get; set; } = string.Empty;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Parses from full name.
        /// </summary>
        /// <param name="postionId">The postion identifier.</param>
        /// <returns></returns>
        private string ParseFromFullName(int postionId)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return string.Empty;
            }

            string fullName = Name;
            if (!fullName.Contains(','))
            {
                return postionId == 0 ? fullName : string.Empty;
            }

            string[] nameParts = fullName.Split(',');
            string lastName = nameParts[0];
            string findIt = string.Format(CultureInfo.CurrentCulture, "{0}{1}", lastName, ',');
            string firstName = fullName.Remove(0, findIt.Length).Trim();
            if (firstName.Contains(' '))
            {
                nameParts = firstName.Split(' ');
                firstName = nameParts[0];
            }
            return postionId == 0 ? firstName : lastName;
        }

        /// <summary>
        /// Parses from case stlye.
        /// </summary>
        /// <param name="postionId">The postion identifier.</param>
        /// <returns></returns>
        private string ParseFromCaseStyle(int postionId)
        {
            if (postionId > 100)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(CaseStyle))
            {
                return string.Empty;
            }

            List<ICaseDataParser> providers = GetCaseDataParses().FindAll(x => x.CanParse());
            if (!providers.Any())
            {
                return string.Empty;
            }

            string response = string.Empty;
            foreach (ICaseDataParser parser in providers)
            {
                response = parser.Parse().Plantiff;
                if (!string.IsNullOrEmpty(response))
                {
                    return response;
                }
            }
            return providers[0].Parse().Plantiff;
        }

        #endregion Methods

        #region Indexer

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified index name.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="indexName">Name of the index.</param>
        /// <returns></returns>
        public string this[string indexName]
        {
            get
            {
                if (string.IsNullOrEmpty(indexName))
                {
                    return string.Empty;
                }

                string keyName = indexName.ToLower(CultureInfo.CurrentCulture);
                if (!FieldList.Contains(keyName))
                {
                    return string.Empty;
                }

                switch (keyName)
                {
                    case "name":
                        return Name;

                    case "zip":
                        return Zip;

                    case "address1":
                        return Address1;

                    case "address2":
                        return Address2;

                    case "address3":
                        return Address3;

                    case "case number":
                        return CaseNumber;

                    case "date filed":
                        return DateFiled;

                    case "court":
                        return Court;

                    case "case type":
                        return CaseType;

                    case "case style":
                        return CaseStyle;

                    case "firstname":
                        return FirstName;

                    case "lastname":
                        return LastName;

                    case "plantiff":
                        return Plantiff;

                    case "fname":
                        return CalcFirstName ?? "";

                    case "lname":
                        return CalcLastName ?? "";

                    default:
                        return string.Empty;
                }
            }
            set
            {
                if (string.IsNullOrEmpty(indexName))
                {
                    return;
                }

                string keyName = indexName.ToLower(CultureInfo.CurrentCulture);
                if (!FieldList.Contains(keyName))
                {
                    return;
                }

                switch (keyName)
                {
                    case "name":
                        Name = value;
                        return;

                    case "zip":
                        Zip = value;
                        return;

                    case "address1":
                        Address1 = value;
                        return;

                    case "address2":
                        Address2 = value;
                        return;

                    case "address3":
                        Address3 = value;
                        return;

                    case "case number":
                        CaseNumber = value;
                        return;

                    case "date filed":
                        DateFiled = value;
                        return;

                    case "court":
                        Court = value;
                        return;

                    case "case type":
                        CaseType = value;
                        return;

                    case "case style":
                        CaseStyle = value;
                        return;

                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string this[int index]
        {
            get
            {
                if (index < 0)
                {
                    return string.Empty;
                }

                if (index > FieldList.Count - 1)
                {
                    return string.Empty;
                }

                return this[FieldList[index]];
            }
            set
            {
                if (index < 0)
                {
                    return;
                }

                if (index > FieldList.Count - 1)
                {
                    return;
                }

                this[FieldList[index]] = value;
            }
        }

        #endregion Indexer

        #region Static Methods

        private static string GetMiddleAddress(List<string> middle)
        {
            if (middle.Count < 3)
            {
                return string.Empty;
            }

            string addr = string.Empty;
            for (int i = 1; i < middle.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty(addr))
                {
                    addr += ", ";
                }
                addr += middle[i];
            }
            return addr;
        }

        public static PersonAddress? ConvertFrom(XmlNode personNode)
        {
            if (personNode == null)
            {
                return null;
            }

            PersonAddress addr = new()
            {
                Name = TryGet(personNode, "name"),
                Zip = TryGet(personNode, "address", "zip"),
                Address1 = TryGet(personNode, "address", "addressA"),
                Address2 = TryGet(personNode, "address", "addressB"),
                Address3 = TryGet(personNode, "address", "addressC"),
                CaseNumber = TryGet(personNode, "case", ""),
                DateFiled = TryGet(personNode, "dateFiled", ""),
                Court = TryGet(personNode, "court", ""),
                CaseType = TryGet(personNode, "caseType", "")
            };

            return addr;
        }

        public static string ConvertFrom(string nodeName, XmlNode personNode)
        {
            if (string.IsNullOrEmpty(nodeName))
            {
                return string.Empty;
            }

            if (personNode == null)
            {
                return string.Empty;
            }

            if (personNode.ChildNodes.Count < 2)
            {
                return string.Empty;
            }

            string lowerNodeName = nodeName.ToLower(CultureInfo.CurrentCulture);
            string addressNode = personNode.ChildNodes[1]!.InnerText.Trim();
            List<string> addressFields = "address1,address2,address3,zip".Split(',').ToList();
            if (addressFields.Contains(lowerNodeName) && string.IsNullOrEmpty(addressNode))
            {
                return string.Empty;
            }

            switch (lowerNodeName)
            {
                case "address1":
                    List<string> first = GetAddressList(addressNode);
                    return first[0];

                case "address2":
                    List<string> middle = GetAddressList(addressNode);
                    return GetMiddleAddress(middle);

                case "address3":
                    List<string> second = GetAddressList(addressNode);
                    if (second.Count < 2)
                    {
                        return string.Empty;
                    }

                    return second[^1];

                case "zip":
                    string parts = addressNode
                        .Replace("<br/>", "~")
                        .Replace("<br />", "~").Split('~')[^1];
                    return parts.Split(' ')[^1];

                case "case":
                    XmlNode? caseNode = personNode.SelectSingleNode("case");
                    if (caseNode == null)
                    {
                        return string.Empty;
                    }
                    return ((XmlCDataSection)caseNode.ChildNodes[0]!).Data;
                case "court":
                    XmlNode? courtNode = personNode.SelectSingleNode("court");
                    if (courtNode == null)
                    {
                        return string.Empty;
                    }

                    return ((XmlCDataSection)courtNode.ChildNodes[0]!).Data;

                case "datefiled":
                    XmlNode? filedNode = personNode.SelectSingleNode("dateFiled");
                    if (filedNode == null)
                    {
                        return string.Empty;
                    }

                    return ((XmlCDataSection)filedNode.ChildNodes[0]!).Data;

                case "casetype":
                    XmlNode? caseTypeNode = personNode.SelectSingleNode("caseType");
                    if (caseTypeNode == null)
                    {
                        return string.Empty;
                    }

                    return ((XmlCDataSection)caseTypeNode.ChildNodes[0]!).Data;

                case "casestyle":
                    XmlNode? caseStyleNode = personNode.SelectSingleNode("caseStyle");
                    if (caseStyleNode == null)
                    {
                        return string.Empty;
                    }

                    return ((XmlCDataSection)caseStyleNode.ChildNodes[0]!).Data;

                default:
                    break;
            }
            return string.Empty;
        }

        private static string TryGet(XmlNode personNode, string nodeName)
        {
            XmlNode? node = personNode.SelectSingleNode(nodeName);
            if (node == null)
            {
                return string.Empty;
            }

            return node.InnerText;
        }

        private static string TryGet(XmlNode personNode, string nodeName, string childNodeName)
        {
            try
            {
                XmlNode? node = personNode.SelectSingleNode(nodeName);
                if (node == null)
                {
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(childNodeName))
                {
                    return ((XmlCDataSection)node.ChildNodes[0]!).Data;
                }
                else
                {
                    if (childNodeName.StartsWith("address", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string fullAddress = ((XmlCDataSection)node.ChildNodes[0]!).Data;
                        List<string> addressList = GetAddressList(fullAddress);
                        switch (childNodeName)
                        {
                            case "addressA":
                                return addressList[0];

                            case "addressC":
                                return addressList[^1];

                            case "addressB":
                                return GetMiddleAddress(addressList);

                            default:
                                return string.Empty;
                        }
                    }
                }
                XmlNode? childNode = node.SelectSingleNode(childNodeName);
                if (childNode == null)
                {
                    return string.Empty;
                }

                if (!childNode.HasChildNodes)
                {
                    return string.Empty;
                }

                return ((XmlCDataSection)childNode.ChildNodes[0]!).Data;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static List<string> GetAddressList(string address)
        {
            List<string> result = new();
            List<string> data = address
                .Replace("<br>", "~")
                .Replace("<br/>", "~")
                .Replace("<br />", "~").Split('~').ToList();
            foreach (string? addr in data)
            {
                if (string.IsNullOrEmpty(addr))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(addr.Trim()))
                {
                    continue;
                }

                result.Add(addr.Trim());
            }
            return result;
        }

        #endregion Static Methods
    }
}