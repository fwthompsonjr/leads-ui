
namespace legallead.permissions.api.Interfaces
{
    public interface ICustomerLockInfrastructure
    {
        Task AddIncidentAsync(string userId);
        Task<bool> IsAccountLockedAsync(string userId);
        Task<bool> LockAccountAsync(string userId);
        Task<bool> UnLockAccountAsync(string userId);
    }
}
