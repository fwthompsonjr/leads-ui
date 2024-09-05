namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "SearchProgress")]
    public class StatusDto : BaseDto
    {
        public string? SearchProgress { get; set; }
        public int? Total { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("SearchProgress", Comparison)) return SearchProgress;
                return Total;
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
                if (fieldName.Equals("SearchProgress", Comparison))
                {
                    SearchProgress = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("Total", Comparison)) Total = ChangeType<int?>(value);
            }
        }
    }
}