using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class UserTokenRepository : BaseRepository<UserRefreshToken>, IUserTokenRepository
    {
        public UserTokenRepository(DataContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserRefreshToken>> GetAll(User user)
        {
            using var connection = _context.CreateConnection();
            var parm = new UserRefreshToken { UserId = user.Id ?? string.Empty };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            return await _command.QueryAsync<UserRefreshToken>(connection, sql, parms);
        }

        public async Task<UserRefreshToken> Add(UserRefreshToken token)
        {
            token.Id = Guid.NewGuid().ToString("D");
            await Create(token);
            return token;
        }

        public async Task<UserRefreshToken?> Find(string userId, string refreshToken)
        {
            using var connection = _context.CreateConnection();
            const string sproc = "CALL USP_GET_REFRESH_TOKEN( '{0}', '{1}' );";
            var command = string.Format(sproc, userId, refreshToken);
            return await _command.QuerySingleOrDefaultAsync<UserRefreshToken>(connection, command);
        }

        public async Task DeleteTokens(User user)
        {
            var current = await GetAll(user);
            if (current == null) return;
            current.ToList().ForEach(async token =>
            {
                if (!string.IsNullOrEmpty(token.Id)) { await Delete(token.Id); }
            });
        }
    }
}