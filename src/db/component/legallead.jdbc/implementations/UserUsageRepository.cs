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
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.LeadId, leadId);
                parameters.Add(ProcParameterNames.SearchDate, searchDate);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<DbCountyUsageRequestDto>(connection, prc, parameters);
                if (response == null) return default;
                var list = new List<DbCountyUsageRequestBo>();
                response.ToList().ForEach(r =>
                    list.Add(GenericMap<DbCountyUsageRequestDto, DbCountyUsageRequestBo>(r)));
                list.ForEach(r => r.ExcelName = r.ShortFileName);
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


        public async Task<ExcelFileNameResponse> GetUsageFileDetails(string requestId)
        {
            var response = new ExcelFileNameResponse { Id = requestId };
            try
            {
                var name = await GetExcelDetail(requestId);
                if (name == null) return response;
                response.Name = name.ShortFileName ?? string.Empty;
                response.Password = name.FileToken ?? string.Empty;
                response.IsCompleted = name.CompleteDate.HasValue;
                return response;
            }
            catch (Exception)
            {
                return response;
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

        public async Task<bool> OfflineRequestFlagAsDownloadedAsync(OfflineDownloadModel model)
        {
            const string prc = ProcNames.OFFLINE_FLAG_DOWNLOADED;
            try
            {
                if (!Guid.TryParse(model.Id, out var _)) return false;
                if (!Guid.TryParse(model.RequestId, out var _)) return false;
                if (!model.CanDownload) return false;
                if (string.IsNullOrWhiteSpace(model.Workload)) return false;
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.RequestId, model.RequestId);
                parameters.Add(ProcParameterNames.Logs, model.Workload);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<OfflineRequestModel> OfflineRequestBeginAsync(OfflineRequestModel model)
        {
            const string prc = ProcNames.OFFLINE_BEGIN;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.RequestId, model.RequestId);
                parameters.Add(ProcParameterNames.Workload, model.Workload);
                parameters.Add(ProcParameterNames.Cookies, model.Cookie);
                parameters.Add(ProcParameterNames.ItemCount, model.RowCount);
                using var connection = _context.CreateConnection();
                var dto = await _command.QuerySingleOrDefaultAsync<OfflineBeginDto>(connection, prc, parameters);
                if (dto != null) model.OfflineId = dto.Id;
                return model;
            }
            catch (Exception)
            {
                return model;
            }
        }

        public async Task<bool> OfflineRequestUpdateAsync(OfflineRequestModel model)
        {
            const string prc = ProcNames.OFFLINE_UPDATE;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.RequestId, model.RequestId);
                parameters.Add(ProcParameterNames.Workload, model.Workload);
                parameters.Add(ProcParameterNames.Logs, model.Message);
                parameters.Add(ProcParameterNames.ItemCount, model.RowCount);
                parameters.Add(ProcParameterNames.RetryCount, model.RetryCount);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<OfflineStatusModel?> GetOfflineStatusAsync(OfflineRequestModel model)
        {
            const string prc = ProcNames.OFFLINE_GET_BY_ID;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.RequestId, model.RequestId);
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<OfflineStatusDto>(connection, prc, parameters);
                if (response == null) return default;
                return GenericMap<OfflineStatusDto, OfflineStatusModel>(response);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<List<OfflineStatusModel>?> GetOfflineStatusAsync(string userId)
        {
            const string prc = ProcNames.OFFLINE_GET_FOR_USER_ID;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.LeadId, userId);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<OfflineStatusDto>(connection, prc, parameters);
                if (response == null || !response.Any()) return [];
                return GenericMap<IEnumerable<OfflineStatusDto>, List<OfflineStatusModel>>(response);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<bool> OfflineRequestTerminateAsync(OfflineRequestModel model)
        {
            const string prc = ProcNames.OFFLINE_TERMINATE;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.RequestId, model.RequestId);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> OfflineRequestSetSearchTypeAsync()
        {
            const string prc = ProcNames.OFFLINE_SET_SEARCHTYPE;
            try
            {
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> OfflineRequestSetCourtTypeAsync(OfflineRequestModel model)
        {
            const string prc = ProcNames.OFFLINE_SET_COURTTYPE;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.RequestId, model.RequestId);
                parameters.Add(ProcParameterNames.Logs, model.Workload);
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc, parameters);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<OfflineDownloadModel?> OfflineRequestCanDownload(OfflineRequestModel model)
        {
            const string prc = ProcNames.OFFLINE_CAN_DOWNLOAD;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.RequestId, model.RequestId);
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<OfflineDownloadDto>(connection, prc, parameters);
                if (response == null) return default;
                return GenericMap<OfflineDownloadDto, OfflineDownloadModel>(response);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<OfflineCaseItemModel?> OfflineFindByCaseNumber(OfflineCaseItemModel model)
        {
            const string prc = ProcNames.OFFLINE_FIND_BY_CASENUMBER;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.CountyId, model.CountyId);
                parameters.Add(ProcParameterNames.CaseNo, model.CaseNumber);
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<OfflineCaseItemDto>(connection, prc, parameters);
                if (response == null) return default;
                return GenericMap<OfflineCaseItemDto, OfflineCaseItemModel>(response);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<bool> OfflineRequestSyncHistoryAsync()
        {
            const string prc = ProcNames.OFFLINE_SYNC_HISTORY;
            try
            {
                using var connection = _context.CreateConnection();
                await _command.ExecuteAsync(connection, prc);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<OfflineSearchTypeModel>?> GetOfflineGetSearchTypeAsync(string leadId)
        {
            const string prc = ProcNames.OFFLINE_SEARCH_TYPE_BY_USERID;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.LeadId, leadId);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<OfflineSearchTypeDto>(connection, prc, parameters);
                if (response == null) return default;
                return GenericMap<IEnumerable<OfflineSearchTypeDto>, List<OfflineSearchTypeModel>>(response);
            }
            catch (Exception)
            {
                return default;
            }
        }

        private async Task<DbExcelNameDto?> GetExcelDetail(string requestId)
        {
            const string prc = ProcNames.GET_USAGE_FILE_BY_ID;
            try
            {
                var json = JsonConvert.SerializeObject(new { idx = requestId });
                var parameters = new DynamicParameters();
                parameters.Add(ProcParameterNames.LeadId, json);
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<DbExcelNameDto>(connection, prc, parameters);
                return response;
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
                return [.. response];
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
            public const string GET_USAGE_FILE_BY_ID = "CALL USP_LEADUSER_EXL_GET_FILENAME_BY_ID ( ? );";
            public const string GET_USAGE_SUMMARY_MTD = "CALL USP_USER_USAGE_GET_SUMMARY_MTD ( ?, ? );";
            public const string GET_USAGE_SUMMARY_YTD = "CALL USP_USER_USAGE_GET_SUMMARY_YTD ( ?, ? );";
            public const string SET_USAGE_LIMIT = "CALL USP_USER_USAGE_SET_MONTHLY_LIMIT ( ?, ?, ? );";
            public const string OFFLINE_BEGIN = "CALL USP_OFFLINESEARCH_BEGIN ( ?, ?, ?, ? );";
            public const string OFFLINE_UPDATE = "CALL USP_OFFLINESEARCH_UPDATE ( ?, ?, ?, ?, ? );";
            public const string OFFLINE_GET_BY_ID = "CALL USP_OFFLINESEARCH_FETCH ( ? );";
            public const string OFFLINE_GET_FOR_USER_ID = "CALL USP_OFFLINESEARCH_FETCH_BY_USER_ID ( ? );";
            public const string OFFLINE_TERMINATE = "CALL USP_OFFLINESEARCH_TERMINATE ( ? );";
            public const string OFFLINE_CAN_DOWNLOAD = "CALL USP_OFFLINESEARCH_CAN_DOWNLOAD ( ? );";
            public const string OFFLINE_FLAG_DOWNLOADED = "CALL USP_OFFLINESEARCH_FLAG_DOWNLOAD ( ?, ? );";
            public const string OFFLINE_SET_COURTTYPE = "CALL USP_OFFLINESEARCH_SET_COURT_TYPE ( ?, ? );";
            public const string OFFLINE_SET_SEARCHTYPE = "CALL USP_OFFLINE_SET_SEARCH_TYPE_INTERNAL();";
            public const string OFFLINE_SYNC_HISTORY = "CALL USP_OFFLINESEARCH_SYNC_HISTORY();";
            public const string OFFLINE_SEARCH_TYPE_BY_USERID = "CALL USP_OFFLINESEARCH_SEARCHTYPE_BY_USER_ID ( ? );";
            public const string OFFLINE_FIND_BY_CASENUMBER = "CALL USP_OFFLINE_TRY_FIND_BY_CASENUMBER ( ?, ? );";
        }

        private static class ProcParameterNames
        {
            public const string Js = "js_parameter";
            public const string LeadId = "lead_index";
            public const string CountyId = "county_id";
            public const string MonthLimit = "month_limit";
            public const string SearchDate = "search_date";
            public const string RequestId = "request_index";
            public const string Workload = "work_load";
            public const string Logs = "js_logs";
            public const string Cookies = "js_cookie";
            public const string ItemCount = "item_count";
            public const string RetryCount = "retry_count";
            public const string CaseNo = "case_number";
        }
    }
}