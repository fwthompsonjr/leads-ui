using legallead.email.services;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics.CodeAnalysis;

namespace legallead.email.actions
{
    internal abstract class BaseEmailAction(MailMessageService messaging, ISmtpService smtp) : IResultFilter
    {
        protected readonly MailMessageService _mailMessageService = messaging;
        protected readonly ISmtpService _smtpService = smtp;

        public virtual void OnResultExecuting(ResultExecutingContext context)
        {
            // This method runs before the result is executed
        }

        public abstract void OnResultExecuted(ResultExecutedContext context);

        [ExcludeFromCodeCoverage(Justification = "Protected member tested from public method.")]
        protected bool CanSend()
        {
            return (_mailMessageService.CanSend() && _mailMessageService.Message != null);
        }
    }
}