namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "BGCOMPONENTHEATH")]
    public class BackgroundComponentHealthDto : BaseDto
    {
        public string? ComponentId { get; set; }
        public int? LineNbr { get; set; }
        public string? Health { get; set; }
        public int? HealthId { get; set; }
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("ComponentId", Comparison)) return ComponentId;
                if (fieldName.Equals("LineNbr", Comparison)) return LineNbr;
                if (fieldName.Equals("Health", Comparison)) return Health;
                if (fieldName.Equals("HealthId", Comparison)) return HealthId;
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
                if (fieldName.Equals("ComponentId", Comparison))
                {
                    ComponentId = ChangeType<string>(value);
                }
                if (fieldName.Equals("LineNbr", Comparison))
                {
                    LineNbr = ChangeType<int?>(value);
                }
                if (fieldName.Equals("Health", Comparison))
                {
                    Health = ChangeType<string>(value);
                }
                if (fieldName.Equals("HealthId", Comparison))
                {
                    HealthId = ChangeType<int?>(value);
                }
                if (fieldName.Equals("CreateDate", Comparison))
                {
                    CreateDate = ChangeType<DateTime?>(value);
                }
            }
        }
    }
}