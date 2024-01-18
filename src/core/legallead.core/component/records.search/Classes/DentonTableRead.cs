
using legallead.records.search.Models;

namespace legallead.records.search.Classes
{
    public class DentonTableRead
    {
        public string? Heading { get; set; }
        public string? Records { get; set; }
        public DentonTableReadRecord[]? RecordSet { get; set; }
        public DentonTableReadHeader? Header { get; set; }

        internal List<HLinkDataRow> ToCaseList(bool isCriminalSearch = false)
        {
            var list = new List<HLinkDataRow>();
            if (RecordSet == null || !RecordSet.Any()) return list;
            var source = RecordSet.ToList();
            source.ForEach(s =>
            {
                var info = new HLinkDataRow
                {
                    Case = s.CaseNumber ?? string.Empty,
                    CaseStyle = s.CaseStyle ?? string.Empty,
                    CaseType = s.CaseType ?? string.Empty,
                    Status = s.Status ?? string.Empty,
                    Court = s.Court ?? string.Empty,
                    DateFiled = s.DateFiled ?? string.Empty,
                    IsCriminal = isCriminalSearch,
                    CriminalCaseStyle = (isCriminalSearch ? s.CaseStyle : string.Empty) ?? string.Empty,
                    WebAddress = s.WebAddress ?? string.Empty,
                };
                list.Add(info);
            });
            return list;
        }
    }
}
