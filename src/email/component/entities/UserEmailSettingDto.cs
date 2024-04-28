using legallead.email.attributes;

namespace legallead.email.entities
{
    [TargetTable("PRC__GET_USER_EMAIL_SETTINGS")]
    public class UserEmailSettingDto : BaseDto
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? KeyValue { get; set; }
        public string? KeyName { get; set; }

        public override object? this[string field]
        {
            get
            {
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("Email", Comparison)) return Email;
                if (fieldName.Equals("UserName", Comparison)) return UserName;
                if (fieldName.Equals("KeyValue", Comparison)) return KeyValue;
                return KeyName;
            }
            set
            {
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return;
                if (fieldName.Equals("Id", Comparison))
                {
                    Id = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("Email", Comparison)) { Email = ChangeType<string?>(value); return; }
                if (fieldName.Equals("UserName", Comparison)) { UserName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("KeyValue", Comparison)) { KeyValue = ChangeType<string?>(value); return; }
                if (fieldName.Equals("KeyName", Comparison)) { KeyName = ChangeType<string?>(value); }
            }
        }


    }
}
