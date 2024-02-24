using legallead.jdbc.entities;

namespace legallead.permissions.api.Models
{
    public class PaymentSessionJs
    {
        public List<SearchInvoiceBo> Data { get; set; } = new();
        public string? Description { get; set; }
        public string? ExternalId { get; set; }
        public string? SuccessUrl { get; set; }
    }
}
