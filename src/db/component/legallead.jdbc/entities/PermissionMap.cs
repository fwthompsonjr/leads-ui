namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "permissionmap")]
    public class PermissionMap : BaseDto
    {
        public int OrderId { get; set; }
        public string KeyName { get; set; } = string.Empty;

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("OrderId", Comparison)) return OrderId;
                if (fieldName.Equals("KeyName", Comparison)) return KeyName;
                return null;
            }
            set
            {
                if (field == null) return;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison))
                {
                    Id = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("OrderId", Comparison))
                {
                    OrderId = ChangeType<int>(value);
                }
                if (fieldName.Equals("KeyName", Comparison))
                {
                    KeyName = ChangeType<string>(value) ?? string.Empty;
                }
            }
        }
    }
}