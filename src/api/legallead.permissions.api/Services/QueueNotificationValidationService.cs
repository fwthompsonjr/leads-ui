using legallead.email.models;
using legallead.permissions.api.Entities;
using Newtonsoft.Json;

namespace legallead.permissions.api.Services
{
    public class QueueNotificationValidationService
    {
        public QueueNotificationValidationResponse IsValid(QueuedRecord dto, QueueWorkingUserBo user)
        {
            var response = new QueueNotificationValidationResponse();
            var search = TryConvert(dto.Payload);
            if (search == null ||
                string.IsNullOrEmpty(dto.Id) ||
                !Guid.TryParse(dto.Id, out var _)) return response;
            response.Search = new UserRecordSearch
            {
                SearchRequestId = dto.Id,
                Search = search
            };
            var account = new UserAccountByEmailBo { Email = user.Email, Id = user.Id, UserName = user.UserName };
            var id = user.Id ?? string.Empty;
            if (!Guid.TryParse(id, out var _)) return response;
            response.IsValid = true;
            response.Account = account;
            return response;
        }

        private static SearchRequest? TryConvert(string? conversionJs)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(conversionJs)) return null;
                return JsonConvert.DeserializeObject<SearchRequest>(conversionJs);
            }
            catch { return null; }
        }
    }
}
