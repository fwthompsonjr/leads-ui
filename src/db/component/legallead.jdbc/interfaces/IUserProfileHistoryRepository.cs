using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IUserProfileHistoryRepository
    {
        string SnapshotProcedureName { get; }
        Task<IEnumerable<UserProfileHistory>> GetAll();

        Task<IEnumerable<UserProfileHistory>> GetAll(User user);

        Task<IEnumerable<UserProfileHistory>> GetLatest(User user);

        Task CreateSnapshot(User user, ProfileChangeTypes ProfileChange);
    }
}