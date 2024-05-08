using legallead.email.models;
using legallead.email.services;
using legallead.email.utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace legallead.email.actions
{
    internal class PermissionChangeRequested(MailMessageService messaging, ISmtpService smtp) : BaseEmailAction(messaging, smtp)
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (context.Result is not OkObjectResult okResult) return;
            var model = PermissionMapper.Mapper.Map<PermissionChangeResponse>(okResult);
            if (model == null) return;
            var verification = PermissionMapper.Mapper.Map<PermissionChangeValidation>(model);
            if (!verification.IsValid) return;
            var account = _mailMessageService.SettingsDb.GetUserByEmail(model.Email).GetAwaiter().GetResult();
            if (account == null) return;
            var id = account.Id ?? string.Empty;
            if (!Guid.TryParse(id, out var _)) return;
            _mailMessageService.With(TemplateNames.PermissionChangeRequested, id);
            if (_mailMessageService.Message == null || !CanSend()) return;
            var body = model.ToHtml(account, _mailMessageService.Message.Body);
            _mailMessageService.Beautify(body);
            _smtpService.Send(_mailMessageService.Message, id);
        }
    }
}