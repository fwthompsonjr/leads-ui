using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Interfaces
{
    public interface IUserMailbox
    {
        Task<EmailCountBo?> GetCountAsync(MailboxRequest request);
        Task<EmailBodyBo?> GetBodyAsync(MailboxRequest request);
        Task<List<EmailListBo>?> GetMailMessagesAsync(MailboxRequest request);
    }
}
