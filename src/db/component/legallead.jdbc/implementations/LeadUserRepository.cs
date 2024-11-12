using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Newtonsoft.Json;
using System.Data;

namespace legallead.jdbc.implementations
{
    public class LeadUserRepository(DataContext context) : BaseRepository<SearchDto>(context), ILeadUserRepository
    {
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
            bo.Keys.ForEach(async key =>
            {
                var id = bo.Keys.IndexOf(key);
                switch (id)
                {
                    case 0:
                        bo.UserData = JsonConvert.SerializeObject(response);
                        break;
                    case 1:
                        bo.CountyData = await GetJson<LeadUserCountyDto>("USP_GET_LEADUSER_COUNTY_SECRETS", response.Id, connection);
                        break;
                    case 2:
                        bo.IndexData = await GetJson<LeadUserCountyIndexDto>("USP_GET_LEADUSER_COUNTY_INDEXES", response.Id, connection);
                        break;
                    default:
                        break;
                }
            });
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