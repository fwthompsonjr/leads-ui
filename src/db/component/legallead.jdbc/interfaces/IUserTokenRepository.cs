using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IUserTokenRepository
    {
        Task<IEnumerable<UserRefreshToken>> GetAll();

        Task<UserRefreshToken?> GetById(string id);

        Task Create(UserRefreshToken user);

        Task Update(UserRefreshToken user);

        Task Delete(string id);
        Task<UserRefreshToken> Add(UserRefreshToken token);
        Task<UserRefreshToken?> Find(string userId, string refreshToken);
        Task DeleteTokens(User user);
    }
}