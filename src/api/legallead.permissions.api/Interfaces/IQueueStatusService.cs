using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Interfaces
{
    public interface IQueueStatusService
    {
        List<QueueWorkingBo> Insert(QueueInitializeRequest request);
        Task<QueueWorkingBo?> Update(QueueUpdateRequest request);
        Task<List<QueuedRecord>> Fetch();
    }
}
