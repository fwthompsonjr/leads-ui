using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface IMailBoxRepository
    {
        Task<EmailCountBo?> GetCount(string userId);
        Task<EmailBodyBo?> GetBody(string messageId, string userId);
        Task<List<EmailListBo>?> GetMailMessages(string userId, DateTime? lastUpdate);
    }
}