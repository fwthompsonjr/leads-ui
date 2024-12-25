namespace legallead.jdbc.entities
{
    [TargetTable(TableName = "LEADPAYMENTCUSTOMER")]
    public class LeadCustomerDto : BaseDto
    {
        public string? LeadUserId { get; set; }
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
                if (fieldName.Equals("LeadUserId", Comparison)) return LeadUserId;
                if (fieldName.Equals("CustomerId", Comparison)) return CustomerId;
                if (fieldName.Equals("Email", Comparison)) return Email;
                if (fieldName.Equals("IsTest", Comparison)) return IsTest;
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
                if (fieldName.Equals("LeadUserId", Comparison)) { LeadUserId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("CustomerId", Comparison)) { CustomerId = ChangeType<string?>(value); return; }
                if (fieldName.Equals("Email", Comparison)) { Email = ChangeType<string?>(value); return; }
                if (fieldName.Equals("IsTest", Comparison)) { IsTest = ChangeType<bool?>(value); return; }
                if (fieldName.Equals("CreateDate", Comparison)) { CreateDate = ChangeType<DateTime?>(value); }
            }
        }
    }
}