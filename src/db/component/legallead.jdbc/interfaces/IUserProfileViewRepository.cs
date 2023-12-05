using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IUserProfileViewRepository
    {
        Task<IEnumerable<UserProfileView>> GetAll();

        Task<IEnumerable<UserProfileView>> GetAll(User user);

        Task<UserProfileView?> GetById(string id);

        Task Create(UserProfileView user);

        Task Update(UserProfileView user);

        Task Delete(string id);

        Task<bool> DoesRecordExist(User user, string profileId);
    }
}