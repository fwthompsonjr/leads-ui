using legallead.records.search.Models;
using System.Text;
using System.Xml;

namespace legallead.records.search.Classes
{
    public class XmlContentHolder
    {
        public string FileName { get; set; } = string.Empty;

        public XmlDocument Document { get; set; } = new XmlDocument();

        public XmlNode? Data
        {
            get
            {
                if (Document == null)
                {
                    return null;
                }

                return Document.DocumentElement?.SelectSingleNode(@"//results/result[@name='casedata']");
            }
        }

        public XmlNode? PeopleData
        {
            get
            {
                if (Document == null)
                {
                    return null;
                }

                return Document.DocumentElement?.SelectSingleNode(@"//results/result[@name='peopledata']");
            }
        }

        public XmlNode? People
        {
            get
            {
                if (Document == null)
                {
                    return null;
                }

                return Document.DocumentElement?.SelectSingleNode(@"//results/result[@name='person']/people");
            }
        }

        public List<PersonAddress> GetPersonAddresses()
        {
            var people = People;
            if (people == null)
            {
                return new();
            }

            if (!people.HasChildNodes)
            {
                return new List<PersonAddress>();
            }

            List<PersonAddress> addressList = new();
            foreach (XmlNode personNode in people.ChildNodes.Cast<XmlNode>())
            {
                var addressPerson = PersonAddress.ConvertFrom(personNode);
                if (addressPerson == null)
                {
                    continue;
                }

                if (addressPerson.IsValid)
                {
                    addressList.Add(addressPerson);
                }
                else if (!string.IsNullOrEmpty(personNode.ChildNodes[0]?.InnerText))
                {
                    addressPerson = new PersonAddress
                    {
                        Name = personNode.ChildNodes[0]?.InnerText ?? string.Empty,
                        Address1 = PersonAddress.ConvertFrom("address1", personNode),
                        Address2 = PersonAddress.ConvertFrom("address2", personNode),
                        Address3 = PersonAddress.ConvertFrom("address3", personNode),
                        Zip = PersonAddress.ConvertFrom("zip", personNode),
                        CaseNumber = PersonAddress.ConvertFrom("case", personNode),
                        CaseStyle = PersonAddress.ConvertFrom("caseStyle", personNode),
                        CaseType = PersonAddress.ConvertFrom("caseType", personNode),
                        Court = PersonAddress.ConvertFrom("court", personNode),
                        DateFiled = PersonAddress.ConvertFrom("dateFiled", personNode)
                    };
                    addressList.Add(addressPerson);
                }
                if (!addressPerson.IsValid && !string.IsNullOrEmpty(addressPerson.Name))
                {
                    addressPerson.Address1 = "No Address Found";
                    addressPerson.Address3 = "Not, Available 00000";
                    addressPerson.Zip = string.Empty;
                    addressPerson.CaseNumber = PersonAddress.ConvertFrom("case", personNode);
                    addressPerson.CaseStyle = PersonAddress.ConvertFrom("caseStyle", personNode);
                    addressPerson.CaseType = PersonAddress.ConvertFrom("caseType", personNode);
                    addressPerson.Court = PersonAddress.ConvertFrom("court", personNode);
                    addressPerson.DateFiled = PersonAddress.ConvertFrom("dateFiled", personNode);
                }
            }
            return addressList;
        }

        protected StringBuilder CharacterData { get; set; } = new StringBuilder();
        protected StringBuilder CharacterPeople { get; set; } = new StringBuilder();
        public int Id { get; internal set; }

        public void Append(Models.HLinkDataRow dta)
        {
            if (dta == null)
            {
                throw new ArgumentNullException(nameof(dta));
            }

            CharacterData ??= new StringBuilder("");

            CharacterPeople ??= new StringBuilder("");

            string openTable = @"<table style='border-collapse: collapse; border: 1px solid black;'>";
            string opnTable = openTable + @"<tr style='border: 1px solid black;'>";
            opnTable += Environment.NewLine;
            opnTable += @"<td colspan='2' style='border: 1px solid black;'>Case Results</td></tr>";
            opnTable += Environment.NewLine;
            const string closeTable = @"</table>";
            const string fmttbl = @"{0}{1}{2}";
            CleanHtml(dta.Data);
            CharacterData.AppendLine(dta.Data);
            if (Data is not XmlCDataSection cnode) return;
            cnode.Data = string.Format(
                CultureInfo.CurrentCulture,
                fmttbl,
                openTable,
                CharacterData.ToString(),
                closeTable);
            var person = People?.FirstChild?.CloneNode(true);
            SetChildNodeText(person, dta.Defendant);
            XmlNode? addressNode = person?.ChildNodes[1];
            if (addressNode?.FirstChild is XmlCDataSection section)
            {
                section.Data = dta.Address;
            }
            if (person == null || addressNode == null) return;
            ParseAddressInformation(dta.Address, addressNode);
            person = MapExtraData(dta, person);
            People?.AppendChild(person);

            string personHtml = PersonHtml(dta);
            if (!string.IsNullOrEmpty(personHtml))
            {
                personHtml = personHtml.Replace(@"<tr>", @"<tr style='border: 1px solid black;'>");
                CharacterPeople.AppendLine(personHtml);
                if (PeopleData?.FirstChild is XmlCDataSection cdat)
                {
                    cdat.Data = string.Format(
                    CultureInfo.CurrentCulture,
                    fmttbl,
                    opnTable,
                    CharacterPeople.ToString(),
                    closeTable);
                }
            }
            // this should save document to resource service
            var html = Document.OuterXml;
            var expiration = TimeSpan.FromMinutes(15);
            ResourceFileService.AddOrUpdate(FileName, html, expiration);
        }

