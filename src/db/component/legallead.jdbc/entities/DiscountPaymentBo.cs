namespace legallead.jdbc.entities
{
    public class DiscountPaymentBo
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? ExternalId { get; set; }
        public string? LevelName { get; set; }
        public string? PriceType { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}