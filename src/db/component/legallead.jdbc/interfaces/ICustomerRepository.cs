using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface ICustomerRepository
    {
        Task<bool> DoesCustomerExist(PaymentCustomerQuery query);
        Task<PaymentCustomerBo?> GetCustomer(PaymentCustomerQuery query);
        Task<KeyValuePair<bool, string>> AddCustomer(PaymentCustomerInsert dto);
        Task<List<UnMappedCustomerBo>?> GetUnMappedCustomers(PaymentCustomerQuery query);
        Task<KeyValuePair<bool, string>> AddLevelChangeRequest(string jsonRequest);
        Task<List<LevelRequestBo>?> GetLevelRequestHistory(string userId);
        Task<LevelRequestBo?> GetLevelRequestById(string externalId);
        Task<KeyValuePair<bool, string>> UpdateLevelChangeRequest(string jsonRequest);
    }
}