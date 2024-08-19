using legallead.email.services;
using legallead.email.utility;
using legallead.permissions.api.Entities;
using Microsoft.AspNetCore.Mvc.Filters;

namespace legallead.permissions.api.Services
{
    [ExcludeFromCodeCoverage(Justification = "Method behavior is wrapper to around inherited code in email assembly")]
    internal class QueueNotificationService(MailMessageService messaging, ISmtpService smtp) : BaseEmailAction(messaging, smtp), IQueueNotificationService
    {
        private static readonly QueueNotificationValidationService validator = new();
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            // this method is intentionally left blank
        }

        public void Send(QueuedRecord dto, QueueWorkingUserBo user)
        {
            var validation = validator.IsValid(dto, user);
            SendMessage(validation);
        }

        private void SendMessage(QueueNotificationValidationResponse validation)
        {
            if (!validation.IsValid || validation.Account == null || validation.Search == null) return;
            try
            {
                var account = validation.Account;
                var conversion = validation.Search;
                var id = account.Id ?? string.Empty;
                _mailMessageService.With(TemplateNames.SearchCompleted, id);
                if (_mailMessageService.Message == null || !CanSend()) return;
                var body = conversion.ToHtml(account, _mailMessageService.Message.Body);
                _mailMessageService.Beautify(body);
                _smtpService.Send(_mailMessageService.Message, id);
            }
            catch (Exception)
            {
                // no action on failure
            }
        }
    }
}
