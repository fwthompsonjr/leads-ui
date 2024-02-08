using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Stripe;
using legallead.permissions.api.Interfaces;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentStripeOption stripeOptions;
        private readonly ISearchInfrastructure infrastructure;

        public PaymentController(PaymentStripeOption payment, ISearchInfrastructure infrastructure)
        {
            stripeOptions = payment;
            this.infrastructure = infrastructure;
        }

        [HttpGet("product-codes")]
        public IActionResult ProductCodes()
        {
            return Ok(stripeOptions.Codes);
        }

        [HttpGet]
        public IActionResult SessionStatus([FromQuery] string session_id)
        {
            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);
            var response = new { status = session.RawJObject["status"], customer_email = session.RawJObject["customer_details"]["email"] };
            return Ok(response);
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> Create(PaymentCreateRequest request)
        {
            var user = await infrastructure.GetUser(Request);
            var guid = request.Id;
            if (user == null || !Guid.TryParse(guid, out var _)) { return Unauthorized(); }
            var searches = await infrastructure.GetPreview(Request, guid);
            if (searches == null) return UnprocessableEntity(guid);
            var options = new SessionCreateOptions
            {
                UiMode = "embedded",
                LineItems = new List<SessionLineItemOptions>
                {
                  new() {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                    Price = "{{PRICE_ID}}",
                    Quantity = 1,

                  },
                },
                Metadata = new Dictionary<string, string>
                {
                    { "user-name", user.UserName },
                    { "user-email", user.Email },
                    { "product-type", request.ProductType }
                },
                Mode = "payment",
                AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
            };
            var service = new SessionService();
            Session session = service.Create(options);
            var response = new { clientSecret = session.RawJObject["client_secret"] };
            return Ok(response);
        }
    }
}
