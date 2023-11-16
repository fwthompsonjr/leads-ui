using legallead.records.search.Classes;
using System.Text;

namespace legallead.records.search.Models
{
    public class WebNavigationParameter
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<WebNavigationKey> Keys { get; set; }

        public List<WebNavInstruction> Instructions { get; set; }

        public List<WebNavInstruction> CaseInstructions { get; set; }
    }

    public class WebNavigationKey
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class NavInstruction
    {
        public IList<WebNavInstruction> NavInstructions { get; set; }
    }

    public class WebNavInstruction
    {
        public string Name { get; set; }

        public string FriendlyName { get; set; }

        public string By { get; set; }

        public string Value { get; set; }

        public string CommandType { get; set; }
    }

    public class HLinkDataRow
    {
        private const System.StringComparison comparison = System.StringComparison.CurrentCultureIgnoreCase;

        public int WebsiteId { get; set; }
        public string Data { get; set; }

        public string WebAddress { get; set; }

        public string Defendant { get; set; }

        public string Address { get; set; }

        public bool IsCriminal { get; set; }
        public bool IsMapped { get; set; }
        public string Case { get; set; }
        public string DateFiled { get; set; }
        public string Court { get; set; }
        public string CaseType { get; set; }
        public string CaseStyle { get; set; }
        public string PageHtml { get; internal set; }

        public string CriminalCaseStyle
        { get; set; }

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
                    return;
                }
                /* set the specified index to value here */
            }
        }

        private string GetFromData()
        {
            if (string.IsNullOrEmpty(Data))
            {
                return null;
            }

            WebInteractive webHelper = new();
            string data = new StringBuilder(Data).ToString();
            if (data.Contains("<img"))
            {
                data = webHelper.RemoveElement(data, "<img");
                Data = data;
            }
            System.Xml.XmlDocument doc = XmlDocProvider.GetDoc(data);
            System.Xml.XmlNode? node = doc.FirstChild.ChildNodes[1];
            if (node == null)
            {
                return string.Empty;
            }

            return node.InnerText;
        }
    }
}