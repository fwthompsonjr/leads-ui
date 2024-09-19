namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "QUEUEPERSONS")]
    public class QueueNonPersonDto : BaseDto
    {
        public string? UserId { get; set; }
        public int? ExpectedRows { get; set; }
        public string? SearchProgress { get; set; }
        public string? StateCode { get; set; }
        public string? CountyName { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public byte[]? ExcelData { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("ExpectedRows", Comparison)) return ExpectedRows;
                if (fieldName.Equals("SearchProgress", Comparison)) return SearchProgress;
                if (fieldName.Equals("StateCode", Comparison)) return StateCode;
                if (fieldName.Equals("CountyName", Comparison)) return CountyName;
                if (fieldName.Equals("EndDate", Comparison)) return EndDate;
                if (fieldName.Equals("StartDate", Comparison)) return StartDate;
                if (fieldName.Equals("CreateDate", Comparison)) return CreateDate;
                return ExcelData;
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
                if (fieldName.Equals("UserId", Comparison)) { UserId = ChangeType<string>(value); return; }
                if (fieldName.Equals("ExpectedRows", Comparison)) { ExpectedRows = ChangeType<int?>(value); return; }
                if (fieldName.Equals("SearchProgress", Comparison)) { SearchProgress = ChangeType<string>(value); return; }
                if (fieldName.Equals("StateCode", Comparison)) { StateCode = ChangeType<string>(value); return; }
                if (fieldName.Equals("CountyName", Comparison)) { CountyName = ChangeType<string>(value); return; }
                if (fieldName.Equals("EndDate", Comparison)) { EndDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("StartDate", Comparison)) { StartDate = ChangeType<DateTime?>(value); }
                if (fieldName.Equals("CreateDate", Comparison)) CreateDate = ChangeType<DateTime?>(value);
                if (fieldName.Equals("ExcelData", Comparison)) ExcelData = ChangeType<byte[]?>(value);
            }
        }
    }
}