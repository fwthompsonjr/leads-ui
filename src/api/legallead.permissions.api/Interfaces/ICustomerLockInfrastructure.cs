
namespace legallead.permissions.api.Interfaces
{
    public interface ICustomerLockInfrastructure
    {
        Task AddIncident(string userId);
        Task<bool> IsAccountLocked(string userId);
        Task<bool> LockAccount(string userId);
        Task<bool> UnLockAccount(string userId);
    }
}
