namespace legallead.jdbc.entities
{
    public class LevelRequestDto : BaseDto
    {
        public string? UserId { get; set; }
        public string? ExternalId { get; set; }
        public string? InvoiceUri { get; set; }
        public string? LevelName { get; set; }
        public string? SessionId { get; set; }
        public bool? IsPaymentSuccess { get; set; }
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
                if (fieldName.Equals("ExternalId", Comparison)) return ExternalId;
                if (fieldName.Equals("InvoiceUri", Comparison)) return InvoiceUri;
                if (fieldName.Equals("LevelName", Comparison)) return LevelName;
                if (fieldName.Equals("SessionId", Comparison)) return SessionId;
                if (fieldName.Equals("IsPaymentSuccess", Comparison)) return IsPaymentSuccess;
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
                if (fieldName.Equals("UserId", Comparison)) { UserId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("ExternalId", Comparison)) { ExternalId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("InvoiceUri", Comparison)) { InvoiceUri = ChangeType<string?>(value); return; }
                if (fieldName.Equals("LevelName", Comparison)) { LevelName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("SessionId", Comparison)) { SessionId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("IsPaymentSuccess", Comparison)) { IsPaymentSuccess = ChangeType<bool?>(value); return; }
                if (fieldName.Equals("CompletionDate", Comparison)) { CompletionDate = ChangeType<DateTime?>(value); }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }
    }
}