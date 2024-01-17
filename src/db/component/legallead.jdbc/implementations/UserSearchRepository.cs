using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using System.Text;

namespace legallead.jdbc.implementations
{
    public class UserSearchRepository : BaseRepository<SearchDto>, IUserSearchRepository
    {
        public UserSearchRepository(DataContext context) : base(context)
        {
        }
        public async Task<IEnumerable<SearchDtoHeader>> History(string userId)
        {
            const string prc = "CALL USP_QUERY_USER_SEARCH( '{0}' );";
            var command = string.Format(prc, userId);
            using var connection = _context.CreateConnection();
            var response = await _command.QueryAsync<SearchDto>(connection, command);
            var translation = response.Select(x => new SearchDtoHeader
            {
                Id = x.Id,
                UserId = x.UserId,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                EstimatedRowCount = x.ExpectedRows,
                CreateDate = x.CreateDate
            });
            return translation;
        }

        public async Task<KeyValuePair<bool, string>> Append(SearchTargetTypes search, string? id, object data, string? keyname = null)
        {
            try
            {
                var procedure = AppendProcs[search];
                var dapperParm = new DynamicParameters();
                dapperParm.Add("searchItemId", id);
                dapperParm.Add("jscontent", data);
                if (keyname != null)
                {
                    dapperParm.Add("stagingName", keyname);
                    var isByte = data is byte[];
                    dapperParm.Add("isByteArray", Convert.ToByte(isByte ? 1 : 0));
                }
                using (var connection = _context.CreateConnection())
                    await _command.ExecuteAsync(connection, procedure, dapperParm);
                return new KeyValuePair<bool, string>(true, "Command executed succesfully");
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> Begin(string userId, string payload)
        {
            const string procedure = "CALL USP_APPEND_USER_SEARCH_AND_REQUEST ( ?, ?, ?, ? );";

            try
            {
                var searchGuid = Guid.NewGuid().ToString("D");
                var dapperParm = new DynamicParameters();
                dapperParm.Add("userRefId", userId);
                dapperParm.Add("searchGuid", searchGuid);
                dapperParm.Add("searchStart", DateTime.UtcNow);
                dapperParm.Add("jscontent", Encoding.UTF8.GetBytes(payload));
                using var connection = _context.CreateConnection();
                var inserted = await _command.QuerySingleOrDefaultAsync<SearchDto>(connection, procedure, dapperParm);
                if (inserted != null && !string.IsNullOrWhiteSpace(inserted.Id))
                {
                    return new KeyValuePair<bool, string>(true, inserted.Id);
                }
                return new KeyValuePair<bool, string>(false, "Insert new request failed to send response");
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> Complete(string id)
        {
            const string procedure = "CALL USP_UPDATE_USER_SEARCH_COMPLETION(?, ?, ?);";

            try
            {
                int? noRecordCount = null;
                var dapperParm = new DynamicParameters();
                dapperParm.Add("searchUid", id);
                dapperParm.Add("estimatedRecords", noRecordCount);
                dapperParm.Add("completionDate", DateTime.UtcNow);
                using (var connection = _context.CreateConnection())
                    await _command.ExecuteAsync(connection, procedure, dapperParm);
                return new KeyValuePair<bool, string>(true, "Record update completed.");
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateRowCount(string id, int rowCount)
        {
            const string procedure = "CALL USP_UPDATE_USER_SEARCH_COMPLETION(?, ?, ?);";

            try
            {
                DateTime? noDate = null;
                var dapperParm = new DynamicParameters();
                dapperParm.Add("searchUid", id);
                dapperParm.Add("estimatedRecords", rowCount);
                dapperParm.Add("completionDate", noDate);
                using (var connection = _context.CreateConnection())
                    await _command.ExecuteAsync(connection, procedure, dapperParm);
                return new KeyValuePair<bool, string>(true, "Record update completed.");
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public async Task<IEnumerable<SearchTargetModel>?> GetTargets(SearchTargetTypes search, string? userId, string? id)
        {
            string prc = QueryProcs[search];
            var parms = new DynamicParameters();
            parms.Add("userUid", userId);
            parms.Add("searchUid", id);
            using var connection = _context.CreateConnection();
            var response = await _command.QueryAsync<SearchTargetDto>(connection, prc, parms);
            if (response == null) return null;
            var translation = response.Select(x => new SearchTargetModel
            {
                Component = x.Component,
                SearchId = x.SearchId,
                LineNbr = x.LineNbr,
                Line = x.Line,
                CreateDate = x.CreateDate
            });
            return translation;
        }

        public async Task<KeyValuePair<bool, object>> GetStaged(string id, string keyname)
        {
            const string procedure = "CALL USP_FIND_USER_SEARCH_STAGING(?, ?);";

            try
            {
                var dapperParm = new DynamicParameters();
                dapperParm.Add("searchItemId", id);
                dapperParm.Add("stagingName", keyname);
                using var connection = _context.CreateConnection();
                var staging = await _command.QueryAsync<SearchStagingDto>(connection, procedure, dapperParm);
                return new KeyValuePair<bool, object>(true, staging);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, object>(false, ex.Message);
            }
        }

        private static readonly Dictionary<SearchTargetTypes, string> AppendProcs = new(){
            { SearchTargetTypes.Detail, "CALL USP_APPEND_USER_SEARCH_DETAIL( ?, ? );" },
            { SearchTargetTypes.Request, "CALL USP_APPEND_USER_SEARCH_REQUEST( ?, ? );" },
            { SearchTargetTypes.Response, "CALL USP_APPEND_USER_SEARCH_RESPONSE( ?, ? );" },
            { SearchTargetTypes.Status, "CALL USP_APPEND_USER_SEARCH_STATUS( ?, ? );" },
            { SearchTargetTypes.Staging, "CALL USP_APPEND_USER_SEARCH_STAGING( ?, ?, ?, ? );" },
            };
        private static readonly Dictionary<SearchTargetTypes, string> QueryProcs = new(){
            { SearchTargetTypes.Detail, "CALL USP_QUERY_USER_SEARCH_DETAIL( ?, ? );" },
            { SearchTargetTypes.Request, "CALL USP_QUERY_USER_SEARCH_REQUEST( ?, ? );" },
            { SearchTargetTypes.Response, "CALL USP_QUERY_USER_SEARCH_RESPONSE( ?, ? );" },
            { SearchTargetTypes.Status, "CALL USP_QUERY_USER_SEARCH_STATUS( ?, ? );" },
            { SearchTargetTypes.Staging, "CALL USP_QUERY_USER_SEARCH_STAGING( ?, ? );" },
            };
    }
}
