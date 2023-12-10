using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;

namespace legallead.jdbc.implementations
{
    public class UserProfileHistoryRepository : BaseRepository<UserProfileHistory>, IUserProfileHistoryRepository
    {
        public UserProfileHistoryRepository(DataContext context) : base(context)
        {
        }

        public string SnapshotProcedureName => "CALL USP_APPEND_PROFILE_HISTORY('{0}', '{1}');";

        public async Task CreateSnapshot(User user, ProfileChangeTypes ProfileChange)
        {
            var changeId = ((int)ProfileChange).ToString("D2");
            var changeCode = $"UP{changeId}";
            var userindex = user.Id ?? Guid.Empty.ToString();
            var command = string.Format(SnapshotProcedureName, userindex, changeCode);
            using var connection = _context.CreateConnection();
            await _command.ExecuteAsync(connection, command);
        }

        public async Task<IEnumerable<UserProfileHistory>> GetAll(User user)
        {
            using var connection = _context.CreateConnection();
            var parm = new UserProfileHistory { UserId = user.Id ?? string.Empty };
            var sql = _sut.SelectSQL(parm);
            var parms = _sut.SelectParameters(parm);
            var list = await _command.QueryAsync<UserProfileHistory>(connection, sql, parms);
            if (list == null) return Array.Empty<UserProfileHistory>();
            var data = list.ToList();
            data.Sort((a, b) =>
            {
                var aa = a.GroupId.GetValueOrDefault().CompareTo(b.GroupId.GetValueOrDefault());
                if (aa != 0) return aa;
                return a.KeyName.CompareTo(b.KeyName);
            });
            return data;
        }

        public async Task<IEnumerable<UserProfileHistory>> GetLatest(User user)
        {
            var records = await GetAll(user);
            var mx = records.Max(x => x.GroupId.GetValueOrDefault());
            return records.ToList().FindAll(x => x.GroupId.GetValueOrDefault() == mx);
        }
    }
}