namespace legallead.permissions.api.Models
{
    public class GetInvoiceResponse
    {
        public List<InvoiceHeaderModel> Headers { get; set; } = [];
        public List<InvoiceDetailModel> Lines { get; set; } = [];
    }
}