        private static void SetChildNodeText(XmlNode? node, string text)
        {
            if (node == null || node.ChildNodes[0] is not XmlNode child) { return; }
            child.InnerText = text;
        }

        private static void ParseAddressInformation(string address, XmlNode addressNode)
        {
            const string lineBreak = @"<br/>";
            if (addressNode == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(address))
            {
                return;
            }

            address = address.Trim();
            if (string.IsNullOrEmpty(address))
            {
                return;
            }

            List<string> addresses = address.Split(new string[] { lineBreak }, StringSplitOptions.None).ToList();
            if (addresses.Count <= 1 || addresses.Count > 4)
            {
                return;
            }

            XmlNode? zipNode = addressNode.SelectSingleNode("zip");
            XmlNode? addressA = addressNode.SelectSingleNode("addressA");
            XmlNode? addressB = addressNode.SelectSingleNode("addressB");
            XmlNode? addressC = addressNode.SelectSingleNode("addressC");
            // append a cdata section
            List<XmlNode?> ndes = new() { zipNode, addressA, addressB, addressC };
            foreach (XmlNode? item in ndes)
            {
                if (item == null || item.OwnerDocument == null) { continue; }
                XmlCDataSection cdta = item.OwnerDocument.CreateCDataSection(string.Empty);
                item.AppendChild(cdta);
            }
            addresses = RemoveBlank(addresses);
            string firstItem = addresses[0].Trim().ToUpper(CultureInfo.CurrentCulture);
            string lastItem = addresses[^1].Trim().ToUpper(CultureInfo.CurrentCulture);
            string middleItem = GetMiddleAddress(addresses).Trim().ToUpper(CultureInfo.CurrentCulture);
            SetCDataValue(zipNode, ParseZipCode(lastItem));
            SetCDataValue(addressC, lastItem);
            SetCDataValue(addressA, firstItem);
            SetCDataValue(addressB, middleItem);
        }

        private static void SetCDataValue(XmlNode? node, string value)
        {
            if (node?.FirstChild is XmlCDataSection cdata)
            {
                cdata.Data = value;
            }
        }

        private static string ParseZipCode(string lastItem)
        {
            List<string> pieces = lastItem.Split(' ').ToList();
            return pieces[^1];
        }

        private static XmlNode MapExtraData(HLinkDataRow dta, XmlNode person)
        {
            if (!dta.IsMapped)
            {
                return person;
            }

            List<string> fields = "case,dateFiled,court,caseType,caseStyle".Split(',').ToList();
            foreach (string? fieldName in fields)
            {
                XmlNode? targetNode = person.SelectSingleNode(fieldName);
                if (targetNode == null)
                {
                    continue;
                }
                SetCDataValue(targetNode, dta[fieldName]);
            }
            return person;
        }

        private static void CleanHtml(string rawHtml)
        {
            if (string.IsNullOrEmpty(rawHtml))
            {
                return;
            }

            if (!rawHtml.ToLower(CultureInfo.CurrentCulture).Contains("href="))
            {
                return;
            }

            try
            {
                XmlDocument doc = XmlDocProvider.GetDoc(rawHtml);
                XmlNode? firstChild = doc.DocumentElement?.FirstChild;
                XmlNode? hlink = firstChild?.SelectSingleNode("a");
                if (firstChild == null || hlink == null)
                {
                    return;
                }

                firstChild.InnerXml = string.Format(
                    CultureInfo.CurrentCulture,
                    "<span>{0}</span>", hlink.InnerText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string PersonHtml(Models.HLinkDataRow dta)
        {
            if (string.IsNullOrEmpty(dta.Defendant))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(dta.Address))
            {
                return string.Empty;
            }

            const string rfmt = @"<tr><td caseNumber='{2}'>{0}</td><td>{1}</tr>";
            return string.Format(
                CultureInfo.CurrentCulture,
                rfmt, dta.Defendant, dta.Address, dta.Case ?? "");
        }

        private static List<string> RemoveBlank(List<string> data)
        {
            List<string> result = new();
            foreach (string addr in data)
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
    }
}