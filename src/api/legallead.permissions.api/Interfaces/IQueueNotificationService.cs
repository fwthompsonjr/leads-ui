using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Interfaces
{
    public interface IQueueNotificationService
    {
        void Send(QueuedRecord dto, QueueWorkingUserBo user);
    }
}
