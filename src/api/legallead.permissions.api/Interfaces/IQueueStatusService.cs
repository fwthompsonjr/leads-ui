using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Interfaces
{
    public interface IQueueStatusService
    {
        List<QueueWorkingBo> Insert(QueueInitializeRequest request);
        Task<QueueWorkingBo?> UpdateAsync(QueueUpdateRequest request);
        Task<List<QueuedRecord>> FetchAsync();
        Task<KeyValuePair<bool, string>> StartAsync(QueuedRecord search);
        Task CompleteAsync(QueueRecordStatusRequest request);
        Task GenerationCompleteAsync(QueueCompletionRequest request);
        Task PostStatusAsync(QueueRecordStatusRequest request);
        Task ContentAsync(string id, byte[] bytes);
        Task<List<StatusSummaryByCountyBo>> GetQueueStatusAsync(QueueSummaryRequest request);
        Task<List<StatusSummaryBo>> GetQueueSummaryAsync(QueueSummaryRequest request);
        Task<List<QueueNonPersonBo>> FetchNonPersonQueueAsync();
        bool UpdatePersonList(QueueNonPersonBo bo, string json);
    }
}
