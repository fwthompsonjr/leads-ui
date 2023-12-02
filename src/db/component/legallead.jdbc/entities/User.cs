using System.Xml.Linq;
using System;

namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "users")]
    public class User : BaseDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;


        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserName", Comparison)) return UserName;
                if (fieldName.Equals("Email", Comparison)) return Email;
                if (fieldName.Equals("PasswordHash", Comparison)) return PasswordHash;
                if (fieldName.Equals("PasswordSalt", Comparison)) return PasswordSalt;
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
                if (fieldName.Equals("UserName", Comparison))
                {
                    UserName = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("Email", Comparison))
                {
                    Email = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("PasswordHash", Comparison))
                {
                    PasswordHash = ChangeType<string>(value) ?? string.Empty;
                    return;
                }
                if (fieldName.Equals("PasswordSalt", Comparison))
                {
                    PasswordSalt = ChangeType<string>(value) ?? string.Empty;
                }
            }
        }
    }
}