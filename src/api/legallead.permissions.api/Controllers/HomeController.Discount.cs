using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [SuppressMessage("Sonar Qube", 
        "S6967:ModelState.IsValid should be called in controller actions", 
        Justification = "Model state is not relevent for these landings")]
    public partial class HomeController
    {

        [HttpGet("/discount-result")]
        public async Task<IActionResult> DiscountLanding([FromQuery] string? sts, [FromQuery] string? id)
        {
            var isValid = await paymentSvc.IsDiscountLevel(sts, id);
            var content =
                isValid ? Properties.Resources.page_payment_completed
                : Properties.Resources.page_level_request_completed;
            content = await paymentSvc.TransformForDiscounts(subscriptionSvc, isValid, id, content);
            return Content(content, "text/html");
        }



        [HttpGet("/discount-checkout")]
        public async Task<IActionResult> DiscountCheckout([FromQuery] string? id, string? sessionid)
        {
            var session = await paymentSvc.IsDiscountValid(id, sessionid);
            if (session == null)
            {
                var nodata = Properties.Resources.page_payment_detail_invalid;
                return Content(nodata, "text/html");
            }
            var ispaid = await paymentSvc.IsDiscountPaid(session);
            if (ispaid)
            {
                // incorrect mapping need new landing for discount
                return await DiscountLanding("success", id);
            }
            var content = Properties.Resources.page_invoice_discount_html;
            var discountRequest = ModelMapper.Mapper.Map<DiscountRequestBo>(session);
            var clientSecret = GetDiscountSecret(session);
            content = paymentSvc.Transform(discountRequest, content);
            content = content.Replace("<!-- payment get intent index -->", clientSecret);
            return Content(content, "text/html");
        }

        [HttpPost("/discount-fetch-intent")]
        
        public async Task<IActionResult> FetchDiscountIntent([FromBody] FetchIntentRequest request)
        {
            var nodata = Json(new { clientSecret = Guid.Empty.ToString("D") });
            var session = await subscriptionSvc.GetDiscountRequestById(request.Id, null);
            if (session == null || string.IsNullOrEmpty(session.SessionId))
            {
                return nodata;
            }
            var clientSecret = GetDiscountSecret(session);
            return Json(new { clientSecret });
        }

        protected string GetDiscountSecret(LevelRequestBo? session)
        {
            if (session == null) return NoPaymentItem;
            var discountRequest = ModelMapper.Mapper.Map<DiscountRequestBo>(session);
            var clientSecret = stripeSvc.FetchClientSecretValue(session).GetAwaiter().GetResult();
            if (!clientSecret.Equals(NoPaymentItem)) return clientSecret;
            return secretSvc.GetDiscountSecret(discountRequest);
        }
    }
}
