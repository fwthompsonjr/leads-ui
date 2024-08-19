using legallead.email.models;

namespace legallead.permissions.api.Entities
{
    public class QueueNotificationValidationResponse
    {
        public bool IsValid { get; set; }
        public UserRecordSearch? Search { get; set; }
        public UserAccountByEmailBo? Account { get; set; }
    }
}
