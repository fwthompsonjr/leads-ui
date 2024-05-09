using legallead.email.attributes;

namespace legallead.email.entities
{
    [TargetTable("PRC_EMAIL_GET_ACCOUNT_BY_EMAIL_ADDRESS")]
    public class GetUserAccountByEmailDto : BaseDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }

        public override object? this[string field]
        {
            get
            {
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserName", Comparison)) return UserName;
                return Email;
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
                if (fieldName.Equals("UserName", Comparison)) { UserName = ChangeType<string?>(value); return; }
                if (fieldName.Equals("Email", Comparison)) { Email = ChangeType<string?>(value); }
            }
        }
    }
}