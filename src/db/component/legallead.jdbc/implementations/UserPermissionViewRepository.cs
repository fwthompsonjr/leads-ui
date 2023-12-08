using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class UserPermissionViewRepository : BaseRepository<UserPermissionView>, IUserPermissionViewRepository
    {
        public UserPermissionViewRepository(DataContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserPermissionView>> GetAll(User user)
        {
            using var connection = _context.CreateConnection();
            var parm = new UserPermissionView { UserId = user.Id ?? string.Empty };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            var list = await _command.QueryAsync<UserPermissionView>(connection, sql, parms);
            if (list == null) return Array.Empty<UserPermissionView>();
            var data = list.ToList();
            data.Sort((a, b) => a.OrderId.CompareTo(b.OrderId));
            return data;
        }

        public async Task<bool> DoesRecordExist(User user, string permissionId)
        {
            using var connection = _context.CreateConnection();
            var parm = new UserPermissionView
            {
                UserId = user.Id ?? string.Empty,
                PermissionMapId = permissionId
            };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            var record = await _command.QuerySingleOrDefaultAsync<UserPermissionView>(connection, sql, parms);
            return record != null;
        }
    }
}