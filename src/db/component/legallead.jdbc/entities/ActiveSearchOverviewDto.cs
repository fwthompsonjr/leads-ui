namespace legallead.jdbc.entities
{
    public class ActiveSearchOverviewDto : BaseDto
    {
        public string? Searches { get; set; }
        public string? Statuses { get; set; }
        public string? Staged { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("Searches", Comparison)) return Searches;
                if (fieldName.Equals("Statuses", Comparison)) return Statuses;
                return Staged;
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
                if (fieldName.Equals("Searches", Comparison)) { Searches = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("Statuses", Comparison)) { Statuses = ChangeType<string>(value) ?? string.Empty; return; }
                if (fieldName.Equals("Staged", Comparison)) { Staged = ChangeType<string>(value) ?? string.Empty; }
            }
        }
    }
}
