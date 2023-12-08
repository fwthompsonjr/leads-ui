using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IUserPermissionViewRepository
    {
        Task<IEnumerable<UserPermissionView>> GetAll();

        Task<IEnumerable<UserPermissionView>> GetAll(User user);

        Task<UserPermissionView?> GetById(string id);

        Task Create(UserPermissionView user);

        Task Update(UserPermissionView user);

        Task Delete(string id);

        Task<bool> DoesRecordExist(User user, string permissionId);
    }
}