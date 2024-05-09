using legallead.email.services;
using legallead.email.utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace legallead.email.actions
{
    internal class BaseEmailActionTemplate(MailMessageService messaging, ISmtpService smtp) : BaseEmailAction(messaging, smtp)
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (context.Result is not OkObjectResult okResult) return;
            if (okResult.Value is not string id) return;
            if (!Guid.TryParse(id, out var _)) return;
            _mailMessageService.With(TemplateNames.None, id);
            if (!CanSend()) return;
            _smtpService.Send(_mailMessageService.Message, id);
        }
    }
}