using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using Newtonsoft.Json;
using System.Globalization;

namespace legallead.jdbc.implementations
{
    public class HarrisLoadRepository(DataContext context) : BaseRepository<SearchDto>(context), IHarrisLoadRepository
    {
        public async Task<KeyValuePair<bool, string>> Append(string data)
        {
            const string prc = "CALL USP_UPSERT_HARRIS_CRIMINAL_DATA ( ? );";
            var parameters = new DynamicParameters();
            parameters.Add("json_data", data);
            try
            {
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return new(true, "Command completed");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }

        }

        public async Task<List<HarrisCriminalUploadBo>> Find(DateTime dte)
        {
            const string prc = "CALL USP_FIND_HARRISDB_BY_DATE ( ? );";
            var filingDate = dte.ToString("yyyyMMdd", CultureInfo.CurrentCulture);
            var parameters = new DynamicParameters();
            parameters.Add("filingDt", filingDate);
            using var connection = _context.CreateConnection();
            var response = await _command.QueryAsync<HarrisCriminalUploadDto>(connection, prc, parameters);
            if (!response.Any()) return [];
            var data = JsonConvert.SerializeObject(response);
            var mapped = JsonConvert.DeserializeObject<List<HarrisCriminalUploadBo>>(data) ?? [];
            return mapped;
        }

        public async Task<int> Count(DateTime dte)
        {
            const string prc = "CALL USP_COUNT_HARRISDB_BY_DATE ( ? );";
            var filingDate = dte.ToString("yyyyMMdd", CultureInfo.CurrentCulture);
            var parameters = new DynamicParameters();
            parameters.Add("filingDt", filingDate);
            using var connection = _context.CreateConnection();
            var response = await _command.QuerySingleOrDefaultAsync<HarrisCriminalCountDto>(connection, prc, parameters);
            if (response == null) return 0;
            return response.RecordCount.GetValueOrDefault();
        }
    }
}