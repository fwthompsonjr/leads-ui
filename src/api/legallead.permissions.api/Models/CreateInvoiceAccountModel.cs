using System.ComponentModel.DataAnnotations;

namespace legallead.permissions.api.Models
{
    public class CreateInvoiceAccountModel
    {
        [Required] public string? LeadId { get; set; }
        [Required] public string? EmailAcct { get; set; }
        [Required] public string? CustId { get; set; }
        public bool? IsTesting { get; set; }
    }
}
