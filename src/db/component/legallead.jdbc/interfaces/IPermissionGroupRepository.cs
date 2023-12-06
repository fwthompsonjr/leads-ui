using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IPermissionGroupRepository
    {
        Task<IEnumerable<PermissionGroup>> GetAll();

        Task<PermissionGroup?> GetById(string id);

        Task Create(PermissionGroup user);

        Task Update(PermissionGroup user);

        Task Delete(string id);
    }
}