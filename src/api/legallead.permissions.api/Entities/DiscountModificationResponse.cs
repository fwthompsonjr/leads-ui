namespace legallead.permissions.api.Entities
{
    public class DiscountModificationResponse
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public string? ExternalId { get; set; }
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public string? SuccessUrl { get; set; }
        public List<DiscountPaymentBo> Data { get; set; } = [];
    }
}
