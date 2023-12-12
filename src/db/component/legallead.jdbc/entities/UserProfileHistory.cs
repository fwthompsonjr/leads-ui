namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "userprofilehistory")]
    public class UserProfileHistory : BaseDto
    {
        public string UserProfileId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ProfileMapId { get; set; } = string.Empty;
        public string KeyValue { get; set; } = string.Empty;
        public string KeyName { get; set; } = string.Empty;
        public int? GroupId { get; set; }
        public DateTime? CreateDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserProfileId", Comparison)) return UserProfileId;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("ProfileMapId", Comparison)) return ProfileMapId;
                if (fieldName.Equals("KeyValue", Comparison)) return KeyValue;
                if (fieldName.Equals("KeyName", Comparison)) return KeyName;
                if (fieldName.Equals("GroupId", Comparison)) return GroupId;
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
                if (fieldName.Equals("UserProfileId", Comparison))
                {
                    UserProfileId = ChangeType<string>(value) ?? string.Empty;
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
                if (fieldName.Equals("GroupId", Comparison))
                {
                    GroupId = ChangeType<int?>(value);
                    return;
                }
                if (fieldName.Equals("CreateDate", Comparison))
                {
                    CreateDate = ChangeType<DateTime?>(value);
                }
            }
        }
    }
}