using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Enumerations
{
    public static class MailboxRequestExtensions
    {

        public static bool IsValid(this MailboxRequest request)
        {
            const StringComparison oic = StringComparison.OrdinalIgnoreCase;
            var names = mailboxRequestTypes.Split(',').ToList();
            if (string.IsNullOrEmpty(request.RequestType)) return false;
            var item = names.Find(x => x.Equals(request.RequestType, oic));
            if (item == null) return false;
            if (!Guid.TryParse(request.UserId, out var _)) return false;
            var valid = item switch
            {
                "body" => IsValidGuid(request.MessageId),
                _ => true
            };
            return valid;
        }

        public static bool IsValid(this MailboxRequest request, ISearchInfrastructure infrastructure, HttpRequest http)
        {
            request.AssignUser(infrastructure, http);
            return request.IsValid();
        }

        private static void AssignUser(this MailboxRequest request, ISearchInfrastructure infrastructure, HttpRequest http)
        {
            var user = infrastructure.GetUserAsync(http).GetAwaiter().GetResult();
            if (user == null)
            {
                request.UserId = string.Empty;
                return;
            }
            request.UserId = user.Id;
        }

        private static bool IsValidGuid(string? test)
        {
            if (string.IsNullOrWhiteSpace(test)) return false;
            return Guid.TryParse(test, out var _);
        }
        private static readonly string mailboxRequestTypes =
            "body,count,messages";
    }
}
