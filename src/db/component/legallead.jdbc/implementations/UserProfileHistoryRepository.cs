using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using System.Diagnostics;
using System.Text;

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
            await Task.Run(() =>
            {
                Histories.ForEach(async h =>
                {
                    using var connection = _context.CreateConnection();
                    var sb = new StringBuilder(h);
                    sb.Replace("'userindex'", $"'{userindex}'");
                    sb.Replace("'changecode'", $"'{changeCode}'");
                    var command = sb.ToString();
                    await _command.ExecuteAsync(connection, command);
                });
                Debugger.Break();
            });
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

        private static readonly string nw = Environment.NewLine;

        private static readonly List<string> Histories = new() {
            "-- update history for current user " + nw +
            "UPDATE USERPROFILEHISTORY h " + nw +
            "SET GroupId = CASE WHEN GroupId IS NULL THEN 0 ELSE GroupId - 1 END " + nw +
            "WHERE h.UserId = 'userindex';",

            "-- add change history line record(s) for user " + nw +
            "INSERT INTO USERPROFILECHANGE " + nw +
            "( " + nw +
            "UserId, GroupId, CreateDate " + nw +
            ")" + nw +
            "SELECT h.UserId, h.GroupId, h.CreateDate " + nw +
            "FROM " + nw +
            "(" + nw +
            "SELECT UserId, GroupId, Max( createdate ) createdate " + nw +
            "FROM USERPROFILEHISTORY " + nw +
            "WHERE UserId = 'userindex' " + nw +
            "GROUP BY UserId, GroupId " + nw +
            ") h " + nw +
            "LEFT JOIN USERPROFILECHANGE c " + nw +
            "ON     h.UserId = c.UserId " + nw +
            "AND    h.CreateDate = c.CreateDate " + nw +
            "WHERE  c.Id is null;",

            "-- synchronize group indexes " + nw +
            "UPDATE USERPROFILECHANGE C  " + nw +
            "JOIN (  " + nw +
            "    SELECT UserId, GroupId, Max( CreateDate ) CreateDate  " + nw +
            "    FROM USERPROFILEHISTORY  " + nw +
            "    WHERE UserId = 'userindex' " + nw +
            "    GROUP BY UserId, GroupId  " + nw +
            "  ) AS subquery " + nw +
            "  ON C.UserId = subquery.UserId  " + nw +
            "  AND C.CreateDate = subquery.CreateDate  " + nw +
            "SET   C.GroupId = subquery.GroupId  " + nw +
            "WHERE 1 = 1    " + nw +
            "AND C.UserId = subquery.UserId " + nw +
            "AND C.CreateDate = subquery.CreateDate  " + nw +
            "AND C.GroupId != subquery.GroupId;",

            "-- set change reason code " + nw +
            "UPDATE USERPROFILECHANGE " + nw +
            "SET  ReasonCode = 'changecode' " + nw +
            "WHERE " + nw +
            "EXISTS (SELECT 1 FROM REASONCODES WHERE ReasonCode = 'changecode') " + nw +
            "AND UserId = 'userindex' " + nw +
            "AND GroupId = 0 " + nw +
            "AND ReasonCode IS NULL;"
        };
    }
}