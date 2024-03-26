using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Newtonsoft.Json;

namespace legallead.jdbc.implementations
{
    public class UserLockStatusRepository : BaseRepository<UserLockStatusDto>, IUserLockStatusRepository
    {
        public UserLockStatusRepository(DataContext context) : base(context)
        {
        }
        public async Task<KeyValuePair<bool, string>> AddIncident(User user)
        {
            const string prc = "CALL USP_INCREMENT_USER_LOCK_STATUS( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("user_index", user.Id);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parms);
                return new(true, "Command executed successfully");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> ClearSuspension(string userId)
        {
            const string prc = "CALL USP_CLEAR_EXTENDED_USER_LOCK_STATUS( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("user_index", userId);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parms);
                return new(true, "Command executed successfully");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }

        public async Task<UserLockStatusBo?> GetStatus(string userId)
        {
            const string prc = "CALL USP_GET_USER_LOCK_STATUS( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("user_index", userId);
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<UserLockStatusDto>(connection, prc, parms);
                if (response == null) return null;
                var json = JsonConvert.SerializeObject(response);
                var bo = JsonConvert.DeserializeObject<UserLockStatusBo>(json);
                return bo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<KeyValuePair<bool, string>> Suspend(UserLockStatusBo userStatus)
        {
            const string prc = "CALL USP_CREATE_EXTENDED_USER_LOCK( ?, ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("user_index", userStatus.UserId);
                parms.Add("expirationDate", userStatus.UserId);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parms);
                return new(true, "Command executed successfully");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> Unlock(UserLockStatusBo userStatus)
        {
            const string prc = "CALL USP_CLEAR_USER_LOCK_STATUS( ? );";
            try
            {
                var parms = new DynamicParameters();
                parms.Add("user_index", userStatus.UserId);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parms);
                return new(true, "Command executed successfully");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }
    }
}
