namespace legallead.permissions.api.Interfaces
{
    public interface ILeadAuthenicationService
    {
        Task<bool> ChangeCountyCredentialAsync(string userId, string oldPassword, string newPassword);
        Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<bool> ChangeCountyPermissionAsync(string userId, string countyList);

        Task<string> CreateLoginAsync(string userName, string password);
        Task<string> LoginAsync(string userName, string password);

        Task<string> GetCountyCredentialAsync(string userId, string county);
        Task<string> GetCountyPermissionAsync(string userId);
    }
}