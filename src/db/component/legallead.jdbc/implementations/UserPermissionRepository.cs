using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class UserPermissionRepository : BaseRepository<UserPermission>, IUserPermissionRepository
    {
        public UserPermissionRepository(DataContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserPermission>> GetAll(User user)
        {
            using var connection = _context.CreateConnection();
            var parm = new UserPermission { UserId = user.Id ?? string.Empty };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            return await _command.QueryAsync<UserPermission>(connection, sql, parms);
        }

        public async Task<bool> DoesRecordExist(User user, string permissionId)
        {
            using var connection = _context.CreateConnection();
            var parm = new UserPermission
            {
                UserId = user.Id ?? string.Empty,
                PermissionMapId = permissionId
            };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            var record = await _command.QuerySingleOrDefaultAsync<UserPermission>(connection, sql, parms);
            return record != null;
        }
    }
}