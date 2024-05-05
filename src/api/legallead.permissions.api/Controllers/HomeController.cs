using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace legallead.permissions.api.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly IPaymentHtmlTranslator paymentSvc;
        private readonly ISearchInfrastructure infrastructure;
        private readonly ISubscriptionInfrastructure subscriptionSvc;
        private readonly ICustomerLockInfrastructure _lockingDb;
        private readonly IStripeInfrastructure stripeSvc;
        public HomeController(
            IPaymentHtmlTranslator service,
            ISearchInfrastructure search,
            ISubscriptionInfrastructure subscriptionSvc,
            ICustomerLockInfrastructure lockingDb,
            IStripeInfrastructure stripe)
        {
            paymentSvc = service;
            infrastructure = search;
            this.subscriptionSvc = subscriptionSvc;
            _lockingDb = lockingDb;
            stripeSvc = stripe;
        }
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            var content = IndexHtml;
            return Content(content, "text/html");
        }

        [HttpGet("/payment-result")]
        public async Task<IActionResult> PaymentLanding([FromQuery] string? sts, [FromQuery] string? id)
        {
            var isValid = await paymentSvc.IsRequestValid(sts, id);
            var content =
                isValid ? Properties.Resources.page_payment_completed
                : Properties.Resources.page_payment_detail_invalid;
            content = await paymentSvc.Transform(isValid, sts, id, content);
            return Content(content, "text/html");
        }

        [HttpGet("/subscription-result")]
        public async Task<IActionResult> UserLevelLanding([FromQuery] string? sts, [FromQuery] string? id)
        {
            var isValid = await paymentSvc.IsChangeUserLevel(sts, id);
            var content =
                isValid ? Properties.Resources.page_payment_completed
                : Properties.Resources.page_level_request_completed;
            content = await paymentSvc.TransformForPermissions(isValid, sts, id, content);
            return Content(content, "text/html");
        }

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

        [HttpGet("/payment-checkout")]
        // [ServiceFilter(typeof(SearchPaymentCompleted))]
        public async Task<IActionResult> PaymentCheckout([FromQuery] string? id)
        {
            var session = await paymentSvc.IsSessionValid(id);
            if (session == null)
            {
                var nodata = Properties.Resources.page_payment_detail_invalid;
                return Content(nodata, "text/html");
            }
            var ispaid = await paymentSvc.IsRequestPaid(session);
            if (ispaid)
            {
                return await PaymentLanding("success", id);
            }
            var content = Properties.Resources.page_invoice_html;
            content = paymentSvc.Transform(session, content);
            return Content(content, "text/html");
        }

        [HttpGet("/subscription-checkout")]
        public async Task<IActionResult> SubscriptionCheckout([FromQuery] string? id, string? sessionid)
        {
            var session = await paymentSvc.IsSubscriptionValid(id, sessionid);
            if (session == null)
            {
                var nodata = Properties.Resources.page_payment_detail_invalid;
                return Content(nodata, "text/html");
            }
            var ispaid = await paymentSvc.IsRequestPaid(session);
            if (ispaid)
            {
                return await UserLevelLanding("success", id);
            }
            var content = Properties.Resources.page_invoice_subscription_html;
            content = paymentSvc.Transform(session, content);
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
            content = paymentSvc.Transform(discountRequest, content);
            return Content(content, "text/html");
        }

        [HttpPost("/payment-fetch-intent")]
        public async Task<IActionResult> FetchIntent([FromBody] FetchIntentRequest request)
        {
            var session = await paymentSvc.IsSessionValid(request.Id);
            if (session == null || string.IsNullOrEmpty(session.ClientId))
            {
                var nodata = Properties.Resources.page_payment_detail_invalid;
                return Content(nodata, "text/html");
            }
            return Json(new { clientSecret = session.ClientId });
        }


        [HttpPost("/subscription-fetch-intent")]
        public async Task<IActionResult> FetchSubscriptionIntent([FromBody] FetchIntentRequest request)
        {
            var nodata = Json(new { clientSecret = Guid.Empty.ToString("D") });
            var session = await subscriptionSvc.GetLevelRequestById(request.Id, null);
            if (session == null || string.IsNullOrEmpty(session.SessionId))
            {
                return nodata;
            }
            var clientSecret = await stripeSvc.FetchClientSecret(session);
            return Json(new { clientSecret });
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
            var clientSecret = await stripeSvc.FetchClientSecret(session);
            return Json(new { clientSecret });
        }

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

        private static string? _index;
        private static string IndexHtml => _index ??= GetIndex();

        private static string GetIndex()
        {
            var operations = new List<string>()
            {
                "Account Management",
                "Account Registration",
                "Account Login",
                "Account Password Reset",
                "Common Lists",
                "Manage User Permssions",
                "Manage User Profile",
            };
            var listitems = "<ul>" + string.Join(Environment.NewLine, operations.Select(x => $"<li>{x}</li>")) + "</ul>";
            var content = new StringBuilder("<html>");
            content.AppendLine();
            content.AppendLine("<head>");
            content.AppendLine(" <meta charset=`UTF-8` />");
            content.AppendLine(" <meta name=`viewport` content=`width=device-width, initial-scale=1.0` />");
            content.AppendLine(" <meta http-equiv=`X-UA-Compatible` content=`ie=edge` />");
            content.AppendLine(" <title>legallead: permissions api</title>");
            content.AppendLine(" <link rel=`preconnect` href=`https://fonts.gstatic.com` />");
            content.AppendLine(" <link href=`https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap`");
            content.AppendLine(" rel=`stylesheet` />");
            content.AppendLine(" <link rel=`stylesheet`");
            content.AppendLine(" href=`https://unpkg.com/@astrouxds/astro-web-components/dist/astro-web-components/astro-web-components.css` />");
            content.AppendLine(" <script type=`module`");
            content.AppendLine(" src=`https://unpkg.com/@astrouxds/astro-web-components/dist/astro-web-components/astro-web-components.esm.js`></script>");
            content.AppendLine("</head>");
            content.AppendLine("<body>");
            content.AppendLine(" <rux-container name=`tha-container` style=`padding: 10px`>");
            content.AppendLine("   <div slot=`header`>");
            content.AppendLine("     legallead: permissions api");
            content.AppendLine("   </div>");
            content.AppendLine("   <div>");
            content.AppendLine("     <h2Permissions Api</h2>");
            content.AppendLine("     <blockquote>");
            content.AppendLine("      The legallead.permissions.api is a component used to manage authentication and authorization of <br/>");
            content.AppendLine("      all key activities pertaining to users and applications in the leadlead applications suite.");
            content.AppendLine("     </blockquote>");
            content.AppendLine("   </div>");
            content.AppendLine("   <div>");
            content.AppendLine("     <h2>Operations Overview: </h2>");
            content.AppendLine(listitems);
            content.AppendLine("   </div>");
            content.AppendLine(" </rux-container>");
            content.AppendLine("</body>");
            content.AppendLine("</html>");
            content.Replace('`', '"');
            return content.ToString();
        }
    }
}