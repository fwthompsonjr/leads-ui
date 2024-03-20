using legallead.jdbc.interfaces;

namespace legallead.jdbc.entities
{
    public class SubscriptionDetailDto : BaseDto, ISubscriptionDetail
    {
        public string? UserId { get; set; }
        public string? CustomerId { get; set; }
        public string? Email { get; set; }
        public string? ExternalId { get; set; }
        public string? SubscriptionType { get; set; }
        public string? SubscriptionDetail { get; set; }
        public string? PermissionLevel { get; set; }
        public string? CountySubscriptions { get; set; }
        public string? StateSubscriptions { get; set; }
        public bool? IsSubscriptionVerified { get; set; }
        public DateTime? VerificationDate { get; set; }
        public DateTime? CompletionDate { get; set; }
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
                if (fieldName.Equals("ExternalId", Comparison)) return ExternalId;
                if (fieldName.Equals("SubscriptionType", Comparison)) return SubscriptionType;
                if (fieldName.Equals("SubscriptionDetail", Comparison)) return SubscriptionDetail;
                if (fieldName.Equals("PermissionLevel", Comparison)) return PermissionLevel;
                if (fieldName.Equals("CountySubscriptions", Comparison)) return CountySubscriptions;
                if (fieldName.Equals("StateSubscriptions", Comparison)) return StateSubscriptions;
                if (fieldName.Equals("IsSubscriptionVerified", Comparison)) return IsSubscriptionVerified;
                if (fieldName.Equals("VerificationDate", Comparison)) return VerificationDate;
                if (fieldName.Equals("CompletionDate", Comparison)) return CompletionDate;
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
                if (fieldName.Equals("UserId", Comparison))
                {
                    UserId = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("CustomerId", Comparison))
                {
                    CustomerId = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("Email", Comparison))
                {
                    Email = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("ExternalId", Comparison))
                {
                    ExternalId = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("SubscriptionType", Comparison))
                {
                    SubscriptionType = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("SubscriptionDetail", Comparison))
                {
                    SubscriptionDetail = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("PermissionLevel", Comparison))
                {
                    PermissionLevel = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("CountySubscriptions", Comparison))
                {
                    CountySubscriptions = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("StateSubscriptions", Comparison))
                {
                    StateSubscriptions = ChangeType<string>(value);
                    return;
                }
                if (fieldName.Equals("IsSubscriptionVerified", Comparison))
                {
                    IsSubscriptionVerified = ChangeType<bool?>(value);
                    return;
                }
                if (fieldName.Equals("VerificationDate", Comparison))
                {
                    VerificationDate = ChangeType<DateTime?>(value);
                    return;
                }
                if (fieldName.Equals("CompletionDate", Comparison))
                {
                    CompletionDate = ChangeType<DateTime?>(value);
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
