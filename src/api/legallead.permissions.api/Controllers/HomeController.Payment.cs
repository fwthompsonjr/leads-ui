using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    public partial class HomeController
    {

        [HttpGet("/payment-result")]
        public async Task<IActionResult> PaymentLandingAsync([FromQuery] string? sts, [FromQuery] string? id)
        {
            var isValid = await paymentSvc.IsRequestValidAsync(sts, id);
            var content =
                isValid ? Properties.Resources.page_payment_completed
                : Properties.Resources.page_payment_detail_invalid;
            content = await paymentSvc.TransformAsync(isValid, sts, id, content);
            return Content(content, "text/html");
        }

        [HttpGet("/payment-checkout")]
        [ServiceFilter(typeof(SearchPaymentCompleted))]
        public async Task<IActionResult> PaymentCheckoutAsync([FromQuery] string? id)
        {
            var session = await paymentSvc.IsSessionValidAsync(id);
            if (session == null)
            {
                var nodata = Properties.Resources.page_payment_detail_invalid;
                return Content(nodata, "text/html");
            }
            var ispaid = await paymentSvc.IsRequestPaidAsync(session);
            if (ispaid)
            {
                return await PaymentLandingAsync("success", id);
            }
            var content = Properties.Resources.page_invoice_html;
            content = paymentSvc.Transform(session, content);
            return Content(content, "text/html");
        }

        [HttpPost("/payment-fetch-intent")]
        public async Task<IActionResult> FetchIntentAsync([FromBody] FetchIntentRequest request)
        {
            var session = await paymentSvc.IsSessionValidAsync(request.Id);
            if (session == null || string.IsNullOrEmpty(session.ClientId))
            {
                var nodata = Properties.Resources.page_payment_detail_invalid;
                return Content(nodata, "text/html");
            }
            return Json(new { clientSecret = session.ClientId });
        }

    }
}
