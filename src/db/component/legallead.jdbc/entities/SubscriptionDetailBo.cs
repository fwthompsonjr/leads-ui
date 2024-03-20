using legallead.jdbc.interfaces;

namespace legallead.jdbc.entities
{
    public class SubscriptionDetailBo : ISubscriptionDetail
    {
        public string Id { get; set; } = string.Empty;
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
    }
}
