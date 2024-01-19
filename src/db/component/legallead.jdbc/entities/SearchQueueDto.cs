namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "SEARCHQUEUE")]
    public class SearchQueueDto : SearchDto
    {
        public string? Payload { get; set; }

        public override object? this[string field]
        {
            get
            {
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Payload", Comparison)) return Payload;
                return base[field];
            }
            set
            {
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Payload", Comparison))
                {
                    Payload = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                base[field] = value;
            }
        }
    }
}
