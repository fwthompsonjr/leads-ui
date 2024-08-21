using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Interfaces
{
    public interface IQueueStatusService
    {
        List<QueueWorkingBo> Insert(QueueInitializeRequest request);
        Task<QueueWorkingBo?> Update(QueueUpdateRequest request);
        Task<List<QueuedRecord>> Fetch();
        Task<KeyValuePair<bool, string>> Start(QueuedRecord search);
        Task Complete(QueueRecordStatusRequest request);
        Task GenerationComplete(QueueCompletionRequest request);
        Task PostStatus(QueueRecordStatusRequest request);
        Task Content(string id, byte[] bytes);
    }
}
