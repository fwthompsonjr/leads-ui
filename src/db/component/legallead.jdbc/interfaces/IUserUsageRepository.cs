using legallead.jdbc.entities;
using legallead.jdbc.models;

namespace legallead.jdbc.interfaces
{
    public interface IUserUsageRepository
    {
        /// <summary>
        /// adds user search request details to database
        /// </summary>
        /// <param name="model">append record details</param>
        /// <returns>the id of the record added if successful</returns>
		Task<DbCountyAppendLimitBo?> AppendUsageRecord(UserUsageAppendRecordModel model);

        /// <summary>
        /// updates the completion status of user search request
        /// </summary>
        /// <param name="model">record completion details</param>
        /// <returns>
        /// true when completion is success,
        /// otherwise false and message about failure state
        /// </returns>
        Task<KeyValuePair<bool, string>> CompleteUsageRecord(UserUsageCompleteRecordModel model);

        /// <summary>
        /// gets list of monthly limits for specified user for all counties
        /// </summary>
        /// <param name="leadId">unique lead identity</param>
        Task<List<DbCountyUsageLimitBo>?> GetMonthlyLimit(string leadId);
        /// <summary>
        /// gets monthly limits for specified user and county
        /// </summary>
        /// <param name="leadId">unique lead identity</param>
        /// <param name="countyId">county index</param>
        /// <returns></returns>
        Task<DbCountyUsageLimitBo?> GetMonthlyLimit(string leadId, int countyId);

        /// <summary>
        /// sets monthly limits for specified user and county
        /// </summary>
        /// <param name="leadId">unique lead identity</param>
        /// <param name="countyId">county index</param>
        /// <param name="monthLimit">maximun records per month limit</param>
        /// <returns></returns>
        Task<DbCountyUsageLimitBo?> SetMonthlyLimit(string leadId, int countyId, int monthLimit);
        /// <summary>
        /// gets monthly usage history for specified user
        /// </summary>
        /// <param name="leadId">unique lead identity</param>
        /// <param name="searchDate">date to search</param>
        /// <param name="monthOnly">limit search to month of search-date</param>
        /// <returns>
        /// when month only is true - matching searches for the month of search-date
        /// otherwise - matching searches for the year of search-date
        /// </returns>
        Task<List<DbCountyUsageRequestBo>?> GetUsage(string leadId, DateTime searchDate, bool monthOnly = false);

        /// <summary>
        /// gets monthly usage summary for specified user
        /// </summary>
        /// <param name="leadId">unique lead identity</param>
        /// <param name="searchDate">date to search</param>
        /// <param name="monthOnly">limit search to month of search-date</param>
        /// <returns></returns>
        Task<List<DbUsageSummaryBo>?> GetUsageSummary(string leadId, DateTime searchDate, bool monthOnly = false);
        /// <summary>
        /// gets excel file details for specified request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        Task<ExcelFileNameResponse> GetUsageFileDetails(string requestId);
        /// <summary>
        /// initialize offline search record and fetch search uid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<OfflineRequestModel> OfflineRequestBeginAsync(OfflineRequestModel model);

        /// <summary>
        /// update offline search record details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> OfflineRequestUpdateAsync(OfflineRequestModel model);

        /// <summary>
        /// get offline search record details for single request
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<OfflineStatusModel?> GetOfflineStatusAsync(OfflineRequestModel model);

        /// <summary>
        /// get offline search record details for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<OfflineStatusModel>?> GetOfflineStatusAsync(string userId);
        Task<bool> OfflineRequestTerminateAsync(OfflineRequestModel model);
        Task<OfflineDownloadModel?> OfflineRequestCanDownload(OfflineRequestModel model);
        Task<bool> OfflineRequestFlagAsDownloadedAsync(OfflineDownloadModel model);
        Task<bool> OfflineRequestSetCourtTypeAsync(OfflineRequestModel model);
        Task<bool> OfflineRequestSetSearchTypeAsync();
        Task<bool> OfflineRequestSyncHistoryAsync();
        Task<List<OfflineSearchTypeModel>?> GetOfflineGetSearchTypeAsync(string leadId);
        Task<OfflineCaseItemModel?> OfflineFindByCaseNumber(OfflineCaseItemModel model);
        Task<List<OfflineStatusModel>?> GetOfflineWorkQueueAsync();
    }
}
