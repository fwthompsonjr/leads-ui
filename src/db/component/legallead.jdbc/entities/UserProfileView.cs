namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "vwuserprofile")]
    public class UserProfileView : BaseDto
    {
        public string UserId { get; set; } = string.Empty;
        public string ProfileMapId { get; set; } = string.Empty;
        public string KeyValue { get; set; } = string.Empty;
        public string KeyName { get; set; } = string.Empty;
        public int OrderId { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("ProfileMapId", Comparison)) return ProfileMapId;
                if (fieldName.Equals("KeyValue", Comparison)) return KeyValue;
                if (fieldName.Equals("KeyName", Comparison)) return KeyName;
                if (fieldName.Equals("OrderId", Comparison)) return OrderId;
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
                if (fieldName.Equals("ProfileMapId", Comparison))
                {
                    ProfileMapId = ChangeType<string>(value) ?? string.Empty;
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
                if (fieldName.Equals("OrderId", Comparison))
                {
                    OrderId = ChangeType<int>(value);
                }
            }
        }
    }
}