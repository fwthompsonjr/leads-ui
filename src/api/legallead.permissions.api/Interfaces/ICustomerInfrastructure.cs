using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface ICustomerInfrastructure
    {
        Task<PaymentCustomerBo?> GetCustomerAsync(string userId);
        Task<PaymentCustomerBo?> CreateCustomerAsync(string userId, string accountId);
        Task<PaymentCustomerBo?> GetOrCreateCustomerAsync(string userId);
        Task<List<UnMappedCustomerBo>?> GetUnMappedCustomersAsync();
        Task<bool> MapCustomersAsync();
        Task<LevelRequestBo?> AddLevelChangeRequestAsync(LevelChangeRequest request);
        Task<LevelRequestBo?> GetLevelRequestByIdAsync(string externalId);
        Task<LevelRequestBo?> CompleteLevelRequestAsync(LevelRequestBo request);
        Task<LevelRequestBo?> CompleteDiscountRequestAsync(LevelRequestBo request);
        Task<LevelRequestBo?> AddDiscountChangeRequestAsync(LevelChangeRequest request);
        Task<LevelRequestBo?> GetDiscountRequestByIdAsync(string externalId);
    }
}
