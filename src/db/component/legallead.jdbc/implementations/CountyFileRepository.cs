using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;

namespace legallead.jdbc.implementations
{
    public class CountyFileRepository(DataContext context) :
        BaseRepository<DbCountyFileDto>(context), ICountyFileRepository
    {

        public async Task<DbCountyFileModel?> GetContentAsync(DbCountyFileModel request)
        {
            _ = await InitializeAsync();
            var prc = ProcedureNames.GetProc;
            var parameters = new DynamicParameters();
            parameters.Add(fileId, request.Id);
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<DbCountyFileDto>(connection, prc, parameters);
                return MapFrom(response);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<bool> InitializeAsync()
        {
            var prc = ProcedureNames.InitProc;
            try
            {
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc);
                return true;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateContentAsync(DbCountyFileModel request)
        {
            var current = await GetContentAsync(request);
            if (current == null)
            {
                return new(false, "Invalid record index.");
            }
            var prc = ProcedureNames.SetContentAndStatusProc;
            var parameters = new DynamicParameters();
            parameters.Add(fileId, request.Id);
            parameters.Add(fileStatusName, current.FileStatus);
            parameters.Add(fileContent, request.FileContent);
            try
            {
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return new(true, "");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateTypeAsync(DbCountyFileModel request)
        {
            var prc = ProcedureNames.SetFileTypeProc;
            var parameters = new DynamicParameters();
            parameters.Add(fileId, request.Id);
            parameters.Add(fileTypeName, request.FileType);
            try
            {
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return new(true, "");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateStatusAsync(DbCountyFileModel request)
        {
            var current = await GetContentAsync(request);
            if (current == null)
            {
                return new(false, "Invalid record index.");
            }
            var prc = ProcedureNames.SetContentAndStatusProc;
            var parameters = new DynamicParameters();
            parameters.Add(fileId, request.Id);
            parameters.Add(fileStatusName, request.FileStatus);
            parameters.Add(fileContent, current.FileContent);
            try
            {
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return new(true, "");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }
        private static DbCountyFileModel? MapFrom(DbCountyFileDto? dto)
        {
            if (dto == null) return null;
            var response = new DbCountyFileModel
            {
                Id = dto.DbCountyFileId ?? string.Empty,
                FileContent = dto.Content ?? string.Empty,
                FileStatus = FindValue(dto.FileStatusId.GetValueOrDefault(), statusNames),
                FileType = FindValue(dto.FileTypeId.GetValueOrDefault(), typeNames),
            };

            return response;
        }


        private static string FindValue(int index, Dictionary<int, string> values)
        {
            if (values.TryGetValue(index, out var value)) return value;
            return values.Values.First();
        }
        private const string fileId = "fileUuid";
        private const string fileTypeName = "fileTypeName";
        private const string fileStatusName = "fileStatusName";
        private const string fileContent = "fileContent";
        private static readonly Dictionary<int, string> statusNames = new()
        {
            {0, "EMPTY" },
            {10, "ENCODED" },
            {20, "DECODED" },
            {30, "DOWNLOADED" }
        };
        private static readonly Dictionary<int, string> typeNames = new()
        {
            {0, "NONE" },
            {10, "EXL" },
            {20, "CSV" },
            {30, "JSON" }
        };
        private static class ProcedureNames
        {
            public const string InitProc = "CALL USP_COUNTY_FILE_INITIALIZE_DATA();";
            public const string GetProc = "CALL USP_COUNTY_FILE_GET_BY_ID( ? );";
            public const string SetFileTypeProc = "CALL USP_COUNTY_FILE_SET_TYPE( ?, ? );";
            public const string SetContentAndStatusProc = "CALL USP_COUNTY_FILE_SET_CONTENT_AND_STATUS( ?, ?, ? );";
        }
    }
}