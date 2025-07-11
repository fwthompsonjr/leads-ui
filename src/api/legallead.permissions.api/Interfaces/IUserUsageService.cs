using legallead.permissions.api.Models;

namespace legallead.permissions.api.Interfaces
{
    public interface IUserUsageService
    {
        /// <summary>
        /// adds user search request details to database
        /// </summary>
        Task<AppendUsageRecordResponse?> AppendUsageRecordAsync(AppendUsageRecordRequest model);

        /// <summary>
        /// updates the completion status of user search request
        /// </summary>
        Task<CompleteUsageRecordResponse> CompleteUsageRecordAsync(CompleteUsageRecordRequest model);

        /// <summary>
        /// gets list of monthly limits for specified user for all counties
        /// </summary>
        Task<GetMonthlyLimitResponse> GetMonthlyLimitAsync(GetMonthlyLimitRequest model);

        /// <summary>
        /// sets monthly limits for specified user and county
        /// </summary>
        Task<SetMonthlyLimitResponse> SetMonthlyLimitAsync(SetMonthlyLimitRequest model);

        /// <summary>
        /// gets monthly usage history for specified user
        /// </summary>
        Task<GetUsageResponse> GetUsageAsync(GetUsageRequest model);

        /// <summary>
        /// gets monthly usage summary for specified user
        /// </summary>
        Task<GetUsageResponse> GetUsageSummaryAsync(GetUsageRequest model);
        Task<GetExcelDetailByIdResponse> GetExcelDetailAsync(string id);
        Task<OfflineDataModel> AppendOfflineRecordAsync(OfflineDataModel model);
        Task<OfflineDataModel> UpdateOfflineRecordAsync(OfflineDataModel model);
        Task<IEnumerable<UserOfflineStatusResponse>?> GetOfflineStatusAsync(string userId);
        Task<bool> SetOfflineCourtTypeAsync(OfflineDataModel model);
        Task<bool> TerminateOfflineRequestAsync(OfflineDataModel model);
        Task<string> GetDownloadStatusAsync(OfflineDataModel model);
        Task<string> SetDownloadCompletedAsync(OfflineDataModel model);
        Task UpdateOfflineHistoryAsync();
        Task UpdateOfflineSearchTypesAsync();
        Task<List<OfflineSearchTypeBo>> GetOfflineSearchTypesByIdAsync(string leadId);
        Task<string?> FindCaseItemByCaseNumberAsync(int countyId, string caseNumber);
        Task<List<OfflineDataModel>?> GetOfflineWorkQueueAsync();
        Task<List<MyProfileBo>> GetUserProfileAsync(string leadId);
        Task<List<MyProfileBo>> UpdateUserProfileAsync(string leadId, List<MyProfileBo> updates);
        Task<string> GetUserBillingTypeAsync(string leadId);
        Task<string> SetUserBillingTypeAsync(string leadId, string paymentCode);
        Task<GetAdminStatusBo?> GetUserAdminStatusAsync(string leadId);
        Task<List<UserPermissionHistoryBo>?> GetBillTypeHistoryAsync(string leadId);
    }
}