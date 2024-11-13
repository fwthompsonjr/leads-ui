namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "LEADUSERCOUNTYINDEXES")]
    public class LeadUserCountyIndexDto : BaseDto
    {
        public string? LeadUserId { get; set; }
        public string? CountyList { get; set; }
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("LeadUserId", Comparison)) return LeadUserId;
                if (fieldName.Equals("CountyList", Comparison)) return CountyList;
                return CreateDate;
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
                if (fieldName.Equals("LeadUserId", Comparison)) { LeadUserId = ChangeType<string>(value); return; }
                if (fieldName.Equals("CountyList", Comparison)) { CountyList = ChangeType<string>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) CreateDate = ChangeType<DateTime?>(value);
            }
        }
    }
}