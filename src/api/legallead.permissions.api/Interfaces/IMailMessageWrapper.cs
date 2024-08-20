using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Interfaces
{
    public interface IMailMessageWrapper
    {
        void Send(QueuedRecord dto, QueueWorkingUserBo user);
    }
}
