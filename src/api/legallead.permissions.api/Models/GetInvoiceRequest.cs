using legallead.permissions.api.Attr;

namespace legallead.permissions.api.Models
{
    public class GetInvoiceRequest
    {
        [GetInvoiceType]
        public string RequestType { get; set; } = string.Empty;
        
        public string CustomerId { get; set; } = string.Empty;
        public string InvoiceId { get; set; } = string.Empty;
    }
}
