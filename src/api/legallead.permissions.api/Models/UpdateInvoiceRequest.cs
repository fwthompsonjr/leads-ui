using legallead.permissions.api.Attr;
using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Models
{
    public class UpdateInvoiceRequest
    {
        [UpdateInvoice]
        public string UpdateType { get; set; } = string.Empty;
        [Required]
        public string InvoiceId { get; set; } = string.Empty;
        public string InvoiceStatus { get; set; } = string.Empty;
        public string InvoiceUri { get; set; } = string.Empty;
    }
}
