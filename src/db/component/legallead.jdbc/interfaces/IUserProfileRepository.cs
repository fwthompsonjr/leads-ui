using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetById(string id);

        Task Create(UserProfile user);

        Task Update(UserProfile user);

        Task Delete(string id);

        Task<IEnumerable<UserProfile>> GetAll();

        Task<IEnumerable<UserProfile>> GetAll(User user);

        Task<bool> DoesRecordExist(User user, string profileId);
    }
}