namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "MYLEADPROFILE")]
    public class MyProfileDto : BaseDto
    {
        public int? OrderId { get; set; }
        public string? UserId { get; set; } = string.Empty;
        public string? ProfileId { get; set; }
        public string? ProfileGroup { get; set; } = string.Empty;
        public string? KeyValue { get; set; } = string.Empty;
        public string? KeyName { get; set; } = string.Empty;

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("OrderId", Comparison)) return OrderId;
                if (fieldName.Equals("ProfileId", Comparison)) return ProfileId;
                if (fieldName.Equals("ProfileGroup", Comparison)) return ProfileGroup;
                if (fieldName.Equals("KeyValue", Comparison)) return KeyValue;
                return KeyName;
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
                if (fieldName.Equals("OrderId", Comparison))
                {
                    OrderId = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("UserId", Comparison))
                {
                    UserId = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("ProfileId", Comparison))
                {
                    ProfileId = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("ProfileGroup", Comparison))
                {
                    ProfileGroup = ChangeType<string>(value) ?? string.Empty;
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
                }
            }
        }
    }
}