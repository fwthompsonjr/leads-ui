using legallead.records.search.Classes;
using System.Text;

namespace legallead.records.search.Models
{
    public class WebNavigationParameter
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<WebNavigationKey> Keys { get; set; } = new();

        public List<WebNavInstruction> Instructions { get; set; } = new();

        public List<WebNavInstruction> CaseInstructions { get; set; } = new();
    }

    public class WebNavigationKey
    {
        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
    }

    public class NavInstruction
    {
        public IList<WebNavInstruction> NavInstructions { get; set; } = new List<WebNavInstruction>();
    }

    public class WebNavInstruction
    {
        public string Name { get; set; } = string.Empty;

        public string FriendlyName { get; set; } = string.Empty;

        public string By { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public string CommandType { get; set; } = string.Empty;
    }

    public class HLinkDataRow
    {
        private const System.StringComparison comparison = System.StringComparison.CurrentCultureIgnoreCase;

        public int WebsiteId { get; set; }
        public string Data { get; set; } = string.Empty;

        public string WebAddress { get; set; } = string.Empty;

        public string Defendant { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public bool IsCriminal { get; set; }
        public bool IsMapped { get; set; }
        public string Case { get; set; } = string.Empty;
        public string DateFiled { get; set; } = string.Empty;
        public string Court { get; set; } = string.Empty;
        public string CaseType { get; set; } = string.Empty;
        public string CaseStyle { get; set; } = string.Empty;
        public string PageHtml { get; internal set; } = string.Empty;

        public string CriminalCaseStyle { get; set; } = string.Empty;

        public bool IsProbate { get; internal set; }
        public bool IsJustice { get; internal set; }

        public string this[string fieldName]
        {
            get
            {
                if (string.IsNullOrEmpty(fieldName))
                {
                    return string.Empty;
                }

                if (fieldName.ToLower(CultureInfo.CurrentCulture).Equals("case", comparison))
                {
                    return Case;
                }
                if (fieldName.ToLower(CultureInfo.CurrentCulture).Equals("datefiled", comparison))
                {
                    return DateFiled;
                }
                if (fieldName.ToLower(CultureInfo.CurrentCulture).Equals("court", comparison))
                {
                    return Court;
                }
                if (fieldName.ToLower(CultureInfo.CurrentCulture).Equals("casetype", comparison))
                {
                    return CaseType;
                }
                if (fieldName.ToLower(CultureInfo.CurrentCulture).Equals("casestyle", comparison))
                {
                    return CaseStyle ??= GetFromData();
                }
                return string.Empty;
            }
            set
            {
                if (string.IsNullOrEmpty(fieldName))
                {
                    return;
                }

                if (fieldName.ToLower(CultureInfo.CurrentCulture).Equals("case", comparison))
                {
                    Case = value;
                    return;
                }
                if (fieldName.ToLower(CultureInfo.CurrentCulture).Equals("datefiled", comparison))
                {
                    DateFiled = value;
                    return;
                }
                if (fieldName.ToLower(CultureInfo.CurrentCulture).Equals("court", comparison))
                {
                    Court = value;
                    return;
                }
                if (fieldName.ToLower(CultureInfo.CurrentCulture).Equals("casetype", comparison))
                {
                    CaseType = value;
                    return;
                }
                if (fieldName.ToLower(CultureInfo.CurrentCulture).Equals("casestyle", comparison))
                {
                    CaseStyle = value;
                }
            }
        }

        private string GetFromData()
        {
            if (string.IsNullOrEmpty(Data))
            {
                return string.Empty;
            }

            WebInteractive webHelper = new();
            string data = new StringBuilder(Data).ToString();
            if (data.Contains("<img"))
            {
                data = webHelper.RemoveElement(data, "<img");
                Data = data;
            }
            System.Xml.XmlDocument doc = XmlDocProvider.GetDoc(data);
            System.Xml.XmlNode? node = doc.FirstChild?.ChildNodes[1];
            if (node == null)
            {
                return string.Empty;
            }

            return node.InnerText;
        }
    }
}