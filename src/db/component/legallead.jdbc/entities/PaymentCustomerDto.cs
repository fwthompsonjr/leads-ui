namespace legallead.jdbc.entities
{
    public class PaymentCustomerDto : BaseDto
    {
        public string? UserId { get; set; }
        public string? CustomerId { get; set; }
        public string? Email { get; set; }
        public bool? IsTest { get; set; }
        public DateTime? CreateDate { get; set; }

        public override object? this[string field]
        {
            get
            {
                if (field == null) return null;
                var fieldName = FieldList.Find(x => x.Equals(field, Comparison));
                if (fieldName == null) return null;
                if (fieldName.Equals("Id", Comparison)) return Id;
                if (fieldName.Equals("UserId", Comparison)) return UserId;
                if (fieldName.Equals("CustomerId", Comparison)) return CustomerId;
                if (fieldName.Equals("Email", Comparison)) return Email;
                if (fieldName.Equals("IsTest", Comparison)) return IsTest;
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
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); return; }
                if (fieldName.Equals("UserId", Comparison)) { UserId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("CustomerId", Comparison)) { CustomerId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("Email", Comparison)) { Email = ChangeType<string?>(value); return; }
                if (fieldName.Equals("IsTest", Comparison)) { IsTest = ChangeType<bool?>(value); }
            }
        }
    }
}