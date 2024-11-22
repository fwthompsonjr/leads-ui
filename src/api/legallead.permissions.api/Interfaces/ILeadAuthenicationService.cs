using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Interfaces
{
    public interface ILeadAuthenicationService
    {
        LeadUserModel? GetUserModel(HttpRequest? request, string reason);

        Task<bool> ChangeCountyCredentialAsync(string userId, string county, string userName, string password);
        Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<bool> ChangeCountyPermissionAsync(string userId, string countyList);

        Task<string> CreateLoginAsync(string userName, string password, string email);
        Task<string> LoginAsync(string userName, string password);

        Task<string> GetCountyCredentialAsync(string userId, string county);
        Task<string> GetCountyPermissionAsync(string userId);
        Task<LeadUserModel?> GetModelByIdAsync(string id);
        KeyValuePair<bool, string> VerifyCountyList(string countyList);
        Task<bool> AddCountyUsageAsync(string userId, string county, int monthlyUsage);
        Task<List<GetUsageUserByIdResponse>> GetUsageUserByIdAsync(string userId);
        Task<bool> AddCountyUsageIncidentAsync(string userId, string county, int monthlyUsage, string searchRange = "");
    }
}