﻿using legallead.email.services;
using legallead.email.utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace legallead.email.actions
{
    internal class RegistrationCompleted(MailMessageService messaging, ISmtpService smtp) : BaseEmailAction(messaging, smtp)
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            Task.Run(() =>
            {
                if (context.Result is not OkObjectResult okResult) return;
                if (okResult.Value is not string id) return;
                if (!Guid.TryParse(id, out var _)) return;
                _mailMessageService.With(TemplateNames.RegistrationCompleted, id);
                if (!CanSend()) return;
                _smtpService.Send(_mailMessageService.Message, id);
            });
        }
    }
}
