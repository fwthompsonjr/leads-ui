namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "BGCOMPONENT")]
    public class BackgroundComponentDto : BaseDto
    {
        public string? ComponentName { get; set; }
        public string? ServiceName { get; set; }
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("ComponentName", Comparison)) return ComponentName;
                if (fieldName.Equals("ServiceName", Comparison)) return ServiceName;
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
                if (fieldName.Equals("ComponentName", Comparison))
                {
                    ComponentName = ChangeType<string>(value);
                }
                if (fieldName.Equals("ServiceName", Comparison))
                {
                    ServiceName = ChangeType<string>(value);
                }
                if (fieldName.Equals("CreateDate", Comparison))
                {
                    CreateDate = ChangeType<DateTime?>(value);
                }
            }
        }
    }
}