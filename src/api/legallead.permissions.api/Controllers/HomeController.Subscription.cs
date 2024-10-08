﻿using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    public partial class HomeController
    {

        [HttpGet("/subscription-result")]
        public async Task<IActionResult> UserLevelLandingAsync([FromQuery] string? sts, [FromQuery] string? id)
        {
            var isValid = await paymentSvc.IsChangeUserLevelAsync(sts, id);
            var content =
                isValid ? Properties.Resources.page_payment_completed
                : Properties.Resources.page_level_request_completed;
            content = await paymentSvc.TransformForPermissionsAsync(isValid, sts, id, content);
            return Content(content, "text/html");
        }

        [HttpGet("/subscription-checkout")]
        public async Task<IActionResult> SubscriptionCheckoutAsync([FromQuery] string? id, string? sessionid)
        {
            var session = await paymentSvc.IsSubscriptionValidAsync(id, sessionid);
            if (session == null)
            {
                var nodata = Properties.Resources.page_payment_detail_invalid;
                return Content(nodata, "text/html");
            }
            var ispaid = await paymentSvc.IsRequestPaidAsync(session);
            if (ispaid)
            {
                return await UserLevelLandingAsync("success", id);
            }
            var clientSecret = GetSubscriptionSecret(session);
            var temp = Properties.Resources.page_invoice_subscription_html;
            var content = paymentSvc.Transform(session, temp) ?? temp;
            content = content.Replace("<!-- payment get intent index -->", clientSecret);
            var result = Content(content, "text/html");
            return result;
        }


        [HttpPost("/subscription-fetch-intent")]
        public async Task<IActionResult> FetchSubscriptionIntentAsync([FromBody] FetchIntentRequest request)
        {
            var nodata = Json(new { clientSecret = Guid.Empty.ToString("D") });
            var session = await subscriptionSvc.GetLevelRequestByIdAsync(request.Id, null);
            if (session == null || string.IsNullOrEmpty(session.SessionId))
            {
                return nodata;
            }
            var clientSecret = GetSubscriptionSecret(session);
            return Json(new { clientSecret });
        }
        protected string GetSubscriptionSecret(LevelRequestBo? session)
        {
            if (session == null) return NoPaymentItem;
            var clientSecret = stripeSvc.FetchClientSecretValueAsync(session).GetAwaiter().GetResult();
            if (!clientSecret.Equals(NoPaymentItem)) return clientSecret;
            return secretSvc.GetSubscriptionSecret(session);
        }
    }
}
