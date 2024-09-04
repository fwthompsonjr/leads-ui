using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    public partial class HomeController
    {

        [Authorize]
        [HttpPost("/payment-fetch-search")]
        public async Task<IActionResult> FetchDownloadAsync([FromBody] FetchIntentRequest request)
        {
            var user = await infrastructure.GetUserAsync(Request);
            if (user == null || string.IsNullOrEmpty(user.Id)) return Unauthorized();
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var session = await paymentSvc.IsSessionValidAsync(request.Id);
            var ispaid = await paymentSvc.IsRequestPaidAsync(session);
            if (!ispaid)
            {
                return StatusCode(400, "Unable to find payment for associated download request.");
            }
            if (session == null || string.IsNullOrEmpty(session.JsText))
            {
                return StatusCode(400, "Unable to process request. One or more result artifacts are missing.");
            }
            var isdownload = await paymentSvc.IsRequestDownloadedAndPaidAsync(session);
            if (isdownload)
            {
                return StatusCode(400, "Associated download result has already been delivered.");
            }
            var dwnload = await paymentSvc.GetDownloadAsync(session);
            return Ok(dwnload);
        }

        [Authorize]
        [HttpPost("/rollback-download")]
        public async Task<IActionResult> RollbackDownloadAsync([FromBody] DownloadResetRequest request)
        {
            var user = await infrastructure.GetUserAsync(Request);
            if (user == null || !user.UserName.Equals(request.UserId, StringComparison.OrdinalIgnoreCase))
                return Unauthorized();
            var isLocked = await _lockingDb.IsAccountLockedAsync(user.Id);
            if (isLocked)
            {
                return Forbid("Account is locked. Contact system administrator to unlock.");
            }
            var session = await paymentSvc.IsSessionValidAsync(request.ExternalId);
            var ispaid = await paymentSvc.IsRequestPaidAsync(session);
            if (!ispaid)
            {
                return StatusCode(400, "Unable to find payment for associated download request.");
            }
            if (session == null || string.IsNullOrEmpty(session.JsText))
            {
                return StatusCode(400, "Unable to process request. One or more result artifacts are missing.");
            }
            var isdownload = await paymentSvc.IsRequestDownloadedAndPaidAsync(session);
            if (!isdownload)
            {
                return StatusCode(400, "Associated download result has not been downloaded.");
            }
            request.UserId = user.Id;
            request.ExternalId = session.InvoiceId;
            var dwnload = await paymentSvc.ResetDownloadAsync(request);
            if (dwnload == null) return UnprocessableEntity("Unable to perform reset. Process rejected by server.");
            return Ok(dwnload);
        }

    }
}
