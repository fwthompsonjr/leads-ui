namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "ADHOCSESSION")]
    public class AdHocSessionDto : BaseDto
    {
        public string? UserId { get; set; }
        public string? IntentId { get; set; }
        public string? ClientId { get; set; }
        public string? ExternalId { get; set; }
        public DateTime? CompletionDate { get; set; }
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
                if (fieldName.Equals("IntentId", Comparison)) return IntentId;
                if (fieldName.Equals("ClientId", Comparison)) return ClientId;
                if (fieldName.Equals("ExternalId", Comparison)) return ExternalId;
                if (fieldName.Equals("CompletionDate", Comparison)) return CompletionDate;
                if (fieldName.Equals("CreateDate", Comparison)) return CreateDate;
                return null;
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
                if (fieldName.Equals("IntentId", Comparison)) { IntentId = ChangeType<string>(value); return; }
                if (fieldName.Equals("ClientId", Comparison)) { ClientId = ChangeType<string>(value); return; }
                if (fieldName.Equals("ExternalId", Comparison)) { ExternalId = ChangeType<string>(value); return; }
                if (fieldName.Equals("CompletionDate", Comparison)) { CompletionDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }
    }
}