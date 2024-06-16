using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Interfaces
{
    public interface IUserMailbox
    {
        Task<EmailCountBo?> GetCount(MailboxRequest request);
        Task<EmailBodyBo?> GetBody(MailboxRequest request);
        Task<List<EmailListBo>?> GetMailMessages(MailboxRequest request);
    }
}
