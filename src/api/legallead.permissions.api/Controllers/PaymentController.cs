using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Stripe;
using legallead.permissions.api.Interfaces;
using legallead.permissions.api.Utility;

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
            var invoice = await infrastructure.CreateInvoice(user.Id, guid);
            if (invoice == null || !invoice.Any()) return UnprocessableEntity(guid);
            var data = invoice.ToList();
            var amount = data.Sum(x => x.Price.GetValueOrDefault(0));
            const decimal minAmount = 0.50m;
            if (amount <= minAmount)
            {
                // close invoice as paid
                var nocost = new
                {
                    Id = Guid.Empty.ToString("D"),
                    PaymentIntentId = Guid.Empty.ToString("D"),
                    clientSecret = string.Empty,
                    externalId = data[0].ExternalId ?? string.Empty,
                    data
                };
                return Ok(nocost);
            }
            var successPg = $"{Request.Scheme}://{Request.Host}/payment-result?sts=success&id={guid}";
            var failurePg = successPg.Replace("success", "cancel");
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions>(),
                Metadata = new Dictionary<string, string>
                {
                    { "user-name", user.UserName },
                    { "user-email", user.Email },
                    { "external-id", data[0].ExternalId ?? string.Empty },
                    { "product-type", request.ProductType }
                },
                Mode = "payment",
                AutomaticTax = new SessionAutomaticTaxOptions { Enabled = false },
                SuccessUrl = successPg,
                CancelUrl = failurePg,
            };
            data.ForEach(d =>
            {
                var item = PricingConverter.ConvertFrom(d, user);
                options.LineItems.Add(item);
            });
            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            var response = new { 
                session.Id,
                session.PaymentIntentId,
                clientSecret = session.RawJObject["client_secret"],
                externalId = data[0].ExternalId ?? string.Empty,
                data
            };
            return Ok(response);
        }
    }
}
