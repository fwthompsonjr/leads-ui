using legallead.jdbc.entities;
using legallead.jdbc.models;

namespace legallead.jdbc.interfaces
{
    public interface IUserMangementRepository
    {
        Task<List<LeadUserAccountBo>?> GetAccountsAsync(UserManagementRequest request);
        Task<List<LeadUserCountyViewBo>?> GetCountyAsync(UserManagementRequest request);
        Task<List<LeadUserInvoiceBo>?> GetInvoiceAsync(UserManagementRequest request);
        Task<List<LeadCountyPricingBo>?> GetPricingAsync(UserManagementRequest request);
        Task<List<LeadUserProfileBo>?> GetProfileAsync(UserManagementRequest request);
        Task<List<LeadUserSearchBo>?> GetSearchAsync(UserManagementRequest request);
        Task<bool> UpdateProfileAsync(UserManagementRequest request);
        Task<bool> UpdateUsageLimitAsync(UserManagementRequest request);
    }
}
