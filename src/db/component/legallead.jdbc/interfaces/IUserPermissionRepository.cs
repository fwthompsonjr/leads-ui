using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IUserPermissionRepository
    {
        Task<IEnumerable<UserPermission>> GetAll();

        Task<UserPermission?> GetById(string id);

        Task Create(UserPermission user);

        Task Update(UserPermission user);

        Task Delete(string id);
        Task<IEnumerable<UserPermission>> GetAll(User user);
        Task<bool> DoesRecordExist(User user, string permissionId);
    }
}