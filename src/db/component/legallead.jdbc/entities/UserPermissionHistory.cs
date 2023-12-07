namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "userpermissionhistory")]
    public class UserPermissionHistory : BaseDto
    {
        public string UserPermissionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string PermissionMapId { get; set; } = string.Empty;
        public string KeyValue { get; set; } = string.Empty;
        public string KeyName { get; set; } = string.Empty;
        public int? GroupId { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserPermissionId", Comparison)) return UserPermissionId;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("PermissionMapId", Comparison)) return PermissionMapId;
                if (fieldName.Equals("KeyValue", Comparison)) return KeyValue;
                if (fieldName.Equals("KeyName", Comparison)) return KeyName;
                if (fieldName.Equals("GroupId", Comparison)) return GroupId;
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
                if (fieldName.Equals("UserPermissionId", Comparison))
                {
                    UserPermissionId = ChangeType<string>(value) ?? string.Empty;
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
                    return;
                }
                if (fieldName.Equals("KeyName", Comparison))
                {
                    KeyName = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("GroupId", Comparison))
                {
                    GroupId = ChangeType<int?>(value);
                }
            }
        }
    }
}