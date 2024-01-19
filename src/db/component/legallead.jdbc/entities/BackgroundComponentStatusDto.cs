namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "BGCOMPONENTSTATUS")]
    public class BackgroundComponentStatusDto : BaseDto
    {
        public string? ComponentId { get; set; }
        public int? LineNbr { get; set; }
        public string? StatusName { get; set; }
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
                if (fieldName.Equals("ComponentId", Comparison)) return ComponentId;
                if (fieldName.Equals("LineNbr", Comparison)) return LineNbr;
                if (fieldName.Equals("StatusName", Comparison)) return StatusName;
                if (fieldName.Equals("StatusId", Comparison)) return StatusId;
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
                if (fieldName.Equals("StatusName", Comparison))
                {
                    StatusName = ChangeType<string>(value);
                }
                if (fieldName.Equals("StatusId", Comparison))
                {
                    StatusId = ChangeType<int?>(value);
                }
                if (fieldName.Equals("CreateDate", Comparison))
                {
                    CreateDate = ChangeType<DateTime?>(value);
                }
            }
        }
    }
}