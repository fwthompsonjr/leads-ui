namespace legallead.jdbc.entities
{
    public class LevelPaymentBo
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? ExternalId { get; set; }
        public string? LevelName { get; set; }
        public string? PriceType { get; set; }
        public decimal? Price { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? SubscriptionAmount { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}