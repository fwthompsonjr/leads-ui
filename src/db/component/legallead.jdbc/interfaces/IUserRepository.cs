using legallead.jdbc.entities;
using legallead.jdbc.models;

namespace legallead.jdbc.interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();

        Task<User?> GetById(string id);

        Task<User?> GetByName(string name);

        Task<User?> GetByEmail(string email);

        Task Create(User user);

        Task Update(User user);

        Task Delete(string id);
        Task<KeyValuePair<bool, User?>> IsValidUserAsync(UserModel model);
    }
}