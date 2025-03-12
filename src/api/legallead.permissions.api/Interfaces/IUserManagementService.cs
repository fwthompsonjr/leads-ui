using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface IUserManagementService
    {
        Task<UserManagementOperationResponse> ExecuteAsync(UserManagementOperationRequest request);
    }
}
