using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IUserLockStatusRepository
    {
        Task<UserLockStatusBo?> GetStatus(string userId);
        Task<KeyValuePair<bool, string>> AddIncident(User user);
        Task<KeyValuePair<bool, string>> ClearSuspension(string userId);
        Task<KeyValuePair<bool, string>> Suspend(UserLockStatusBo userStatus);
        Task<KeyValuePair<bool, string>> Unlock(UserLockStatusBo userStatus);
    }
}
