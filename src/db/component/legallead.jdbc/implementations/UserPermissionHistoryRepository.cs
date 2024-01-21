using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class UserPermissionHistoryRepository : BaseRepository<UserPermissionHistory>, IUserPermissionHistoryRepository
    {
        public UserPermissionHistoryRepository(DataContext context) : base(context)
        {
        }

        public string SnapshotProcedureName => "CALL USP_APPEND_PERMISSION_HISTORY('{0}', '{1}');";

        public async Task CreateSnapshot(User user, PermissionChangeTypes permissionChange)
        {
            var changeId = ((int)permissionChange).ToString("D2");
            var changeCode = $"UC{changeId}";
            var userindex = user.Id ?? Guid.Empty.ToString();
            var command = string.Format(SnapshotProcedureName, userindex, changeCode);
            using var connection = _context.CreateConnection();
            await _command.ExecuteAsync(connection, command);
        }

        public async Task<IEnumerable<UserPermissionHistory>> GetAll(User user)
        {
            using var connection = _context.CreateConnection();
            var parm = new UserPermissionHistory { UserId = user.Id ?? string.Empty };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            var list = await _command.QueryAsync<UserPermissionHistory>(connection, sql, parms);
            var data = list.ToList();
            data.Sort((a, b) =>
            {
                var aa = a.GroupId.GetValueOrDefault().CompareTo(b.GroupId.GetValueOrDefault());
                if (aa != 0) return aa;
                return a.KeyName.CompareTo(b.KeyName);
            });
            return data;
        }

        public async Task<IEnumerable<UserPermissionHistory>> GetLatest(User user)
        {
            var records = await GetAll(user);
            var mx = records.Max(x => x.GroupId.GetValueOrDefault());
            return records.ToList().FindAll(x => x.GroupId.GetValueOrDefault() == mx);
        }
    }
}