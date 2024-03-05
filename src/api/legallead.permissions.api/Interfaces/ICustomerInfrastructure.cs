using legallead.jdbc.entities;

namespace legallead.permissions.api.Interfaces
{
    public interface ICustomerInfrastructure
    {
        Task<PaymentCustomerBo?> GetCustomer(string userId);
        Task<PaymentCustomerBo?> CreateCustomer(string userId, string accountId);
        Task<PaymentCustomerBo?> GetOrCreateCustomer(string userId);
        Task<List<UnMappedCustomerBo>?> GetUnMappedCustomers(); 
        Task<bool> MapCustomers();
    }
}
