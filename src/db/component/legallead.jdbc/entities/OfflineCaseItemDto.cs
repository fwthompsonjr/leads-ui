namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "OFF_CASEITEM_RSP")]
    public class OfflineCaseItemDto : BaseDto
    {
        public int? CountyId { get; set; }
        public string? CaseNumber { get; set; }
        public string? CaseHeader { get; set; }
        public string? PersonName { get; set; }
        public string? Plaintiff { get; set; }
        public string? Address { get; set; }
        public string? Zip { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("CountyId", Comparison)) return CountyId;
                if (fieldName.Equals("CaseNumber", Comparison)) return CaseNumber;
                if (fieldName.Equals("CaseHeader", Comparison)) return CaseHeader;
                if (fieldName.Equals("PersonName", Comparison)) return PersonName;
                if (fieldName.Equals("Plaintiff", Comparison)) return Plaintiff;
                if (fieldName.Equals("Address", Comparison)) return Address;
                return Zip;
            }
            set
            {
                if (field == null) return;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison))
                {
                    Id = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("CountyId", Comparison)) { CountyId = ChangeType<int?>(value); return; }
                if (fieldName.Equals("CaseNumber", Comparison)) { CaseNumber = ChangeType<string?>(value); return; }
                if (fieldName.Equals("CaseHeader", Comparison)) { CaseHeader = ChangeType<string?>(value); return; }
                if (fieldName.Equals("PersonName", Comparison)) { PersonName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("Plaintiff", Comparison)) { Plaintiff = ChangeType<string?>(value); return; }
                if (fieldName.Equals("Address", Comparison)) { Address = ChangeType<string?>(value); return; }
                if (fieldName.Equals("Zip", Comparison)) { Zip = ChangeType<string?>(value); }
            }
        }
    }
}