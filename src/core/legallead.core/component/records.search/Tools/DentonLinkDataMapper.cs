using legallead.records.search.Models;

namespace legallead.records.search.Tools
{
    internal static class DentonLinkDataMapper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Cube",
            "S4158:Empty collections should not be accessed or iterated",
            Justification = "List is populated in lambda expression.")]
        public static List<PersonAddress> ConvertFrom(List<HLinkDataRow> data)
        {
            var culture = new CultureInfo("en-US");
            var list = new List<PersonAddress>();
            data.ForEach(x =>
            {
                var item = ConvertFrom(x);
                list.Add(item);
            });

            list.RemoveAll(x => string.IsNullOrEmpty(x.Name) && string.IsNullOrEmpty(x.Address1));

            list.ForEach(item =>
            {
                var isOk = HasReadableAddress(item);
                if (!isOk)
                {
                    item.Zip = CommonKeyIndexes.NonAddressZipCode;
                    item.Address1 = CommonKeyIndexes.NonAddressLine1;
                    item.Address2 = string.Empty;
                    item.Address3 = CommonKeyIndexes.NonAddressLine2;
                }
            });

            list.Sort((a, b) =>
            {
                var aa = a.CaseNumber.CompareTo(b.CaseNumber);
                if (aa != 0) return aa;
                _ = DateTime.TryParse(a.DateFiled, culture, DateTimeStyles.None, out var d1);
                _ = DateTime.TryParse(b.DateFiled, culture, DateTimeStyles.None, out var d2);
                var bb = d1.CompareTo(d2);
                if (bb != 0) return bb;
                return a.Name.CompareTo(b.Name);
            });
            return list;
        }

        private static PersonAddress ConvertFrom(HLinkDataRow row)
        {
            var person = new PersonAddress
            {
                CaseNumber = row.Case,
                CaseStyle = row.CaseStyle,
                CaseType = row.CaseType,
                Court = row.Court,
                Name = row.Defendant,
                Status = row.Status,
                Plantiff = row.Plaintiff ?? string.Empty,
            };
            if (!string.IsNullOrWhiteSpace(row.Address))
            {
                CalculateAddressLines(row, person);
            }
            return person;
        }

        private static void CalculateAddressLines(HLinkDataRow row, PersonAddress person)
        {
            var address = row.Address.Split('|');
            person.Zip = GetZipCodeOrDefault(row.Address);
            if (address.Length == 1)
            {
                person.Address1 = address[0].Trim();
            }
            if (address.Length == 2)
            {
                person.Address1 = address[0].Trim();
                person.Address2 = address[1].Trim();
            }
            if (address.Length > 2)
            {

                person.Address1 = address[0].Trim();
                person.Address2 = address[1].Trim();
                var rng = address.ToList().GetRange(2, address.Length - 2);
                person.Address3 = string.Join(" ", rng.Select(r => r.Trim()));
            }
        }

        private static string GetZipCodeOrDefault(string address)
        {
            const string fallback = "00000";
            const char space = ' ';
            const char pipe = '|';
            if (string.IsNullOrWhiteSpace(address)) return fallback;
            if (!address.Contains(space)) return fallback;
            if (!address.Contains(pipe)) return fallback;
            var item = address.Split(pipe)[^1];
            if (!item.Contains(space)) return fallback;
            return item.Split(space)[^1];
        }

        private static bool HasReadableAddress(PersonAddress address)
        {
            var elements = new[] { address.Address1, address.Address2, address.Address3 }.ToList();
            return elements.Exists(x => !string.IsNullOrWhiteSpace(x));
        }

    }
}
