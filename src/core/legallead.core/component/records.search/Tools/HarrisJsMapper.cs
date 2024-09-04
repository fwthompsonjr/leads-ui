using legallead.records.search.Models;
using System.Xml;

namespace legallead.records.search.Tools
{
    internal static class HarrisJsMapper
    {
        public static List<PersonAddress> MapFrom(this XmlDocument document, string datatype = "civil")
        {
            var persons = new List<PersonAddress>();
            var node = document.DocumentElement?.SelectSingleNode("//Results");
            if (node == null) return persons;
            var rows = node.SelectNodes("Row")?.Cast<XmlNode>().ToList();
            if (rows == null) return persons;
            rows.ForEach(r =>
            {
                var keys = datatype == "civil" ? r.GetCivilKeys() : r.GetCriminalKeys();
                var person = new PersonAddress();
                foreach (var key in keys)
                {
                    person[key.Key] = key.Value;
                }
                person = person.ToCalculatedNames();
                persons.Add(person);
            });
            return persons;
        }

        private static Dictionary<string, string> GetCriminalKeys(this XmlNode nodeRow)
        {
            var keys = new Dictionary<string, string> {
                { "name", nodeRow.GetCriminalName() ?? "" },
                { "zip", nodeRow.SelectSingleNode("DefHomeAddZIP1")?.InnerText ?? "" },
                { "address1", nodeRow.SelectSingleNode("DefHomeAddLine1")?.InnerText ?? "" },
                { "address2", nodeRow.SelectSingleNode("DefHomeAddLine2")?.InnerText ?? "" },
                { "address3", nodeRow.GetCriminalCityAndState() },
                { "case number", nodeRow.SelectSingleNode("CaseNumber")?.InnerText ?? "" },
                { "date filed", nodeRow.SelectSingleNode("FiledDate")?.InnerText ?? "" },
                { "court", nodeRow.SelectSingleNode("JPCourtID")?.InnerText ?? "" },
                { "case type", nodeRow.SelectSingleNode("CitationNumber")?.InnerText ?? "" },
                { "case style", nodeRow.SelectSingleNode("OffenseDescription")?.InnerText ?? "" }
            };
            return keys;
        }
        private static Dictionary<string, string> GetCivilKeys(this XmlNode nodeRow)
        {
            var keys = new Dictionary<string, string> {
                { "name", nodeRow.SelectSingleNode("DefendantName")?.InnerText ?? "" },
                { "zip", nodeRow.SelectSingleNode("DefendantAddressZIP1")?.InnerText ?? "" },
                { "address1", nodeRow.SelectSingleNode("DefendantAddressLine1")?.InnerText ?? "" },
                { "address2", nodeRow.SelectSingleNode("DefendantAddressLine2")?.InnerText ?? "" },
                { "address3", nodeRow.GetCivilCityAndState() },
                { "case number", nodeRow.SelectSingleNode("CaseNumber")?.InnerText ?? "" },
                { "date filed", nodeRow.SelectSingleNode("FiledDate")?.InnerText ?? "" },
                { "court", nodeRow.SelectSingleNode("CaseNumber")?.InnerText ?? "" },
                { "case type", nodeRow.SelectSingleNode("CaseType")?.InnerText ?? "" },
                { "case style", nodeRow.SelectSingleNode("StyleOfCase")?.InnerText ?? "" }
            };
            return keys;
        }

        private static string GetCivilCityAndState(this XmlNode node)
        {
            var city = node.SelectSingleNode("DefendantAddressCity")?.InnerText ?? "";
            var st = node.SelectSingleNode("DefendantAddressState")?.InnerText ?? "";
            return $"{city}, {st}";
        }

        private static string GetCriminalName(this XmlNode node)
        {
            var first = node.SelectSingleNode("DefFirstName")?.InnerText ?? "";
            var middle = node.SelectSingleNode("DefMiddleName")?.InnerText ?? "";
            var last = node.SelectSingleNode("DefLastName")?.InnerText ?? "";
            if (!string.IsNullOrEmpty(last)) last += ",";
            var final = new List<string> { last, first, middle };
            final.RemoveAll(x => string.IsNullOrEmpty(x));
            if (!final.Any()) return string.Empty;
            return string.Join(" ", final);
        }

        private static string GetCriminalCityAndState(this XmlNode node)
        {
            var city = node.SelectSingleNode("DefHomeAddCity")?.InnerText ?? "";
            var st = node.SelectSingleNode("DefHomeAddState")?.InnerText ?? "";
            return $"{city}, {st}";
        }
    }
}
