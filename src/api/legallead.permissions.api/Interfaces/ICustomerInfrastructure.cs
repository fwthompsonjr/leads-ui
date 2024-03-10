using legallead.jdbc.entities;
using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface ICustomerInfrastructure
    {
        Task<PaymentCustomerBo?> GetCustomer(string userId);
        Task<PaymentCustomerBo?> CreateCustomer(string userId, string accountId);
        Task<PaymentCustomerBo?> GetOrCreateCustomer(string userId);
        Task<List<UnMappedCustomerBo>?> GetUnMappedCustomers(); 
        Task<bool> MapCustomers();
        Task<LevelRequestBo?> AddLevelChangeRequest(LevelChangeRequest request);
        Task<LevelRequestBo?> GetLevelRequestById(string externalId);
        Task<LevelRequestBo?> CompleteLevelRequest(LevelRequestBo request);
    }
}
