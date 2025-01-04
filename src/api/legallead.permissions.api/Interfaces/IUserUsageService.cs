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
    }
}