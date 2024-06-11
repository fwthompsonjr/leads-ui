using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using System.Diagnostics.CodeAnalysis;

namespace legallead.jdbc.implementations
{
    public class MailBoxRepository :
        BaseRepository<EmailListDto>, IMailBoxRepository
    {
        public MailBoxRepository(DataContext context) : base(context)
        {
        }

        public async Task<EmailCountBo?> GetCount(string userId)
        {
            const string prc = EmailProcedureNames.GetCount;
            try
            {
                var parms = BoMapper.GetCountParameters(userId);
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<EmailCountDto>(connection, prc, parms);
                return BoMapper.Map(response);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<EmailBodyBo?> GetBody(string messageId, string userId)
        {
            const string prc = EmailProcedureNames.GetEmailBody;
            try
            {
                var parms = BoMapper.GetBodyParameters(messageId, userId);
                using var connection = _context.CreateConnection();
                var response = await _command.QuerySingleOrDefaultAsync<EmailBodyDto>(connection, prc, parms);
                var mapped = GetEmailBodyBo(response);
                return mapped;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<List<EmailListBo>?> GetMailMessages(string userId, DateTime? lastUpdate)
        {
            const string prc = EmailProcedureNames.GetMailMessages;
            try
            {
                var parms = BoMapper.GetMessagesParameters(userId, lastUpdate);
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<EmailListDto>(connection, prc, parms);
                return BoMapper.Map(response);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public access.")]
        private static EmailBodyBo? GetEmailBodyBo(EmailBodyDto? body)
        {
            var mapped = BoMapper.Map(body);
            if (mapped == null) return default;
            mapped.Body = BoMapper.FromBase64(mapped.Body);
            return mapped;
        }
    }
}