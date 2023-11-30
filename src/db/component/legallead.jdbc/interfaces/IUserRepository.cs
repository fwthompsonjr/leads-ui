using legallead.jdbc.entities;

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
    }
}