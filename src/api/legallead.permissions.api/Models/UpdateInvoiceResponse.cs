namespace legallead.permissions.api.Models
{
    public class UpdateInvoiceResponse : UpdateInvoiceRequest
    {
        public bool IsUpdated { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
