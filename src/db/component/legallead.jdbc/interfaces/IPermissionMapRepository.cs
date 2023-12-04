using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IPermissionMapRepository
    {
        Task<IEnumerable<PermissionMap>> GetAll();

        Task<PermissionMap?> GetById(string id);

        Task Create(PermissionMap user);

        Task Update(PermissionMap user);

        Task Delete(string id);
    }
}