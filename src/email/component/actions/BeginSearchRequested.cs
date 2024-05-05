using legallead.email.services;
using legallead.email.utility;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using legallead.email.models;
using System;
using System.Security.Principal;
using System.Diagnostics.CodeAnalysis;

namespace legallead.email.actions
{
    internal class BeginSearchRequested(MailMessageService messaging, ISmtpService smtp) : BaseEmailAction(messaging, smtp)
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (context.Result is not OkObjectResult okResult) return;
            var conversionJs = JsonConvert.SerializeObject(okResult.Value);
            var conversion = TryConvert(conversionJs);
            if (conversion == null) return;
            var account = _mailMessageService.SettingsDb.GetUserBySearchId(conversion.SearchRequestId).GetAwaiter().GetResult();
            if (account == null) return;
            var id = account.Id ?? string.Empty;
            if (!Guid.TryParse(id, out var _)) return;
            _mailMessageService.With(TemplateNames.BeginSearchRequested, id);
            if (_mailMessageService.Message == null || !CanSend()) return;            
            _mailMessageService.Message.Body = conversion.ToHtml(_mailMessageService.Message.Body);
            _smtpService.Send(_mailMessageService.Message, id);
        }

        [ExcludeFromCodeCoverage(Justification = "Private method tested from public accessor")]
        private static UserRecordSearch? TryConvert(string conversionJs)
        {
            try
            {
                return JsonConvert.DeserializeObject<UserRecordSearch>(conversionJs);
            } catch {  return null; }   
        }
    }
}