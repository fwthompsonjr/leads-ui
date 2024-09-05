namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "StatusSummary")]
    public class StatusSummaryDto : BaseDto
    {
        public string? Region { get; set; }
        public int? Count { get; set; }
        public DateTime? Oldest { get; set; }
        public DateTime? Newest { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("Region", Comparison)) return Region;
                if (fieldName.Equals("Count", Comparison)) return Count;
                if (fieldName.Equals("Oldest", Comparison)) return Oldest;
                return Newest;
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
                if (fieldName.Equals("Region", Comparison))
                {
                    Region = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("Count", Comparison))
                {
                    Count = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("Oldest", Comparison))
                {
                    Oldest = ChangeType<DateTime?>(value);
                    return;
                }
                if (fieldName.Equals("Newest", Comparison)) Newest = ChangeType<DateTime?>(value);
            }
        }
    }
}