namespace legallead.jdbc.entities
{
    public class AdHocSessionBo
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? IntentId { get; set; }
        public string? ClientId { get; set; }
        public string? ExternalId { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
