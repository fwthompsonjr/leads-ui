using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using Newtonsoft.Json;

namespace legallead.jdbc.implementations
{
    public class DbHistoryRepository(DataContext context) :
        BaseRepository<SearchDto>(context), IDbHistoryRepository
    {
        public async Task<DbSearchHistoryBo?> BeginAsync(DbHistoryRequest request)
        {
            var prc = ProcedureNames.BeginProc;
            var payload = Serialize(request);
            var parameters = new DynamicParameters();
            parameters.Add(jsParameter, payload);
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<DbSearchHistoryDto>(connection, prc, parameters);
                if (response == null) return default;
                return MapFrom(response);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<DbSearchHistoryBo?> CompleteAsync(DbHistoryRequest request)
        {
            var prc = ProcedureNames.CompleteProc;
            var payload = Serialize(request);
            var parameters = new DynamicParameters();
            parameters.Add(jsParameter, payload);
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<DbSearchHistoryDto>(connection, prc, parameters);
                if (response == null) return default;
                return MapFrom(response);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<DbSearchHistoryResultBo>?> FindAsync(string id)
        {
            var prc = ProcedureNames.FindProc;
            var parameters = new DynamicParameters();
            parameters.Add("search_id", id);
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<DbSearchHistoryResultDto>(connection, prc, parameters);
                if (response == null) return default;
                var list = new List<DbSearchHistoryResultBo>();
                response.ToList().ForEach(r =>
                {
                    list.Add(MapFrom(r));
                });
                return list;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<bool> UploadAsync(DbUploadRequest request)
        {
            var prc = ProcedureNames.UploadProc;
            var parameters = new DynamicParameters();
            parameters.Add("search_id", request.Id);
            parameters.Add("js_data", JsonConvert.SerializeObject(request.Contents));
            try
            {
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static DbSearchHistoryBo MapFrom(DbSearchHistoryDto response)
        {
            return new()
            {
                Id = response.Id,
                CountyId = response.CountyId,
                RecordCount = response.RecordCount,
                SearchDate = response.SearchDate,
                SearchTypeId = response.SearchTypeId,
                CaseTypeId = response.CaseTypeId,
                DistrictCourtId = response.DistrictCourtId,
                DistrictSearchTypeId = response.DistrictSearchTypeId,
                CreateDate = response.CreateDate,
                CompleteDate = response.CompleteDate
            };
        }

        private static DbSearchHistoryResultBo MapFrom(DbSearchHistoryResultDto response)
        {
            return new()
            {
                Id = response.Id,
                SearchHistoryId = response.SearchHistoryId ?? string.Empty,
                Name = response.Name ?? string.Empty,
                Zip = response.Zip ?? string.Empty,
                Address1 = response.Address1 ?? string.Empty,
                Address2 = response.Address2 ?? string.Empty,
                Address3 = response.Address3 ?? string.Empty,
                CaseNumber = response.CaseNumber ?? string.Empty,
                DateFiled = response.DateFiled ?? string.Empty,
                Court = response.Court ?? string.Empty,
                CaseType = response.CaseType ?? string.Empty,
                CaseStyle = response.CaseStyle ?? string.Empty,
                Plaintiff = response.Plaintiff ?? string.Empty,
                CreateDate = response.CreateDate
            };
        }

        private static string Serialize(DbHistoryRequest request)
        {
            var data = new[] { request };
            return JsonConvert.SerializeObject(data);
        }

        private const string jsParameter = "js_parameter";

        private static class ProcedureNames
        {
            public const string BeginProc = "CALL USP_DBSEARCH_BEGIN( ? );";
            public const string CompleteProc = "CALL USP_DBSEARCH_COMPLETE( ? );";
            public const string FindProc = "CALL USP_DBSEARCH_QUERY_RESULT( ? );";
            public const string UploadProc = "CALL USP_DBSEARCH_UPLOAD_RESULT( ?, ? );";
        }
    }
}