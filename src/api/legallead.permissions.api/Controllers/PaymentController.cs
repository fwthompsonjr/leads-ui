using legallead.permissions.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [SuppressMessage("Sonar Qube",
    "S6960:Controller has multiple responsibilities",
    Justification = "Payment controller actions are all related to payment processing details")]
    public class PaymentController(
        PaymentStripeOption payment,
        ISearchInfrastructure infra,
        IStripeInfrastructure stripe) : ControllerBase
    {
        private readonly PaymentStripeOption stripeOptions = payment;
        private readonly ISearchInfrastructure infrastructure = infra;
        private readonly IStripeInfrastructure stripeService = stripe;
        
        [HttpGet("payment-process-type")]
        [ProducesResponseType<PaymentModeResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<PaymentModeResponse>(StatusCodes.Status202Accepted)]
        public IActionResult GetPaymentType()
        {
            const string live = "live";
            var key = stripeOptions.Key;
            var response = new PaymentModeResponse();
            if (string.IsNullOrWhiteSpace(key)) { return Accepted(response); }
            var isTest = !key.Contains(live, StringComparison.OrdinalIgnoreCase);
            response.IsLive = !isTest;
            response.Name = isTest ? "TEST" : "PROD";
            return Ok(response);
        }

        [HttpGet("product-codes")]
        [ProducesResponseType<PaymentCode>(StatusCodes.Status200OK)]
        public IActionResult ProductCodes()
        {
            return Ok(stripeOptions.Codes);
        }

        [HttpGet("session-status")]
        [ProducesResponseType<object>(StatusCodes.Status200OK)]
        public IActionResult SessionStatus([FromQuery] string session_id)
        {
            return Ok(stripeService.SessionStatus(session_id));
        }

        [HttpPost("create-checkout-session")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType<string>(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType<object>(StatusCodes.Status200OK)]
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
            var model = new PaymentCreateModel(Request, user, guid, request.ProductType);
            var response = await stripeService.CreatePaymentAsync(model, data);
            if (response == null) return UnprocessableEntity(guid);
            return Ok(response);
        }
    }
}
