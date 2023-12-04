using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class UserProfileRepository : BaseRepository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<UserProfile>> GetAll(User user)
        {
            using var connection = _context.CreateConnection();
            var parm = new UserProfile { UserId = user.Id ?? string.Empty };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            return await _command.QueryAsync<UserProfile>(connection, sql, parms);
        }

        public async Task<bool> DoesRecordExist(User user, string profileId)
        {
            using var connection = _context.CreateConnection();
            var parm = new UserProfile
            {
                UserId = user.Id ?? string.Empty,
                ProfileMapId = profileId
            };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            var record = await _command.QuerySingleOrDefaultAsync<UserProfile>(connection, sql, parms);
            return record != null;
        }
    }
}