using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Newtonsoft.Json;
using System.Data;

namespace legallead.jdbc.implementations
{
    public class LeadUserRepository(DataContext context) : BaseRepository<SearchDto>(context), ILeadUserRepository
    {
        public async Task<string> AddAccount(LeadUserDto user)
        {
            const string command = "CALL USP_LEADUSER_ADD_ACCOUNT ( ?, ?, ?, ?, ? )";
            return await AddOrUpdateAccount(user, command);
        }

        public async Task<bool> AddCountyPermissions(LeadUserCountyIndexDto userPermissions)
        {
            const string command = "CALL USP_LEADUSER_ADD_ACCOUNT_PERMISSION ( ?, ? )";
            return await AddOrUpdatePermissions(userPermissions, command);
        }

        public async Task<bool> AddCountyToken(LeadUserCountyDto userCounty)
        {
            const string command = "CALL USP_LEADUSER_ADD_ACCOUNT_TOKEN ( ?, ?, ?, ?, ? )";
            return await AddOrUpdateToken(userCounty, command);
        }

        public async Task<LeadUserBo?> GetUserById(string userId)
        {
            const string prc = "CALL USP_GET_LEADUSER_BY_Id( '{0}' );";
            var command = string.Format(prc, userId);
            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<LeadUserDto>(connection, command);
            if (response == null) return null;
            var bo = new LeadUserBo
            {
                Id = response.Id,
                UserName = response.UserName ?? string.Empty,
            };
            return await GetUserAttributes(connection, response, bo);
        }

        public async Task<LeadUserBo?> GetUser(string userName)
        {
            const string prc = "CALL USP_GET_LEADUSER_BY_USERNAME( '{0}' );";
            var command = string.Format(prc, userName);
            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<LeadUserDto>(connection, command);
            if (response == null) return null;
            var bo = new LeadUserBo
            {
                Id = response.Id,
                UserName = response.UserName ?? string.Empty,
            };
            return await GetUserAttributes(connection, response, bo);
        }

        public async Task<bool> UpdateAccount(LeadUserDto user)
        {
            const string command = "CALL USP_LEADUSER_UPDATE_ACCOUNT ( ?, ?, ?, ?, ? )";
            var tmpUserName = user.UserName ?? string.Empty;
            user.UserName = user.Id;
            var indx = await AddOrUpdateAccount(user, command);
            user.UserName = tmpUserName;
            return !string.IsNullOrEmpty(indx);
        }

        public async Task<bool> UpdateCountyPermissions(LeadUserCountyIndexDto userPermissions)
        {
            const string command = "CALL USP_LEADUSER_UPDATE_ACCOUNT_PERMISSION ( ?, ? )";
            return await AddOrUpdatePermissions(userPermissions, command);
        }

        public async Task<bool> UpdateCountyToken(LeadUserCountyDto userCounty)
        {
            const string command = "CALL USP_LEADUSER_UPDATE_ACCOUNT_TOKEN ( ?, ?, ?, ?, ? )";
            return await AddOrUpdateToken(userCounty, command);
        }



        private async Task<string> AddOrUpdateAccount(LeadUserDto user, string command)
        {
            if (string.IsNullOrEmpty(user.UserName)) return string.Empty;
            if (string.IsNullOrEmpty(user.Phrase)) return string.Empty;
            if (string.IsNullOrEmpty(user.Vector)) return string.Empty;
            if (string.IsNullOrEmpty(user.Token)) return string.Empty;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("accountName", user.UserName);
                parameters.Add("email_address", user.Email ?? string.Empty);
                parameters.Add("passPhrase", user.Phrase);
                parameters.Add("tokenKey", user.Vector);
                parameters.Add("tokenCode", user.Token);
                using var connection = _context.CreateConnection();

                var response = await _command.QuerySingleOrDefaultAsync<LeadUserDto>(connection, command, parameters);
                if (response == null || string.IsNullOrEmpty(response.Id)) return string.Empty;
                return response.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private async Task<bool> AddOrUpdatePermissions(LeadUserCountyIndexDto userPermissions, string command)
        {
            if (string.IsNullOrEmpty(userPermissions.LeadUserId)) return false;
            if (string.IsNullOrEmpty(userPermissions.CountyList)) return false;
            var parameters = new DynamicParameters();
            parameters.Add("userId", userPermissions.LeadUserId);
            parameters.Add("county_list", userPermissions.CountyList);

            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<LeadUserCountyIndexDto>(connection, command, parameters);
            if (response == null || string.IsNullOrEmpty(response.Id)) return false;
            return true;
        }

        private async Task<bool> AddOrUpdateToken(LeadUserCountyDto userCounty, string command)
        {
            if (string.IsNullOrEmpty(userCounty.LeadUserId)) return false;
            if (string.IsNullOrEmpty(userCounty.CountyName)) return false;
            if (string.IsNullOrEmpty(userCounty.Phrase)) return false;
            if (string.IsNullOrEmpty(userCounty.Vector)) return false;
            if (string.IsNullOrEmpty(userCounty.Token)) return false;
            var parameters = new DynamicParameters();
            parameters.Add("userId", userCounty.LeadUserId);
            parameters.Add("county_name", userCounty.CountyName);
            parameters.Add("passPhrase", userCounty.Phrase);
            parameters.Add("tokenKey", userCounty.Vector);
            parameters.Add("tokenCode", userCounty.Token);

            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<LeadUserCountyDto>(connection, command, parameters);
            if (response == null || string.IsNullOrEmpty(response.Id)) return false;
            return true;
        }

        private async Task<LeadUserBo?> GetUserAttributes(IDbConnection connection, LeadUserDto? response, LeadUserBo bo)
        {
            for (var i = 0; i < bo.Keys.Count; i++)
            {
                if (i > 2) break;
                if (i == 0)
                {
                    bo.UserData = JsonConvert.SerializeObject(response);
                    continue;
                }
                if (i == 1)
                {
                    bo.CountyData = await GetJson<LeadUserCountyDto>("USP_GET_LEADUSER_COUNTY_SECRETS", response.Id, connection);
                    continue;
                }
                bo.IndexData = await GetJson<LeadUserCountyIndexDto>("USP_GET_LEADUSER_COUNTY_INDEXES", response.Id, connection);
            }
            return bo;
        }

        private async Task<string> GetJson<D>(string procedureName, string userId, IDbConnection connection) where D : BaseDto, new()
        {
            var prc = $"CALL {procedureName} ( '{userId}' );";
            var response = await _command.QueryAsync<D>(connection, prc);
            if (response == null) return "[]";
            return JsonConvert.SerializeObject(response);
        }

    }
}