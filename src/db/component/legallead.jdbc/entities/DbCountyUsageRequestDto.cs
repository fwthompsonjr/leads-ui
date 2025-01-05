namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "DBCOUNTYUSAGEREQUEST")]
    public class DbCountyUsageRequestDto : BaseDto
    {
        public string? LeadUserId { get; set; }
        public int? CountyId { get; set; }
        public string? CountyName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? DateRange { get; set; }
        public int? RecordCount { get; set; }
        public DateTime? CompleteDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? ShortFileName { get; set; }
        public DateTime? FileCompletedDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("LeadUserId", Comparison)) return LeadUserId;
                if (fieldName.Equals("CountyId", Comparison)) return CountyId;
                if (fieldName.Equals("CountyName", Comparison)) return CountyName;
                if (fieldName.Equals("StartDate", Comparison)) return StartDate;
                if (fieldName.Equals("EndDate", Comparison)) return EndDate;
                if (fieldName.Equals("DateRange", Comparison)) return DateRange;
                if (fieldName.Equals("RecordCount", Comparison)) return RecordCount;
                if (fieldName.Equals("CompleteDate", Comparison)) return CompleteDate;
                if (fieldName.Equals("ShortFileName", Comparison)) return ShortFileName;
                if (fieldName.Equals("FileCompletedDate", Comparison)) return FileCompletedDate;
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
                if (fieldName.Equals("LeadUserId", Comparison)) { LeadUserId = ChangeType<string>(value); return; }
                if (fieldName.Equals("CountyId", Comparison)) { CountyId = ChangeType<int?>(value); return; }
                if (fieldName.Equals("CountyName", Comparison)) { CountyName = ChangeType<string>(value); return; }
                if (fieldName.Equals("StartDate", Comparison)) { StartDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("EndDate", Comparison)) { EndDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("DateRange", Comparison)) { DateRange = ChangeType<string>(value); return; }
                if (fieldName.Equals("RecordCount", Comparison)) { RecordCount = ChangeType<int?>(value); return; }
                if (fieldName.Equals("CompleteDate", Comparison)) { CompleteDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("ShortFileName", Comparison)) { ShortFileName = ChangeType<string>(value); return; }
                if (fieldName.Equals("FileCompletedDate", Comparison)) { FileCompletedDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }

    }
}