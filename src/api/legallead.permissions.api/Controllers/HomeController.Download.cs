using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    public partial class HomeController
    {

        [Authorize]
        [HttpPost("/payment-fetch-search")]
        public async Task<IActionResult> FetchDownload([FromBody] FetchIntentRequest request)
        {
            var user = await infrastructure.GetUser(Request);
            if (user == null || string.IsNullOrEmpty(user.Id)) return Unauthorized();
            var isLocked = await _lockingDb.IsAccountLocked(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var session = await paymentSvc.IsSessionValid(request.Id);
            var ispaid = await paymentSvc.IsRequestPaid(session);
            if (!ispaid)
            {
                return StatusCode(400, "Unable to find payment for associated download request.");
            }
            if (session == null || string.IsNullOrEmpty(session.JsText))
            {
                return StatusCode(400, "Unable to process request. One or more result artifacts are missing.");
            }
            var isdownload = await paymentSvc.IsRequestDownloadedAndPaid(session);
            if (isdownload)
            {
                return StatusCode(400, "Associated download result has already been delivered.");
            }
            var dwnload = await paymentSvc.GetDownload(session);
            return Ok(dwnload);
        }

        [Authorize]
        [HttpPost("/rollback-download")]
        public async Task<IActionResult> RollbackDownload([FromBody] DownloadResetRequest request)
        {
            var user = await infrastructure.GetUser(Request);
            if (user == null || !user.UserName.Equals(request.UserId, StringComparison.OrdinalIgnoreCase))
                return Unauthorized();
            var isLocked = await _lockingDb.IsAccountLocked(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var session = await paymentSvc.IsSessionValid(request.ExternalId);
            var ispaid = await paymentSvc.IsRequestPaid(session);
            if (!ispaid)
            {
                return StatusCode(400, "Unable to find payment for associated download request.");
            }
            if (session == null || string.IsNullOrEmpty(session.JsText))
            {
                return StatusCode(400, "Unable to process request. One or more result artifacts are missing.");
            }
            var isdownload = await paymentSvc.IsRequestDownloadedAndPaid(session);
            if (!isdownload)
            {
                return StatusCode(400, "Associated download result has not been downloaded.");
            }
            request.UserId = user.Id;
            request.ExternalId = session.InvoiceId;
            var dwnload = await paymentSvc.ResetDownload(request);
            if (dwnload == null) return UnprocessableEntity("Unable to perform reset. Process rejected by server.");
            return Ok(dwnload);
        }

    }
}
