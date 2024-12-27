using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface ILeadInvoiceService
    {
        Task<bool> CloseInvoiceAsync(string invoiceId);
        Task<UpdateInvoiceResponse?> CreateRemoteInvoiceAsync(GetInvoiceRequest request);
        Task<GetInvoiceResponse?> GetByCustomerIdAsync(string id);
        Task<GetInvoiceResponse?> GetByInvoiceIdAsync(string id);
        Task<string?> GetInvoiceStatusAsync(GetInvoiceRequest request);
        Task<List<LeadCustomerBo>?> GetOrCreateAccountAsync(CreateInvoiceAccountModel query);
        Task<UpdateInvoiceResponse> UpdateInvoiceAsync(UpdateInvoiceRequest request);
    }
}
