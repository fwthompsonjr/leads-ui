namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "LEADUSERCOUNTY")]
    public class LeadUserCountyDto : BaseDto
    {
        public string? LeadUserId { get; set; }
        public string? CountyName { get; set; }
        public string? Phrase { get; set; }
        public string? Vector { get; set; }
        public string? Token { get; set; }
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
                if (fieldName.Equals("CountyName", Comparison)) return CountyName;
                if (fieldName.Equals("Phrase", Comparison)) return Phrase;
                if (fieldName.Equals("Vector", Comparison)) return Vector;
                if (fieldName.Equals("Token", Comparison)) return Token;
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
                if (fieldName.Equals("CountyName", Comparison)) { CountyName = ChangeType<string>(value); return; }
                if (fieldName.Equals("Phrase", Comparison)) { Phrase = ChangeType<string>(value); return; }
                if (fieldName.Equals("Vector", Comparison)) { Vector = ChangeType<string>(value); return; }
                if (fieldName.Equals("Token", Comparison)) { Token = ChangeType<string>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) CreateDate = ChangeType<DateTime?>(value);
            }
        }
    }
}