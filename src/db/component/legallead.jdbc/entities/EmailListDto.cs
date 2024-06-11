namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "EMAILLIST")]
    public class EmailListDto : BaseDto
    {
        public string? UserId { get; set; }
        public string? FromAddress { get; set; }
        public string? ToAddress { get; set; }
        public string? Subject { get; set; }
        public int? StatusId { get; set; }
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("FromAddress", Comparison)) return FromAddress;
                if (fieldName.Equals("ToAddress", Comparison)) return ToAddress;
                if (fieldName.Equals("Subject", Comparison)) return Subject;
                if (fieldName.Equals("StatusId", Comparison)) return StatusId;
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
                if (fieldName.Equals("UserId", Comparison)) { UserId = ChangeType<string>(value); return; }
                if (fieldName.Equals("FromAddress", Comparison)) { FromAddress = ChangeType<string>(value); return; }
                if (fieldName.Equals("ToAddress", Comparison)) { ToAddress = ChangeType<string>(value); return; }
                if (fieldName.Equals("Subject", Comparison)) { Subject = ChangeType<string>(value); return; }
                if (fieldName.Equals("StatusId", Comparison)) { StatusId = ChangeType<int?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }
    }
}
