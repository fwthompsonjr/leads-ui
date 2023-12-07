using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IUserPermissionHistoryRepository
    {
        string SnapshotProcedureName { get; }
        Task<IEnumerable<UserPermissionHistory>> GetAll();

        Task<IEnumerable<UserPermissionHistory>> GetAll(User user);

        Task<IEnumerable<UserPermissionHistory>> GetLatest(User user);

        Task CreateSnapshot(User user);
    }
}