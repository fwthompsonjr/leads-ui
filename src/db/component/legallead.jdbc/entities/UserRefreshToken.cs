namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "usertokens")]
    public class UserRefreshToken : BaseDto
    {
        public string UserId { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime? CreateDate { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("RefreshToken", Comparison)) return RefreshToken;
                if (fieldName.Equals("IsActive", Comparison)) return IsActive;
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
                    Id = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("UserId", Comparison))
                {
                    UserId = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("RefreshToken", Comparison))
                {
                    RefreshToken = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("IsActive", Comparison))
                {
                    IsActive = ChangeType<bool>(value);
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