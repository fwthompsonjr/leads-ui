namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "LEADCOUNTYPRICING")]
    public class LeadCountyPricingDto : BaseDto
    {
        public int? CountyId { get; set; }
        public string? CountyName { get; set; }
        public bool? IsActive { get; set; }
        public decimal? PerRecord { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("CountyId", Comparison)) return CountyId;
                if (fieldName.Equals("CountyName", Comparison)) return CountyName;
                if (fieldName.Equals("IsActive", Comparison)) return IsActive;
                if (fieldName.Equals("PerRecord", Comparison)) return PerRecord;
                if (fieldName.Equals("CompleteDate", Comparison)) return CompleteDate;
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
                if (fieldName.Equals("CountyId", Comparison)) { CountyId = ChangeType<int?>(value); return; }
                if (fieldName.Equals("CountyName", Comparison)) { CountyName = ChangeType<string>(value); return; }
                if (fieldName.Equals("IsActive", Comparison)) { IsActive = ChangeType<bool?>(value); return; }
                if (fieldName.Equals("PerRecord", Comparison)) { PerRecord = ChangeType<decimal?>(value); return; }
                if (fieldName.Equals("CompleteDate", Comparison)) { CompleteDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) CreateDate = ChangeType<DateTime?>(value);
            }
        }
    }
}