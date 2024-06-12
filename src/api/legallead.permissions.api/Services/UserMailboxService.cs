using legallead.jdbc.interfaces;
using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Services
{
    public class UserMailboxService(IMailBoxRepository repository) : IUserMailbox
    {
        private readonly IMailBoxRepository db = repository;

        public async Task<EmailBodyBo?> GetBody(MailboxRequest request)
        {
            var response = await db.GetBody(request.MessageId, request.UserId);
            return response;
        }

        public async Task<EmailCountBo?> GetCount(MailboxRequest request)
        {
            var response = await db.GetCount(request.UserId);
            return response;
        }

        public async Task<List<EmailListBo>?> GetMailMessages(MailboxRequest request)
        {
            var response = await db.GetMailMessages(request.UserId, request.LastUpdate);
            return response;
        }
    }
}
