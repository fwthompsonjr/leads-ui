using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Services
{
    public class MailMessageWrapper(IQueueNotificationService service) : IMailMessageWrapper
    {
        protected readonly IQueueNotificationService _notificationService = service;
        public void Send(QueuedRecord dto, QueueWorkingUserBo user)
        {
            _notificationService.Send(dto, user);
        }
    }
}
