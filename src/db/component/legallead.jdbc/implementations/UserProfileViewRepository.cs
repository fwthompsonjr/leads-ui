using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class UserProfileViewRepository : BaseRepository<UserProfileView>, IUserProfileViewRepository
    {
        public UserProfileViewRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<UserProfileView>> GetAll(User user)
        {
            using var connection = _context.CreateConnection();
            var parm = new UserProfileView { UserId = user.Id ?? string.Empty };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            var list = await _command.QueryAsync<UserProfileView>(connection, sql, parms);
            if (list == null) return Array.Empty<UserProfileView>();
            var data = list.ToList();
            data.Sort((a, b) => a.OrderId.CompareTo(b.OrderId));
            return data;
        }

        public async Task<bool> DoesRecordExist(User user, string profileId)
        {
            using var connection = _context.CreateConnection();
            var parm = new UserProfileView
            {
                UserId = user.Id ?? string.Empty,
                ProfileMapId = profileId
            };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            var record = await _command.QuerySingleOrDefaultAsync<UserProfileView>(connection, sql, parms);
            return record != null;
        }
    }
}