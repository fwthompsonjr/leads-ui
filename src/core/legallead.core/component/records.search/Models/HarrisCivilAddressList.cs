using Newtonsoft.Json;

namespace legallead.records.search.Models
{
    internal static class HarrisCivilAddressList
    {
        public static List<HarrisCivilAddressItem> List { get; set; } = new();

        public static void Clear()
        {
            lock (sync) { List.Clear(); }
        }

        public static void AddRange(List<CaseRowData> table)
        {
            var data = table.SelectMany(a => a.CaseDataAddresses)
                .ToList()
                .FindAll(a =>
                {
                    var role = a.Role.Trim();
                    var party = a.Party.Trim();
                    var caseno = a.Case.Trim();
                    if (string.IsNullOrWhiteSpace(role) ||
                        string.IsNullOrWhiteSpace(party) ||
                        string.IsNullOrWhiteSpace(caseno)) return false;
                    return Keywords.Contains(role, StringComparer.OrdinalIgnoreCase);
                }) ?? new();
            var items = data.Select(a =>
            {
                return new HarrisCivilAddressItem
                {
                    CaseNumber = a.Case.Trim(),
                    Address = GetAddress(a.Party.Trim()),
                    Defendant = a.GetPerson()
                };
            }).ToList();
            var caseNumbers = items.Select(s => s.CaseNumber).Distinct().ToList();
            caseNumbers.ForEach(c =>
            {
                var children = items.FindAll(n => n.CaseNumber.Equals(c, oic));
                if (children.Count > 0)
                {
                    foreach (var child in children) child.Index = children.IndexOf(child);
                }
            });
            items.ForEach(i => i.Parse());
            lock (sync) 
            {
                caseNumbers.ForEach(a => List.RemoveAll(x => x.CaseNumber.Equals(a, oic)));
                List.AddRange(items);
            }
            
        }

        public static void Map(PersonAddress person, int index)
        {
            lock (sync)
            {
                var caseno = person.CaseNumber;
                var zip = person.Zip;
                if (string.IsNullOrWhiteSpace(caseno)) return;
                if (!zip.Equals("00000")) return;
                if (!List.Exists(x => x.CaseNumber.Equals(caseno, oic))) return;
                var subset = List.FindAll(x => x.CaseNumber.Equals(caseno, oic));
                if (subset.Count == 0) { return; }
                var found = subset.Find(x => x.Index == index) ?? subset[0];
                found.Parse();
                if (string.IsNullOrEmpty(found.Address1)) return;
                person.Address1 = found.Address1;
                person.Address2 = found.Address2;
                person.Address3 = found.Address3;
                person.Zip = found.Zip;
            }
        }

        private static string GetAddress(string text)
        {
            char pipe = '|';
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            if (!text.Contains(pipe)) return string.Empty;
            var items = text.Split(pipe, StringSplitOptions.RemoveEmptyEntries);
            var collection = new List<string>();
            for ( var i = 1; i < items.Length; i++)
            {
                collection.Add(items[i].Trim());
            }
            return string.Join(pipe, collection);
        }
        private const StringComparison oic = StringComparison.OrdinalIgnoreCase;
        private static readonly object sync = new();
        private static readonly string[] Keywords = new[] { 
            "plaintiff", 
            "defendant", 
            "principal", 
            "petitioner", 
            "applicant", 
            "claimant", 
            "decedent", 
            "respondent", 
            "condemnee", 
            "guardian" 
        };
    }
}