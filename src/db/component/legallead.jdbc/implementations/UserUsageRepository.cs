using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace legallead.jdbc.implementations
{
    public class UserUsageRepository(DataContext context) :
        BaseRepository<DbCountyUsageRequestDto>(context), IUserUsageRepository
    {
        public async Task<DbCountyAppendLimitBo?> AppendUsageRecord(UserUsageAppendRecordModel model)
        {
            const string prc = ProcNames.APPEND_USAGE_RECORD;
            try
            {
                var list = new List<UserUsageAppendRecordModel> { model };
                var payload = JsonConvert.SerializeObject(list);
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.Js, payload);
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<DbCountyAppendLimitDto>(connection, prc, parameters);
                if (response == null) return default;
                return MapFrom(response);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<KeyValuePair<bool, string>> CompleteUsageRecord(UserUsageCompleteRecordModel model)
        {
            const string prc = ProcNames.COMPLETE_USAGE_RECORD;
            try
            {
                var payload = JsonConvert.SerializeObject(model);
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.Js, payload);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return new(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }

        public async Task<List<DbCountyUsageLimitBo>?> GetMonthlyLimit(string leadId)
        {
            const string prc = ProcNames.GET_MONTHLY_LIMIT_ALL;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.LeadId, leadId);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<DbCountyUsageLimitDto>(connection, prc, parameters);
                if (response == null) return default;
                var list = new List<DbCountyUsageLimitBo>();
                response.ToList().ForEach(r =>
                    list.Add(GenericMap<DbCountyUsageLimitDto, DbCountyUsageLimitBo>(r)));
                return list;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<DbCountyUsageLimitBo?> GetMonthlyLimit(string leadId, int countyId)
        {
            const string prc = ProcNames.GET_MONTHLY_LIMIT_BY_COUNTY;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.LeadId, leadId);
                parameters.Add(ProcParameterNames.CountyId, countyId);
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(connection, prc, parameters);
                if (response == null) return default;
                return GenericMap<DbCountyUsageLimitDto, DbCountyUsageLimitBo>(response);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<DbCountyUsageRequestBo>?> GetUsage(string leadId, DateTime searchDate, bool monthOnly = false)
        {
            string prc = monthOnly ?
                ProcNames.GET_USAGE_DETAIL_MTD :
                ProcNames.GET_USAGE_DETAIL_YTD;
            try
            {
                var names = await GetExcelNames(leadId);
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.LeadId, leadId);
                parameters.Add(ProcParameterNames.SearchDate, searchDate);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<DbCountyUsageRequestDto>(connection, prc, parameters);
                if (response == null) return default;
                var list = new List<DbCountyUsageRequestBo>();
                response.ToList().ForEach(r =>
                    list.Add(GenericMap<DbCountyUsageRequestDto, DbCountyUsageRequestBo>(r)));
                list.ForEach(r =>
                {
                    var name = names.Find(x => (x.RequestId ?? "").Equals(x.RequestId ?? "-"));
                    if (name != null) r.ExcelName = name.ShortFileName ?? string.Empty;
                });
                return list;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<DbUsageSummaryBo>?> GetUsageSummary(string leadId, DateTime searchDate, bool monthOnly = false)
        {
            string prc = monthOnly ?
                ProcNames.GET_USAGE_SUMMARY_MTD :
                ProcNames.GET_USAGE_SUMMARY_YTD;
            try
            {
                var names = await GetExcelNames(leadId);
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.LeadId, leadId);
                parameters.Add(ProcParameterNames.SearchDate, searchDate);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<DbUsageSummaryDto>(connection, prc, parameters);
                if (response == null) return default;
                var list = new List<DbUsageSummaryBo>();
                response.ToList().ForEach(r =>
                    list.Add(GenericMap<DbUsageSummaryDto, DbUsageSummaryBo>(r)));
                list.ForEach(r =>
                {
                    var name = names.Find(x => (x.RequestId ?? "").Equals(x.RequestId ?? "-"));
                    if (name != null) r.ExcelName = name.ShortFileName ?? string.Empty;
                });
                return list;
            }
            catch (Exception)
            {
                return default;
            }
        }



        public async Task<DbCountyUsageLimitBo?> SetMonthlyLimit(string leadId, int countyId, int monthLimit)
        {
            const string prc = ProcNames.SET_USAGE_LIMIT;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.LeadId, leadId);
                parameters.Add(ProcParameterNames.CountyId, countyId);
                parameters.Add(ProcParameterNames.MonthLimit, monthLimit);
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<DbCountyUsageLimitDto>(connection, prc, parameters);
                if (response == null) return default;
                return GenericMap<DbCountyUsageLimitDto, DbCountyUsageLimitBo>(response);
            }
            catch (Exception)
            {
                return default;
            }
        }

        private async Task<List<DbExcelNameDto>> GetExcelNames(string leadId)
        {
            const string prc = ProcNames.GET_USAGE_FILE_NAME;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.LeadId, leadId);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<DbExcelNameDto>(connection, prc, parameters);
                if (response == null) return [];
                return response.ToList();
            }
            catch (Exception)
            {
                return [];
            }
        }


        [ExcludeFromCodeCoverage(Justification = "Private member tested from public accessor")]
        private static DbCountyAppendLimitBo? MapFrom(DbCountyAppendLimitDto? source)
        {
            if (source == null) return default;
            return new() { Id = source.Id };
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public accessor")]
        private static T GenericMap<S, T>(S source) where T : class, new()
        {
            try
            {
                var src = JsonConvert.SerializeObject(source);
                return JsonConvert.DeserializeObject<T>(src) ?? new();
            }
            catch (Exception)
            {
                return new();
            }

        }

        private static class ProcNames
        {
            public const string APPEND_USAGE_RECORD = "CALL USP_USER_USAGE_APPEND_RECORD ( ? );";
            public const string COMPLETE_USAGE_RECORD = "CALL USP_USER_USAGE_COMPLETE_RECORD ( ? );";
            public const string GET_MONTHLY_LIMIT_ALL = "CALL USP_USER_USAGE_GET_MONTHLY_LIMIT_ALL ( ? );";
            public const string GET_MONTHLY_LIMIT_BY_COUNTY = "CALL USP_USER_USAGE_GET_MONTHLY_LIMIT ( ?, ? );";
            public const string GET_USAGE_DETAIL_MTD = "CALL USP_USER_USAGE_GET_DETAIL_MTD ( ?, ? );";
            public const string GET_USAGE_DETAIL_YTD = "CALL USP_USER_USAGE_GET_DETAIL_YTD ( ?, ? );";
            public const string GET_USAGE_FILE_NAME = "CALL USP_LEADUSER_EXL_GET_FILENAMES_BY_USERID ( ? );";
            public const string GET_USAGE_SUMMARY_MTD = "CALL USP_USER_USAGE_GET_SUMMARY_MTD ( ?, ? );";
            public const string GET_USAGE_SUMMARY_YTD = "CALL USP_USER_USAGE_GET_SUMMARY_YTD ( ?, ? );";
            public const string SET_USAGE_LIMIT = "CALL USP_USER_USAGE_SET_MONTHLY_LIMIT ( ?, ?, ? );";
        }

        private static class ProcParameterNames
        {
            public const string Js = "js_parameter";
            public const string LeadId = "lead_index";
            public const string CountyId = "county_id";
            public const string MonthLimit = "month_limit";
            public const string SearchDate = "search_date";
        }
    }
}