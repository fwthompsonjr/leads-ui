namespace legallead.jdbc.interfaces
{
    public interface ISubscriptionDetail
    {
        string Id { get; set; }
        string? UserId { get; set; }
        string? CustomerId { get; set; }
        string? Email { get; set; }
        string? ExternalId { get; set; }
        string? SubscriptionType { get; set; }
        string? SubscriptionDetail { get; set; }
        string? PermissionLevel { get; set; }
        string? CountySubscriptions { get; set; }
        string? StateSubscriptions { get; set; }
        bool? IsSubscriptionVerified { get; set; }
        DateTime? VerificationDate { get; set; }
        DateTime? CompletionDate { get; set; }
        DateTime? CreateDate { get; set; }
    }
}
