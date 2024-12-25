using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IInvoiceRepository
    {
        Task<GenerateInvoiceBo?> GenerateInvoicesAsync();
        Task<List<DbInvoiceViewBo>?> FindAllAsync();
        Task<List<DbInvoiceViewBo>?> QueryAsync(DbInvoiceViewBo query);
        Task<KeyValuePair<bool, string>> UpdateAsync(DbInvoiceViewBo query);
        Task<List<LeadCustomerBo>?> FindAccountAsync(LeadCustomerBo query);
        Task<List<LeadCustomerBo>?> CreateAccountAsync(LeadCustomerBo query);
    }
}
