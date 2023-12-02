namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "userpermission")]
    public class UserPermission : BaseDto
    {
        public string UserId { get; set; } = string.Empty;
        public string PermissionMapId { get; set; } = string.Empty;
        public string KeyValue { get; set; } = string.Empty;
        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("PermissionMapId", Comparison)) return PermissionMapId;
                if (fieldName.Equals("KeyValue", Comparison)) return KeyValue;
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
                if (fieldName.Equals("UserId", Comparison))
                {
                    UserId = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("PermissionMapId", Comparison))
                {
                    PermissionMapId = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("KeyValue", Comparison))
                {
                    KeyValue = ChangeType<string>(value) ?? string.Empty;
                }
            }
        }
    }
}