﻿namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "LEADUSERACCOUNT")]
    public class LeadUserAccountDto : BaseDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool? IsAdministrator { get; set; }
        public DateTime? CreateDate { get; set; }

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
                if (fieldName.Equals("IsAdministrator", Comparison)) return IsAdministrator;
                return CreateDate;
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
                if (fieldName.Equals("UserName", Comparison)) { UserName = ChangeType<string>(value); return; }
                if (fieldName.Equals("Email", Comparison)) { Email = ChangeType<string>(value); return; }
                if (fieldName.Equals("IsAdministrator", Comparison)) { IsAdministrator = ChangeType<bool?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) CreateDate = ChangeType<DateTime?>(value);
            }
        }
    }
}
