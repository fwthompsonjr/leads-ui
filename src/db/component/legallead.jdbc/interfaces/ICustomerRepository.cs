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
        Task<KeyValuePair<bool, string>> AddDiscountChangeRequest(string jsonRequest);
        Task<List<LevelRequestBo>?> GetDiscountRequestHistory(string userId);
        Task<LevelRequestBo?> GetDiscountRequestById(string externalId);
        Task<KeyValuePair<bool, string>> UpdateDiscountChangeRequest(string jsonRequest);
        Task<List<SubscriptionDetailBo>?> GetUserSubscriptions(bool forVerification);
        Task<KeyValuePair<bool, string>> SynchronizeUserSubscriptions();
        Task<KeyValuePair<bool, string>> UpdateSubscriptionVerification(ISubscriptionDetail source);
        Task<List<LevelPaymentBo>?> GetLevelRequestPaymentAmount(string externalId);
        Task<List<DiscountPaymentBo>?> GetDiscountRequestPaymentAmount(string externalId);
    }
}