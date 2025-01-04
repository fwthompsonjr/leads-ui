namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "DBCOUNTYFILENAME")]
    public class DbExcelNameDto : BaseDto
    {
        public string? LeadUserId { get; set; }
        public string? RequestId { get; set; }
        public string? ShortFileName { get; set; }
        public string? FileToken { get; set; }
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
                if (fieldName.Equals("LeadUserId", Comparison)) return LeadUserId;
                if (fieldName.Equals("RequestId", Comparison)) return RequestId;
                if (fieldName.Equals("ShortFileName", Comparison)) return ShortFileName;
                if (fieldName.Equals("FileToken", Comparison)) return FileToken;
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
                if (fieldName.Equals("LeadUserId", Comparison)) { LeadUserId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("RequestId", Comparison)) { RequestId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("ShortFileName", Comparison)) { ShortFileName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("FileToken", Comparison)) { FileToken = ChangeType<string?>(value); return; }
                if (fieldName.Equals("CompleteDate", Comparison)) { CompleteDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }

    }
}