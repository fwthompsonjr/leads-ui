using legallead.email.services;
using legallead.email.utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics.CodeAnalysis;

namespace legallead.email.actions
{
    internal class AccountRegistrationCompleted(MailMessageService messaging, ISmtpService smtp) : IResultFilter
    {
        private readonly MailMessageService _mailMessageService = messaging;
        private readonly ISmtpService _smtpService = smtp;

        public void OnResultExecuting(ResultExecutingContext context)
        {
            // This method runs before the result is executed
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            Task.Run(() =>
            {
                if (context.Result is not OkObjectResult okResult) return;
                if (okResult.Value is not string id) return;
                if (!Guid.TryParse(id, out var _)) return;
                _mailMessageService.With(TemplateNames.AccountRegistration, id);
                if (!CanSend()) return;
                _smtpService.Send(_mailMessageService.Message, id);
            });
        }
        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private bool CanSend()
        {
            return (_mailMessageService.CanSend() && _mailMessageService.Message != null);
        }
    }
}
