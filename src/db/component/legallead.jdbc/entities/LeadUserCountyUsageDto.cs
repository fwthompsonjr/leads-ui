namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "LEADUSERCOUNTYUSAGE")]
    public class LeadUserCountyUsageDto : BaseDto
    {
        public string? LeadUserId { get; set; }
        public string? LeadUserCountyId { get; set; }
        public string? CountyName { get; set; }
        public int? MonthlyUsage { get; set; }
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
                if (fieldName.Equals("LeadUserCountyId", Comparison)) return LeadUserCountyId;
                if (fieldName.Equals("CountyName", Comparison)) return CountyName;
                if (fieldName.Equals("MonthlyUsage", Comparison)) return MonthlyUsage.GetValueOrDefault();
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
                if (fieldName.Equals("LeadUserCountyId", Comparison)) { LeadUserCountyId = ChangeType<string>(value); return; }
                if (fieldName.Equals("CountyName", Comparison)) { CountyName = ChangeType<string>(value); return; }
                if (fieldName.Equals("MonthlyUsage", Comparison)) { MonthlyUsage = ChangeType<int?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) CreateDate = ChangeType<DateTime?>(value);
            }
        }
    }
}